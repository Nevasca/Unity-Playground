using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Tweening
{
    public class TweenVirtual
    {
        #region Extensions
        public static Tween DoFloat(float startValue, float endValue, float duration, Action<float> update)
        {
            var tween = new Tween(duration);
            tween.CoroutineFunction = VirtualFloatCoroutine(tween, startValue, endValue, update);

            Tweener.AddTween(ref tween);
            return tween;
        }
        #endregion

        #region Coroutines
        private static IEnumerator VirtualFloatCoroutine(Tween tween, float startValue, float endValue, Action<float> update)
        {
            float t = 0;

            while (t / tween.Duration < 1f)
            {
                update(Mathf.Lerp(startValue, endValue, tween.Evaluate(t / tween.Duration)));
                t += Tweener.DeltaTime;

                yield return null;
            }

            update(endValue);
            tween.InvokeOnCompleted();
        }
        #endregion
    }
}