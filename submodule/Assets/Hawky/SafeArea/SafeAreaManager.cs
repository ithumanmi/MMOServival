namespace Hawky.SafeArea
{
    using Hawky;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SafeAreaManager : MonoSingleton<SafeAreaManager>
    {

        private Dictionary<int, SafeAreaHelper> dictCanvasSafeAreas;
        private Rect lastSafeArea = Rect.zero;

        private Vector2 anchorMin, anchorMax;

        private void Awake()
        {
            this.dictCanvasSafeAreas = new Dictionary<int, SafeAreaHelper>();
        }

        private void Start()
        {
            this.StartCoroutine(this.CheckSafeArea());
        }

        public void Register(SafeAreaHelper safeArea)
        {
            var instanceId = safeArea.GetInstanceID();
            if (this.dictCanvasSafeAreas.ContainsKey(instanceId))
            {
                return;
            }

            this.dictCanvasSafeAreas.Add(instanceId, safeArea);
#if UNITY_EDITOR
            //cheat for unity 2020
            var safeAreaRect = Screen.safeArea;
            if (Mathf.Approximately(safeAreaRect.width, Screen.width) &&
                Mathf.Approximately(safeAreaRect.height, Screen.height))
            {
                return;
            }
#endif
            safeArea.ApplySafeArea(this.anchorMin, this.anchorMax);
        }

        public void UnRegister(SafeAreaHelper safeArea)
        {
            var instanceId = safeArea.GetInstanceID();
            this.dictCanvasSafeAreas.Remove(instanceId);
        }

        private IEnumerator CheckSafeArea()
        {
            while (true)
            {
                var safeArea = Screen.safeArea;
                //cheat for unity 2020
#if UNITY_EDITOR
                if (Mathf.Approximately(safeArea.width, Screen.width) &&
                    Mathf.Approximately(safeArea.height, Screen.height))
                {
                    yield return new WaitForSecondsRealtime(0.5f);
                    continue;
                }
#endif
                if (this.lastSafeArea != safeArea)
                {
                    this.lastSafeArea = safeArea;
                    this.SetAnchor();
                    this.ApplySafeAreaAll();
                }

                yield return new WaitForSecondsRealtime(0.5f);
            }
        }

        private void ApplySafeAreaAll()
        {
            foreach (var dictCanvasSafeArea in this.dictCanvasSafeAreas.Values)
            {
                dictCanvasSafeArea.ApplySafeArea(this.anchorMin, this.anchorMax);
            }
        }

        private void SetAnchor()
        {
            this.anchorMin = this.lastSafeArea.position;
            this.anchorMax = this.lastSafeArea.position + this.lastSafeArea.size;

            this.anchorMin.x /= Screen.currentResolution.width;
            this.anchorMin.y /= Screen.currentResolution.height;

            this.anchorMax.x /= Screen.currentResolution.width;
            this.anchorMax.y /= Screen.currentResolution.height;
        }

#if UNITY_EDITOR
        public static void ForceApplySafeArea(SafeAreaHelper safeAreaHelper)
        {
            var safeArea = Screen.safeArea;

            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.currentResolution.width;
            anchorMin.y /= Screen.currentResolution.height;

            anchorMax.x /= Screen.currentResolution.width;
            anchorMax.y /= Screen.currentResolution.height;

            safeAreaHelper.ApplySafeArea(anchorMin, anchorMax);
        }
#endif
    }
}
