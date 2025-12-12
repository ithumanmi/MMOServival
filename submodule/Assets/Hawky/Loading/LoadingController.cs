using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hawky.EventObserver;
using Hawky.MyCoroutine;

namespace Hawky.Loading
{
    public class LoadingController : ILoadingController
    {
        private string _id;
        private List<ILoadingHandler> _handlers;
        private int _currentPoint;
        private int _totalPoint;

        private Dictionary<string, List<string>> _dictRequires = new Dictionary<string, List<string>>();

        public string GetId()
        {
            return _id;
        }

        // Constructor mặc định khởi tạo danh sách handler và gán id ngẫu nhiên
        public LoadingController() : this(Guid.NewGuid().ToString())
        {
            _handlers = new List<ILoadingHandler>();
        }

        // Constructor khởi tạo với loadingId được cung cấp
        public LoadingController(string loadingId)
        {
            _id = loadingId;
            _handlers = new List<ILoadingHandler>();
        }

        // Thêm một handler vào danh sách
        public void AddHandler(ILoadingHandler handler)
        {
            _handlers.Add(handler);

            var newList = new List<string>();
            handler.RequireHandlerIds(newList);

            _dictRequires[handler.GetId()] = newList;
        }

        // Bắt đầu quá trình load
        public void StartLoad()
        {
            var dictHandlers = _handlers.ToDictionary(x => x.GetId());

            // Chuẩn bị hàng chờ các loading theo thứ tự dựa vào requireHandlerId của từng handler
            Queue<List<ILoadingHandler>> handlerInOrder = PrepareHandlersInOrder(dictHandlers);

            _totalPoint = _handlers.Sum(h => h.TotalPoint());
            _currentPoint = 0;

            // Bắt đầu coroutine để quản lý quá trình load
            CoroutineManager.Ins.Start(LoadingProgress(handlerInOrder));
        }

        // Trả về tổng điểm hiện tại
        public int CurrentPoint()
        {
            return _currentPoint;
        }

        // Trả về tổng điểm
        public int TotalPoint()
        {
            return _totalPoint;
        }

        // Chuẩn bị hàng chờ các loading theo thứ tự dựa vào requireHandlerId
        private Queue<List<ILoadingHandler>> PrepareHandlersInOrder(Dictionary<string, ILoadingHandler> dictHandlers)
        {
            Queue<List<ILoadingHandler>> handlerInOrder = new Queue<List<ILoadingHandler>>();
            Dictionary<string, int> inDegree = new Dictionary<string, int>();
            HashSet<string> visited = new HashSet<string>();

            // Tính in-degree cho từng handler
            foreach (var handler in _handlers)
            {
                inDegree[handler.GetId()] = 0;
            }

            // Cập nhật in-degree dựa trên các requireHandlerId
            foreach (var handler in _handlers)
            {
                foreach (var requiredId in _dictRequires[handler.GetId()])
                {
                    inDegree[requiredId]++;
                }
            }

            // Phát hiện vòng lặp và báo lỗi
            foreach (var handler in _handlers)
            {
                if (DetectCycle(handler, dictHandlers, new HashSet<string>(), visited, _dictRequires))
                {
                    throw new InvalidOperationException($"Phát hiện vòng lặp phụ thuộc liên quan đến handler {handler.GetId()}");
                }
            }

            // Tạo danh sách các handler không có phụ thuộc (in-degree = 0)
            List<ILoadingHandler> independentHandlers = inDegree
                .Where(kvp => kvp.Value == 0)
                .Select(kvp => dictHandlers[kvp.Key])
                .ToList();

            handlerInOrder.Enqueue(independentHandlers);

            // Sắp xếp topological
            while (independentHandlers.Count > 0)
            {
                List<ILoadingHandler> nextIndependentHandlers = new List<ILoadingHandler>();

                foreach (var handler in independentHandlers)
                {
                    foreach (var dependentHandler in _handlers)
                    {
                        if (_dictRequires[dependentHandler.GetId()].Contains(handler.GetId()))
                        {
                            inDegree[dependentHandler.GetId()]--;

                            if (inDegree[dependentHandler.GetId()] == 0)
                            {
                                nextIndependentHandlers.Add(dependentHandler);
                            }
                        }
                    }
                }

                if (nextIndependentHandlers.Count > 0)
                {
                    handlerInOrder.Enqueue(nextIndependentHandlers);
                }

                independentHandlers = nextIndependentHandlers;
            }

            return handlerInOrder;
        }

        // Phát hiện vòng lặp trong các phụ thuộc của handler sử dụng DFS
        private bool DetectCycle(ILoadingHandler handler, Dictionary<string, ILoadingHandler> dictHandlers, HashSet<string> currentPath, HashSet<string> visited, Dictionary<string, List<string>> dictRequires)
        {
            if (currentPath.Contains(handler.GetId()))
            {
                return true;
            }

            if (visited.Contains(handler.GetId()))
            {
                return false;
            }

            currentPath.Add(handler.GetId());
            visited.Add(handler.GetId());

            foreach (var requiredId in dictRequires[handler.GetId()])
            {
                if (dictHandlers.ContainsKey(requiredId))
                {
                    if (DetectCycle(dictHandlers[requiredId], dictHandlers, currentPath, visited, dictRequires))
                    {
                        return true;
                    }
                }
            }

            currentPath.Remove(handler.GetId());
            return false;
        }

        // Coroutine để quản lý quá trình load các handler
        private IEnumerator LoadingProgress(Queue<List<ILoadingHandler>> handlerInOrder)
        {
            // Tính tổng điểm và tổng điểm của phiên đầu tiên
            int totalPointFirstSession = handlerInOrder.Peek().Sum(h => h.TotalPoint());

            // Gọi sự kiện LOADING_BEGIN với các thông tin mới
            EventObs.Ins.ExcuteEvent(EventName.LOADING_BEGIN, new LoadingBeginEvent(
                _id,
                "",
                totalPointFirstSession.ToString(),
                _totalPoint.ToString()
            ));

            while (handlerInOrder.Count > 0)
            {
                var handlers = handlerInOrder.Dequeue();
                int totalPointInSession = handlers.Sum(h => h.TotalPoint());
                int currentPointInSession = 0;

                // Chuẩn bị các handler phụ thuộc cho từng handler
                foreach (var handler in handlers)
                {
                    var dependentHandlers = _handlers
                        .Where(h => _dictRequires[handler.GetId()].Contains(h.GetId()))
                        .ToDictionary(h => h.GetId());
                    handler.OnRequireLoadDone(dependentHandlers);
                }

                // Bắt đầu load tất cả các handler trong danh sách
                List<IEnumerator> loadingCoroutines = handlers.Select(handler => LoadHandler(handler)).ToList();
                while (loadingCoroutines.Any(c => c.MoveNext()))
                {
                    foreach (var coroutine in loadingCoroutines)
                    {
                        coroutine.MoveNext();
                    }

                    currentPointInSession = handlers.Sum(h => h.CurrentPoint());
                    _currentPoint = _handlers.Sum(h => h.CurrentPoint());

                    // Lấy text từ handler đang load cuối cùng
                    var currentText = handlers
                        .Where(h => h.CurrentPoint() < h.TotalPoint())
                        .Select(h => h.CurrentText())
                        .FirstOrDefault() ?? handlers.Last().CurrentText();

                    EventObs.Ins.ExcuteEvent(EventName.LOADING_UPDATE, new LoadingUpdateEvent(
                        _id,
                        currentText,
                        _totalPoint,
                        _currentPoint,
                        totalPointInSession,
                        currentPointInSession
                    ));

                    yield return null; // Đợi cho đến frame tiếp theo
                }
            }

            _currentPoint = _totalPoint;

            EventObs.Ins.ExcuteEvent(EventName.LOADING_END, new LoadingEndEvent(_id));
        }

        // Coroutine để load từng handler
        private IEnumerator LoadHandler(ILoadingHandler handler)
        {
            handler.StartLoad();
            while (handler.CurrentPoint() < handler.TotalPoint())
            {
                yield return null; // Đợi cho đến frame tiếp theo
            }
        }
    }
}
