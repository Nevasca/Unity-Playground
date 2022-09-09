using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tweening
{
    public static class TweenVolume
    {
        #region Extensions
        public static Tween DoWeight(this Volume volume, float endValue, float duration)
        {
            var tween = new Tween(duration);
            tween.CoroutineFunction = WeightCoroutine(tween, volume, endValue);

            Tweener.AddTween(ref tween);
            return tween;
        }
        #endregion

        #region Coroutines
        private static IEnumerator WeightCoroutine(Tween tween, Volume volume, float endWeight)
        {
            float t = 0;
            float startWeight = volume.weight;

            while (t / tween.Duration < 1f && volume != null)
            {
                volume.weight = Mathf.Lerp(startWeight, endWeight, tween.Evaluate(t / tween.Duration));
                t += Tweener.DeltaTime;

                yield return null;
            }

            if (volume == null)
                yield break;

            volume.weight = endWeight;
            tween.InvokeOnCompleted();
        }
        #endregion
    }
}