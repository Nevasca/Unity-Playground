using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tweening
{
    public static class TweenCanvasGroup
    {
        #region Extensions
        public static Tween DoFade(this CanvasGroup canvasGroup, float endValue, float duration)
        {
            var tween = new Tween(duration);
            tween.CoroutineFunction = FadeCoroutine(tween, canvasGroup, endValue);

            Tweener.AddTween(ref tween);
            return tween;
        }
        #endregion

        #region Coroutines
        private static IEnumerator FadeCoroutine(Tween tween, CanvasGroup canvasGroup, float endAlpha)
        {
            float t = 0;
            float startAlpha = canvasGroup.alpha;

            while (t / tween.Duration < 1f && canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, tween.Evaluate(t / tween.Duration));
                t += Tweener.DeltaTime;

                yield return null;
            }

            if(canvasGroup == null)
                yield break;

            canvasGroup.alpha = endAlpha;
            tween.InvokeOnCompleted();
        }
        #endregion
    }
}