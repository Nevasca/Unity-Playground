using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tweening
{
    public static class TweenAudio
    {
        public static Tween DoVolume(this AudioSource source, float endValue, float duration)
        {
            var tween = new Tween(duration);
            tween.CoroutineFunction = VolumeCoroutine(tween, source, endValue);

            Tweener.AddTween(ref tween);
            return tween;
        }

        private static IEnumerator VolumeCoroutine(Tween tween, AudioSource source, float endValue)
        {
            float t = 0;
            float startValue = source.volume;

            while(t / tween.Duration < 1f && source != null)
            {
                source.volume = Mathf.Lerp(startValue, endValue, tween.Evaluate(t / tween.Duration));
                t += Tweener.DeltaTime;

                yield return null;
            }

            if (source == null)
                yield break;

            source.volume = endValue;
            tween.InvokeOnCompleted();
        }
    }
}