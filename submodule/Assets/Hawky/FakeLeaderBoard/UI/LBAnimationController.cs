using Hawky.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Hawky.FakeLeaderBoard.UI
{
    public class LBUIAnimationController : MonoBehaviour
    {
        [SerializeField] private LBItemView _itemViewPrefab;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _viewPort;
        [SerializeField] private RectTransform _rootRect;
        [SerializeField] private bool scale = true;

        private List<LBItemView> _itemPool = new List<LBItemView>();
        private List<RectTransform> _rectPaddingPool = new List<RectTransform>();

        private float _itemSize;
        private float _rectSize;

        private List<Data> _data;
        private Data _selfData;

        private void Awake()
        {
            _itemSize = _itemViewPrefab.GetComponent<RectTransform>().rect.height;
            _rectSize = _viewPort.rect.height;
        }

        private RectTransform CreatePadding()
        {
            var animationPaddingElement = new GameObject("Animation Padding", typeof(RectTransform));
            var _animationPadding = animationPaddingElement.GetComponent<RectTransform>();
            _animationPadding.transform.SetParent(_rootRect);
            _animationPadding.pivot = new Vector2(0, 1);
            _animationPadding.sizeDelta = Vector2.zero;
            _animationPadding.gameObject.SetActive(true);

            _rectPaddingPool.Add(_animationPadding);

            return _animationPadding;
        }

        public void SetData(List<Data> LeaderBoardData, int oldId, int oldPoint, Action onComplete)
        {
            _data = LeaderBoardData.OrderBy(x => x.id).ToList();
            _selfData = _data.Find(x => x.isSelf);

            // setup oldData
            var beginId = Mathf.Clamp(Mathf.Min(oldId - 10, _selfData.id - 10), 1, LeaderBoardData.Count);
            var endId = Mathf.Clamp(Mathf.Max(oldId + 10, _selfData.id + 10), 1, LeaderBoardData.Count);
            LBItemView selfView = null;

            List<LBItemView> allLbOtherItemView = new List<LBItemView>();

            _scrollRect.vertical = false;
            var allDataShow = _data.FindAll(x => x.id >= beginId && x.id <= endId && x != _selfData).Select(x => new Data
            {
                id = x.id,
                isSelf = x.isSelf,
                point = x.point,
                userName = x.userName,
            }).ToList();
            var counterId = beginId;

            var currentAnchor = Vector2.zero;
            foreach (var data in allDataShow)
            {
                if (counterId == oldId)
                {
                    var oldSelfData = new Data
                    {
                        id = counterId,
                        userName = _selfData.userName,
                        isSelf = true,
                        point = oldPoint,
                    };

                    var newItem = GetItemFromPool();
                    newItem.gameObject.SetActive(true);
                    newItem.SetData(oldSelfData);
                    newItem.transform.SetAsLastSibling();
                    newItem.GetComponent<RectTransform>().anchoredPosition = currentAnchor;

                    currentAnchor += new Vector2(0, -_itemSize);
                    counterId++;

                    selfView = newItem;
                }

                var newItem2 = GetItemFromPool();
                newItem2.transform.SetAsLastSibling();
                newItem2.gameObject.SetActive(true);

                var oldData = new Data
                {
                    id = counterId,
                    point = data.point,
                    userName = data.userName,
                };
                newItem2.SetData(oldData);
                newItem2.GetComponent<RectTransform>().anchoredPosition = currentAnchor;

                currentAnchor += new Vector2(0, -_itemSize);
                allLbOtherItemView.Add(newItem2);

                counterId++;
            }

            if (counterId == oldId)
            {
                var oldSelfData = new Data
                {
                    id = counterId,
                    userName = _selfData.userName,
                    isSelf = true,
                    point = oldPoint,
                };

                var newItem = GetItemFromPool();
                newItem.gameObject.SetActive(true);
                newItem.transform.SetAsLastSibling();
                newItem.SetData(oldSelfData);
                newItem.GetComponent<RectTransform>().anchoredPosition = currentAnchor;

                currentAnchor += new Vector2(0, -_itemSize);
                counterId++;

                selfView = newItem;
            }

            var oldIndex = oldId - beginId;
            _rootRect.anchoredPosition = oldIndex * new Vector2(0, _itemSize);

            var pad1 = GetPaddingFromPool();
            pad1.gameObject.SetActive(true);
            pad1.SetAsFirstSibling();
            pad1.sizeDelta = new Vector2(0, _itemSize);

            var pad2 = GetPaddingFromPool();
            pad2.gameObject.SetActive(true);
            pad2.SetAsLastSibling();
            pad2.sizeDelta = new Vector2(0, _itemSize);

            if (oldId > _selfData.id)
            {
                StartCoroutine(StartAnimation());
            }
            else
            {
                selfView.SetId(_selfData.id);
                selfView.SetPoint(_selfData.point);

                DestroyPad(pad1);
                DestroyPad(pad2);
                _scrollRect.vertical = true;

                onComplete?.Invoke();
            }

            IEnumerator StartAnimation()
            {
                yield return new WaitForSeconds(0.5f);

                selfView.Layout.ignoreLayout = true;
                selfView.transform.SetAsLastSibling();

                var pad3 = GetPaddingFromPool();
                pad3.gameObject.SetActive(true);
                pad3.SetSiblingIndex(oldIndex + 2);
                pad3.sizeDelta = new Vector2(0, _itemSize);

                if (scale)
                {
                    selfView.ZoomIn();
                    yield return new WaitForSeconds(0.5f);
                }

                pad3.sizeDelta = new Vector2(0, _itemSize);

                var selfViewRect = selfView.GetComponent<RectTransform>();
                var currentId = oldId;

                var counter = 0f;
                var timer = 0.8f;

                var offsetAnchor = _itemSize * (oldId - _selfData.id);
                var beginAnchor = selfViewRect.anchoredPosition.y;
                var targetAnchor = selfViewRect.anchoredPosition.y + offsetAnchor;

                var beginContentAnchor = _rootRect.anchoredPosition.y;
                var targetContentAnchor = beginContentAnchor - offsetAnchor;

                var beginId = oldId;
                var targetId = _selfData.id;

                var beginPoint = oldPoint;
                var targetPoint = _selfData.point;

                var targetTransIndex = oldIndex - oldId + _selfData.id;
                var pad4 = GetPaddingFromPool();
                pad4.gameObject.SetActive(true);
                pad4.SetSiblingIndex(targetTransIndex + 2);
                pad4.sizeDelta = Vector2.zero;
                allLbOtherItemView.FindAll(x => x.Id >= _selfData.id && x.Id < oldId).ForEach(x => x.ModifyId(1));

                while (counter < timer)
                {
                    counter = Mathf.Clamp(counter + Time.deltaTime, 0, timer);

                    var newAnchor = EaseCalculator.GetValue(beginAnchor, targetAnchor, timer, counter, EaseType.InOutSine);
                    selfViewRect.anchoredPosition = new Vector2(0, newAnchor);

                    var newContentAnchor = EaseCalculator.GetValue(beginContentAnchor, targetContentAnchor, timer, counter, EaseType.InOutSine);
                    _rootRect.anchoredPosition = new Vector2(0, newContentAnchor);

                    var pad4Size = EaseCalculator.GetValue(0, _itemSize, timer, counter, EaseType.InOutSine);
                    pad4.sizeDelta = new Vector2(0, pad4Size);

                    var pad3Size = EaseCalculator.GetValue(_itemSize, 0, timer, counter, EaseType.InOutSine);
                    pad3.sizeDelta = new Vector2(0, pad3Size);

                    var id = (int)EaseCalculator.GetValue(beginId, targetId, timer, counter, EaseType.InOutSine);
                    selfView.SetId(id);

                    var point = (int)EaseCalculator.GetValue(beginPoint, targetPoint, timer, counter, EaseType.InOutSine);
                    selfView.SetPoint(point);

                    LayoutRebuilder.MarkLayoutForRebuild(_rootRect);
                    yield return null;
                }

                DestroyPad(pad3);
                yield return new WaitForSeconds(0.3f);

                if (scale)
                {
                    selfView.ZoomOut();
                    yield return new WaitForSeconds(0.3f);
                }


                DestroyPad(pad4);
                selfView.transform.SetSiblingIndex(targetTransIndex + 2);
                selfView.Layout.ignoreLayout = false;

                yield return null;

                counter = 0f;
                timer = 0.3f;

                beginContentAnchor = _rootRect.anchoredPosition.y;
                targetContentAnchor = beginContentAnchor - _itemSize;

                while (counter < timer)
                {
                    counter = Mathf.Clamp(counter + Time.deltaTime, 0, timer);
                    var pad12Size = EaseCalculator.GetValue(_itemSize, 0, timer, counter, EaseType.InOutSine);

                    pad1.sizeDelta = new Vector2(0, pad12Size);
                    pad2.sizeDelta = new Vector2(0, pad12Size);

                    if (_selfData.id != 1)
                    {
                        var newContentAnchor = EaseCalculator.GetValue(beginContentAnchor, targetContentAnchor, timer, counter, EaseType.InOutSine);
                        _rootRect.anchoredPosition = new Vector2(0, newContentAnchor);
                    }

                    LayoutRebuilder.MarkLayoutForRebuild(_rootRect);
                    yield return null;
                }

                DestroyPad(pad1);
                DestroyPad(pad2);
                _scrollRect.vertical = true;

                onComplete?.Invoke();
            }
        }

        private LBItemView GetItemFromPool()
        {
            var item = _itemPool.Find(x => x.gameObject.activeSelf == false);

            if (item == null)
            {
                item = GameObject.Instantiate(_itemViewPrefab, _rootRect);
                _itemPool.Add(item);
            }

            return item;
        }

        private RectTransform GetPaddingFromPool()
        {
            var padding = _rectPaddingPool.Find(x => x.gameObject.activeSelf == false);

            if (padding == null)
            {
                return CreatePadding();
            }

            return padding;
        }

        public void ResetAll()
        {
            StopAllCoroutines();

            _itemPool.ForEach(x =>
            {
                GameObject.Destroy(x.gameObject);
            });

            _rectPaddingPool.ForEach(x =>
            {
                GameObject.Destroy(x.gameObject);
            });

            _itemPool.Clear();
            _rectPaddingPool.Clear();
        }

        private void DestroyItem(LBItemView item)
        {
            _itemPool.Remove(item);

            GameObject.Destroy(item.gameObject);
        }

        private void DestroyPad(RectTransform game)
        {
            _rectPaddingPool.Remove(game);

            GameObject.Destroy(game.gameObject);
        }
    }
}
