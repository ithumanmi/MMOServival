using System;
using UnityEngine;

namespace Hawky.Math
{
    public enum EaseType
    {
        Unset,
        Linear,
        InSine,
        OutSine,
        InOutSine,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        InElastic,
        OutElastic,
        InOutElastic,
        InBack,
        OutBack,
        InOutBack,
        InBounce,
        OutBounce,
        InOutBounce,
        Flash,
        InFlash,
        OutFlash,
        InOutFlash
    }

    public class EaseCalculator
    {
        public static Vector3 GetVector3Value(Vector3 from, Vector3 to, float time, float progress, EaseType ease)
        {
            return new Vector3(
                GetValue(from.x, to.x, time, progress, ease),
                GetValue(from.y, to.y, time, progress, ease),
                GetValue(from.z, to.z, time, progress, ease)
                );
        }
        public static float GetValue(float from, float to, float time, float progress, EaseType ease)
        {
            return GetValue(from, to, progress / time, ease);
        }

        public static float GetValue(float from, float to, float percentTime, EaseType ease)
        {
            percentTime = Mathf.Clamp01(percentTime);

            return from + (to - from) * CalculatorProgress(percentTime, ease);
        }

        public static float CalculatorProgress(float percentTime, EaseType easeType)
        {
            switch (easeType)
            {
                case EaseType.InSine:
                    return EaseInSine(percentTime);
                case EaseType.OutSine:
                    return EaseOutSine(percentTime);
                case EaseType.InOutSine:
                    return EaseInOutSine(percentTime);
                case EaseType.InQuad:
                    return EaseInQuad(percentTime);
                case EaseType.OutQuad:
                    return EaseOutQuad(percentTime);
                case EaseType.InOutQuad:
                    return EaseInOutQuad(percentTime);
                case EaseType.InCubic:
                    return EaseInCubic(percentTime);
                case EaseType.OutCubic:
                    return EaseOutCubic(percentTime);
                case EaseType.InOutCubic:
                    return EaseInOutCubic(percentTime);
                case EaseType.InQuart:
                    return EaseInQuart(percentTime);
                case EaseType.OutQuart:
                    return EaseOutQuart(percentTime);
                case EaseType.InOutQuart:
                    return EaseInOutQuart(percentTime);
                case EaseType.InQuint:
                    return EaseInQuint(percentTime);
                case EaseType.OutQuint:
                    return EaseOutQuint(percentTime);
                case EaseType.InOutQuint:
                    return EaseInOutQuint(percentTime);
                case EaseType.InExpo:
                    return EaseInExpo(percentTime);
                case EaseType.OutExpo:
                    return EaseOutExpo(percentTime);
                case EaseType.InOutExpo:
                    return EaseInOutExpo(percentTime);
                case EaseType.InCirc:
                    return EaseInCirc(percentTime);
                case EaseType.OutCirc:
                    return EaseOutCirc(percentTime);
                case EaseType.InOutCirc:
                    return EaseInOutCirc(percentTime);
                case EaseType.InBack:
                    return EaseInBack(percentTime);
                case EaseType.OutBack:
                    return EaseOutBack(percentTime);
                case EaseType.InOutBack:
                    return EaseInOutBack(percentTime);
                case EaseType.InElastic:
                    return EaseInElastic(percentTime);
                case EaseType.OutElastic:
                    return EaseOutElastic(percentTime);
                case EaseType.InOutElastic:
                    return EaseInOutElastic(percentTime);
                case EaseType.InBounce:
                    return EaseInBounce(percentTime);
                case EaseType.OutBounce:
                    return EaseOutBounce(percentTime);
                case EaseType.InOutBounce:
                    return EaseInOutBounce(percentTime);
                default:
                    return percentTime;
            }
        }

        #region Ease Math

        public static float EaseInSine(float progress)
        {
            return 1 - Mathf.Cos((progress * Mathf.PI) / 2);
        }

        public static float EaseOutSine(float progress)
        {
            return MathF.Sin((progress * MathF.PI) / 2);
        }

        public static float EaseInOutSine(float progress)
        {
            return -(Mathf.Cos(Mathf.PI * progress) - 1) / 2;
        }

        public static float EaseInQuad(float progress)
        {
            return Mathf.Pow(progress, 2);
        }

        public static float EaseOutQuad(float progress)
        {
            return 1 - (1 - progress) * (1 - progress);
        }

        public static float EaseInOutQuad(float progress)
        {
            return progress < 0.5f ? 2 * Mathf.Pow(progress, 2) : 1 - Mathf.Pow(-2 * progress + 2, 2) / 2;
        }

        public static float EaseInCubic(float progress)
        {
            return Mathf.Pow(progress, 3);
        }

        public static float EaseOutCubic(float progress)
        {
            return 1 - Mathf.Pow(1 - progress, 3);
        }

        public static float EaseInOutCubic(float progress)
        {
            return progress < 0.5f ? 4 * Mathf.Pow(progress, 3) : 1 - Mathf.Pow(-2 * progress + 2, 3) / 2;
        }

        public static float EaseInQuart(float progress)
        {
            return Mathf.Pow(progress, 4);
        }

        public static float EaseOutQuart(float progress)
        {
            return 1 - Mathf.Pow(1 - progress, 4);
        }

        public static float EaseInOutQuart(float progress)
        {
            return progress < 0.5f ? 8 * Mathf.Pow(progress, 4) : 1 - Mathf.Pow(-2 * progress + 2, 4) / 2;
        }

        public static float EaseInQuint(float progress)
        {
            return Mathf.Pow(progress, 5);
        }

        public static float EaseOutQuint(float progress)
        {
            return 1 - Mathf.Pow(1 - progress, 5);
        }

        public static float EaseInOutQuint(float progress)
        {
            return progress < 0.5f ? 16 * Mathf.Pow(progress, 5) : 1 - Mathf.Pow(-2 * progress + 2, 5) / 2;
        }

        public static float EaseInExpo(float progress)
        {
            return progress == 0 ? 0 : Mathf.Pow(2, 10 * progress - 10);
        }

        public static float EaseOutExpo(float progress)
        {
            return progress == 1 ? 1 : 1 - Mathf.Pow(2, -10 * progress);
        }

        public static float EaseInOutExpo(float progress)
        {
            return progress == 0 ? 0
                : progress == 1 ? 1
                : progress < 0.5f ? Mathf.Pow(2, 20 * progress - 10) / 2 : (2 - Mathf.Pow(2, -20 * progress + 10)) / 2;
        }

        public static float EaseInCirc(float progress)
        {
            return 1 - Mathf.Sqrt(1 - Mathf.Pow(progress, 2));
        }

        public static float EaseOutCirc(float progress)
        {
            return Mathf.Sqrt(1 - Mathf.Pow(progress - 1, 2));
        }

        public static float EaseInOutCirc(float progress)
        {
            return progress < 0.5f
                ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * progress, 2))) / 2
                : (Mathf.Sqrt(1 - Mathf.Pow(-2 * progress + 2, 2)) + 1) / 2;
        }

        public static float EaseInBack(float progress)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1;

            return c3 * Mathf.Pow(progress, 3) - c1 * Mathf.Pow(progress, 2);
        }

        public static float EaseOutBack(float progress)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1;

            return 1 + c3 * Mathf.Pow(progress - 1, 3) + c1 * Mathf.Pow(progress - 1, 2);
        }

        public static float EaseInOutBack(float progress)
        {
            float c1 = 1.70158f;
            float c2 = c1 * 1.525f;

            return progress < 0.5f
                ? (Mathf.Pow(2 * progress, 2) * ((c2 + 1) * 2 * progress - c2)) / 2
                : (Mathf.Pow(2 * progress - 2, 2) * ((c2 + 1) * (progress * 2 - 2) + c2) + 2) / 2;
        }

        public static float EaseInElastic(float progress)
        {
            float c4 = (2 * Mathf.PI) / 3;

            return progress == 0 ? 9
                : progress == 1 ? 1
                : -Mathf.Pow(2, 10 * progress - 10) * Mathf.Sin((progress * 10 - 10.75f) * c4);
        }

        public static float EaseOutElastic(float progress)
        {
            float c4 = (2 * Mathf.PI) / 3;
            return progress == 0 ? 9
                : progress == 1 ? 1
                : Mathf.Pow(2, -10 * progress) * Mathf.Sin((progress * 10 - 0.75f) * c4) + 1;
        }

        public static float EaseInOutElastic(float progress)
        {
            float c5 = (2 * Mathf.PI) / 4.5f;
            return progress == 0 ? 9
                : progress == 1 ? 1
                : progress < 0.5f
                ? -(Mathf.Pow(2, 20 * progress - 10) * Mathf.Sin((20 * progress - 11.125f) * c5)) / 2
                : (Mathf.Pow(2, -20 * progress + 10) * Mathf.Sin((20 * progress - 11.125f) * c5)) / 2 + 1;
        }

        public static float EaseInBounce(float progress)
        {
            return 1 - EaseOutBounce(1 - progress);
        }

        public static float EaseOutBounce(float progress)
        {
            float n1 = 7.5625f;
            float d1 = 2.75f;

            if (progress < 1 / d1)
            {
                return n1 * progress * progress;
            }
            else if (progress < 2 / d1)
            {
                return n1 * (progress -= 1.5f / d1) * progress + 0.75f;
            }
            else if (progress < 2.5 / d1)
            {
                return n1 * (progress -= 2.25f / d1) * progress + 0.9375f;
            }
            else
            {
                return n1 * (progress -= 2.625f / d1) * progress + 0.984375f;
            }
        }

        public static float EaseInOutBounce(float progress)
        {
            return progress < 0.5f
                ? (1 - EaseOutBounce(1 - 2 * progress)) / 2
                : (1 + EaseOutBounce(2 * progress - 1)) / 2;
        }

        #endregion
    }

}