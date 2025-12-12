namespace Hawky.SafeArea
{
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif
    public class SafeAreaHelper : MonoBehaviour
    {
        [SerializeField] private RectTransform[] modifiedRectTransform;

        private void Awake()
        {
            this.UnApplySafeArea();
            SafeAreaManager.Instance?.Register(this);
        }

        private void OnDestroy()
        {
            var ins = SafeAreaManager.Instance;
            if (ins != null)
            {
                ins.UnRegister(this);
            }
        }

        public void ApplySafeArea(Vector2 anchorMin, Vector2 anchorMax)
        {
            foreach (var rectTransform in this.modifiedRectTransform)
            {
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
                rectTransform.ForceUpdateRectTransforms();
            }
        }

        [ContextMenu("UnApply SafeArea")]
        private void UnApplySafeArea()
        {
            foreach (var rectTransform in this.modifiedRectTransform)
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Apply SafeArea")]
        private void ApplySafeArea()
        {
            SafeAreaManager.ForceApplySafeArea(this);
        }
#endif
    }
}
