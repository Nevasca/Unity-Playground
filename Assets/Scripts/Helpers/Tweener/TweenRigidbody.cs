using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tweening
{
    public static class TweenRigidbody
    {
        private readonly static WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

        public static Tween DoMove(this Rigidbody rigidbody, Vector3 endValue, float duration)
        {
            var tween = new Tween(duration);
            tween.CoroutineFunction = MoveCoroutine(tween, rigidbody, endValue);

            Tweener.AddTween(ref tween);
            return tween;
        }

        private static IEnumerator MoveCoroutine(Tween tween, Rigidbody rigidbody, Vector3 endPosition)
        {
            float t = 0;
            Vector3 startPosition = rigidbody.position;

            while (t / tween.Duration < 1f && rigidbody != null)
            {
                rigidbody.position = Vector3.LerpUnclamped(startPosition, endPosition, tween.Evaluate(t / tween.Duration));
                t += Tweener.FixedDeltaTime;

                yield return _waitForFixedUpdate;
            }

            if (rigidbody == null)
                yield break;

            rigidbody.position = endPosition;
            tween.InvokeOnCompleted();
        }
    }
}