using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tweening
{
    public static class TweenImage
    {
        #region Extensions
        public static Tween DoFade(this Image image, float endValue, float duration)
        {
            var tween = new Tween(duration);
            tween.CoroutineFunction = FadeCoroutine(tween, image, endValue);

            Tweener.AddTween(ref tween);
            return tween;
        }
        #endregion

        #region Coroutines
        private static IEnumerator FadeCoroutine(Tween tween, Image image, float endAlpha)
        {
            float t = 0;
            Color startColor = image.color;
            Color endColor = image.color;
            endColor.a = endAlpha;


            while (t / tween.Duration < 1f && image != null)
            {
                image.color = Color.Lerp(startColor, endColor, tween.Evaluate(t / tween.Duration));
                t += Tweener.DeltaTime;

                yield return null;
            }

            if (image == null)
                yield break;

            image.color = endColor;
            tween.InvokeOnCompleted();
        }
        #endregion
    }
}