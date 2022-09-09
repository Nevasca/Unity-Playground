using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Tweening
{

    //Ease functions based on https://easings.net/ by Andrey Sitnik and Ivan Solovev

    public class TweenEase
    {
        public delegate float EaseFunction(float x);

        private const float C1 = 1.70158f;
        private const float C2 = C1 * 1.525f;
        private const float C3 = 2.70158f;
        private const float C4 = 2f * Mathf.PI / 3f;
        private const float C5 = 2f * Mathf.PI / 4.5f;

        private const float N1 = 7.5625f;
        private const float D1 = 2.75f;

        public static EaseFunction GetEaseFunction(Ease ease)
        {
            return ease switch
            {
                Ease.Linear => EvaluateLinear,
                Ease.InSine => EvaluateInSine,
                Ease.OutSine => EvaluateOutSine,
                Ease.InOutSine => EvaluateInOutSine,
                Ease.InCubic => EvaluateInCubic,
                Ease.OutCubic => EvaluateOutCubic,
                Ease.InOutCubic => EvaluateInOutCubic,
                Ease.InQuint => EvaluateInQuint,
                Ease.OutQuint => EvaluateOutQuint,
                Ease.InOutQuint => EvaluateInOutQuint,
                Ease.InCirc => EvaluateInCirc,
                Ease.OutCirc => EvaluateOutCirc,
                Ease.InOutCirc => EvaluateInOutCirc,
                Ease.InElastic => EvaluateInElastic,
                Ease.OutElastic => EvaluateOutElastic,
                Ease.InOutElastic => EvaluateInOutElastic,
                Ease.InQuad => EvaluateInQuad,
                Ease.OutQuad => EvaluateOutQuad,
                Ease.InOutQuad => EvaluateInOutQuad,
                Ease.InQuart => EvaluateInQuart,
                Ease.OutQuart => EvaluateOutQuart,
                Ease.InOutQuart => EvaluateInOutQuart,
                Ease.InExpo => EvaluateInExpo,
                Ease.OutExpo => EvaluateOutExpo,
                Ease.InOutExpo => EvaluateInOutExpo,
                Ease.InBack => EvaluateInBack,
                Ease.OutBack => EvaluateOutBack,
                Ease.InOutBack => EvaluateInOutBack,
                Ease.InBounce => EvaluateInBounce,
                Ease.OutBounce => EvaluateOutBounce,
                Ease.InOutBounce => EvaluateInOutBounce,
                _ => EvaluateOutQuad,
            };
        }

        #region Ease Functions
        public static float EvaluateLinear(float x)
        {
            return x;
        }

        public static float EvaluateInSine(float x)
        {
            return 1f - Mathf.Cos(x * Mathf.PI / 2f);
        }

        public static float EvaluateOutSine(float x)
        {
            return Mathf.Sin(x * Mathf.PI / 2f);
        }

        public static float EvaluateInOutSine(float x)
        {
            return -(Mathf.Cos(Mathf.PI * x) - 1f) / 2f;
        }

        public static float EvaluateInCubic(float x)
        {
            return x * x * x;
        }

        public static float EvaluateOutCubic(float x)
        {
            return 1f - Mathf.Pow(1f - x, 3f);
        }

        public static float EvaluateInOutCubic(float x)
        {
            return x < 0.5f ? 4f * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;
        }

        public static float EvaluateInQuint(float x)
        {
            return x * x * x * x * x;
        }

        public static float EvaluateOutQuint(float x)
        {
            return 1f - Mathf.Pow(1f - x, 5f);
        }

        public static float EvaluateInOutQuint(float x)
        {
            return x < 0.5f ? 16f * x * x * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 5f) / 2f;
        }

        public static float EvaluateInCirc(float x)
        {
            return 1f - Mathf.Sqrt(1f - Mathf.Pow(x, 2f));
        }

        public static float EvaluateOutCirc(float x)
        {
            return Mathf.Sqrt(1f - Mathf.Pow(x - 1f, 2f));
        }

        public static float EvaluateInOutCirc(float x)
        {
            return x < 0.5f
              ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * x, 2f))) / 2f
              : (Mathf.Sqrt(1f - Mathf.Pow(-2f * x + 2f, 2f)) + 1f) / 2f;
        }

        public static float EvaluateInElastic(float x)
        {
            return x == 0f
              ? 0f
              : x == 1f
              ? 1f
              : -Mathf.Pow(2f, 10f * x - 10f) * Mathf.Sin((x * 10f - 10.75f) * C4);
        }

        public static float EvaluateOutElastic(float x)
        {
            return x == 0f
              ? 0f
              : x == 1f
              ? 1f
              : Mathf.Pow(2f, -10f * x) * Mathf.Sin((x * 10f - 0.75f) * C4) + 1f;
        }

        public static float EvaluateInOutElastic(float x)
        {
            return x == 0f
              ? 0f
              : x == 1f
              ? 1f
              : x < 0.5f
              ? -(Mathf.Pow(2f, 20f * x - 10f) * Mathf.Sin((20f * x - 11.125f) * C5)) / 2f
              : (Mathf.Pow(2f, -20f * x + 10f) * Mathf.Sin((20f * x - 11.125f) * C5)) / 2f + 1f;
        }

        public static float EvaluateInQuad(float x)
        {
            return x * x;
        }

        public static float EvaluateOutQuad(float x)
        {
            return 1f - (1f - x) * (1f - x);
        }

        public static float EvaluateInOutQuad(float x)
        {
            return x < 0.5f ? 2f * x * x : 1f - Mathf.Pow(-2f * x + 2f, 2f) / 2f;
        }

        public static float EvaluateInQuart(float x)
        {
            return x * x * x * x;
        }

        public static float EvaluateOutQuart(float x)
        {
            return 1f - Mathf.Pow(1f - x, 4f);
        }

        public static float EvaluateInOutQuart(float x)
        {
            return x < 0.5f ? 8f * x * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 4f) / 2f;
        }

        public static float EvaluateInExpo(float x)
        {
            return x == 0f ? 0f : Mathf.Pow(2f, 10f * x - 10f);
        }

        public static float EvaluateOutExpo(float x)
        {
            return x == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * x);
        }

        public static float EvaluateInOutExpo(float x)
        {
            return x == 0f
              ? 0f
              : x == 1f
              ? 1f
              : x < 0.5f ? Mathf.Pow(2f, 20f * x - 10f) / 2f
              : (2f - Mathf.Pow(2f, -20f * x + 10f)) / 2f;
        }

        public static float EvaluateInBack(float x)
        {
            return C3 * x * x * x - C1 * x * x;
        }

        public static float EvaluateOutBack(float x)
        {
            return 1f + C3 * Mathf.Pow(x - 1f, 3f) + C1 * Mathf.Pow(x - 1f, 2f);
        }

        public static float EvaluateInOutBack(float x)
        {
            return x < 0.5f
              ? (Mathf.Pow(2f * x, 2f) * ((C2 + 1f) * 2f * x - C2)) / 2f
              : (Mathf.Pow(2f * x - 2f, 2f) * ((C2 + 1f) * (x * 2f - 2f) + C2) + 2f) / 2f;
        }

        public static float EvaluateInBounce(float x)
        {
            return 1f - EvaluateOutBounce(1f - x);
        }

        public static float EvaluateOutBounce(float x)
        {
            if (x < 1f / D1)
            {
                return N1 * x * x;
            }
            else if (x < 2f / D1)
            {
                return N1 * (x -= 1.5f / D1) * x + 0.75f;
            }
            else if (x < 2.5 / D1)
            {
                return N1 * (x -= 2.25f / D1) * x + 0.9375f;
            }
            else
            {
                return N1 * (x -= 2.625f / D1) * x + 0.984375f;
            }
        }

        public static float EvaluateInOutBounce(float x)
        {
            return x < 0.5f
                ? (1f - EvaluateOutBounce(1f - 2f * x)) / 2f
                : (1f + EvaluateOutBounce(2f * x - 1f)) / 2f;
        }
        #endregion
    }
}