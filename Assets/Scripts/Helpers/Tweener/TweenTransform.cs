using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:
//DoJump
//DoShakePosition
//DoPunchPosition

namespace Tweening
{
    public static class TweenTransform
    {
        #region Extensions
        public static Tween DoScale(this Transform transform, float endValue, float duration)
        {
            var tween = new Tween(duration);
            tween.CoroutineFunction = ScaleCoroutine(tween, transform, Vector3.one * endValue);

            Tweener.AddTween(ref tween);
            return tween;
        }

        public static Tween DoScale(this Transform transform, Vector3 endValue, float duration)
        {
            var tween = new Tween(duration);
            tween.CoroutineFunction = ScaleCoroutine(tween, transform, endValue);

            Tweener.AddTween(ref tween);
            return tween;
        }

        public static Tween DoPunchScale(this Transform transform, Vector3 endValue, float duration)
        {
            var tween = new Tween(duration);
            tween.CoroutineFunction = PunchScaleCoroutine(tween, transform, endValue);

            Tweener.AddTween(ref tween);
            return tween;
        }

        public static Tween DoMove(this Transform transform, Vector3 endValue, float duration)
        {
            var tween = new Tween(duration);
            tween.CoroutineFunction = MoveCoroutine(tween, transform, endValue);

            Tweener.AddTween(ref tween);
            return tween;
        }

        public static Tween DoMoveY(this Transform transform, float endValue, float duration)
        {
            var tween = new Tween(duration);
            tween.CoroutineFunction = MoveYCoroutine(tween, transform, endValue);

            Tweener.AddTween(ref tween);
            return tween;
        }

        public static Tween DoRotate(this Transform transform, Vector3 endValue, float duration)
        {
            var tween = new Tween(duration);
            tween.CoroutineFunction = RotateCoroutine(tween, transform, endValue);

            Tweener.AddTween(ref tween);
            return tween;
        }
        #endregion

        #region Coroutines
        private static IEnumerator MoveCoroutine(Tween tween, Transform transform, Vector3 endPosition)
        {
            float t = 0;
            Vector3 startPosition = transform.position;

            while (t / tween.Duration < 1f && transform != null)
            {
                transform.position = Vector3.LerpUnclamped(startPosition, endPosition, tween.Evaluate(t / tween.Duration));
                t += Tweener.DeltaTime;

                yield return null;
            }

            if (transform == null)
                yield break;

            transform.position = endPosition;
            tween.InvokeOnCompleted();
        }

        private static IEnumerator MoveYCoroutine(Tween tween, Transform transform, float endValue)
        {
            float t = 0;
            Vector3 startPosition = transform.position;
            Vector3 endPosition = new Vector3(startPosition.x, endValue, startPosition.z);

            while (t / tween.Duration < 1f && transform != null)
            {
                transform.position = Vector3.LerpUnclamped(startPosition, endPosition, tween.Evaluate(t / tween.Duration));
                t += Tweener.DeltaTime;

                yield return null;
            }

            if (transform == null)
                yield break;

            transform.position = endPosition;
            tween.InvokeOnCompleted();
        }

        private static IEnumerator ScaleCoroutine(Tween tween, Transform transform, Vector3 endValue)
        {
            float t = 0;
            Vector3 startScale = transform.localScale;

            while (t / tween.Duration < 1f && transform != null)
            {
                transform.localScale = Vector3.LerpUnclamped(startScale, endValue, tween.Evaluate(t / tween.Duration));
                t += Tweener.DeltaTime;

                yield return null;
            }

            if (transform == null)
                yield break;

            transform.localScale = endValue;
            tween.InvokeOnCompleted();
        }

        private static IEnumerator PunchScaleCoroutine(Tween tween, Transform transform, Vector3 endValue)
        {
            float t = 0;
            Vector3 startScale = transform.localScale;
            float duration = tween.Duration / 2f;

            while (t / duration < 1f && transform != null)
            {
                transform.localScale = Vector3.LerpUnclamped(startScale, endValue, tween.Evaluate(t / tween.Duration));
                t += Tweener.DeltaTime;

                yield return null;
            }

            transform.localScale = endValue;
            t = 0f;

            while (t / duration < 1f && transform != null)
            {
                transform.localScale = Vector3.LerpUnclamped(endValue, startScale, tween.Evaluate(t / tween.Duration));
                t += Tweener.DeltaTime;

                yield return null;
            }

            if (transform == null)
                yield break;

            transform.localScale = startScale;
            tween.InvokeOnCompleted();
        }

        private static IEnumerator RotateCoroutine(Tween tween, Transform transform, Vector3 endValue)
        {
            float t = 0;
            Quaternion startEulerRotation = transform.rotation;
            Quaternion endEulerRotation = Quaternion.Euler(endValue);

            while (t / tween.Duration < 1f && transform != null)
            {
                transform.rotation = Quaternion.SlerpUnclamped(startEulerRotation, endEulerRotation, tween.Evaluate(t / tween.Duration));
                t += Tweener.DeltaTime;

                yield return null;
            }

            if (transform == null)
                yield break;

            transform.rotation = endEulerRotation;
            tween.InvokeOnCompleted();
        }
        #endregion
    }
}