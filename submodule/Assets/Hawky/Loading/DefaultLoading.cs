using System.Collections.Generic;
using DG.Tweening;

namespace Hawky.Loading
{
    public class DefaultLoading : ILoadingHandler
    {
        private float _duration;
        private int _totalPoint;

        private int _currentPoint;
        public DefaultLoading(float duration, int totalPoint)
        {
            _duration = duration;
            _totalPoint = totalPoint;
        }

        public int CurrentPoint()
        {
            return _currentPoint;
        }

        public string CurrentText()
        {
            return "Loading";
        }

        public string GetId()
        {
            return LoadingHandlerId.DEFAULT;
        }

        public void OnRequireLoadDone(Dictionary<string, ILoadingHandler> handlers)
        {

        }

        public void RequireHandlerIds(List<string> requires)
        {

        }

        public void StartLoad()
        {
            DOVirtual.Int(0, _totalPoint, _duration, (x) =>
            {
                _currentPoint = x;
            })
            .SetEase(Ease.Linear);
        }

        public int TotalPoint()
        {
            return _totalPoint;
        }
    }
}
