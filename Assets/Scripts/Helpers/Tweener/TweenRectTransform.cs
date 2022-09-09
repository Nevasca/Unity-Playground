using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tweening
{
    public static class TweenRectTransform
    {
        public static Tween DoAnchorPosX(this RectTransform transform, float endValue, float duration)
        {
            var tween = new Tween(duration);
            tween.CoroutineFunction = AnchorPosXCoroutine(tween, transform, endValue);

            Tweener.AddTween(ref tween);
            return tween;
        }

        public static Tween DoAnchorPosY(this RectTransform transform, float endValue, float duration)
        {
            var tween = new Tween(duration);
            tween.CoroutineFunction = AnchorPosYCoroutine(tween, transform, endValue);

            Tweener.AddTween(ref tween);
            return tween;
        }

        private static IEnumerator AnchorPosXCoroutine(Tween tween, RectTransform transform, float endValue)
        {
            float t = 0;
            Vector2 startPosition = transform.anchoredPosition;
            Vector2 endPosition = new Vector2(endValue, startPosition.y);

            while (t / tween.Duration < 1f && transform != null)
            {
                transform.anchoredPosition = Vector3.LerpUnclamped(startPosition, endPosition, tween.Evaluate(t / tween.Duration));
                t += Tweener.DeltaTime;

                yield return null;
            }

            if (transform == null)
                yield break;

            transform.anchoredPosition = endPosition;
            tween.InvokeOnCompleted();
        }

        private static IEnumerator AnchorPosYCoroutine(Tween tween, RectTransform transform, float endValue)
        {
            float t = 0;
            Vector2 startPosition = transform.anchoredPosition;
            Vector2 endPosition = new Vector2(startPosition.x, endValue);

            while (t / tween.Duration < 1f && transform != null)
            {
                transform.anchoredPosition = Vector3.LerpUnclamped(startPosition, endPosition, tween.Evaluate(t / tween.Duration));
                t += Tweener.DeltaTime;

                yield return null;
            }

            if (transform == null)
                yield break;

            transform.anchoredPosition = endPosition;
            tween.InvokeOnCompleted();
        }
    }
}