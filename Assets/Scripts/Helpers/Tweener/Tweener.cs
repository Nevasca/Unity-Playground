using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//	From()
// Loops

namespace Tweening
{
    public class Tweener : MonoBehaviour
    {
        public static float DeltaTime => Time.unscaledDeltaTime;
        public static float FixedDeltaTime => Time.fixedUnscaledDeltaTime;
        public static TweenEase.EaseFunction DefaultEaseFunction => TweenEase.EvaluateOutQuad;


        private static Tweener _instance;

        private static Queue<Tween> _waitingTweens = new Queue<Tween>();

        #region Static Methods
        private static Tweener SetInstance()
        {
            if (_instance != null)
                return _instance;

            var go = new GameObject("[Tweener]");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<Tweener>();
            return _instance;
        }

        public static void StartTween(ref Tween tween)
        {
            if(_instance == null)
                SetInstance();

            if(tween.CoroutineFunction == null)
            {
                tween.InvokeOnCompleted();
                return;
            }

            tween.StartedCoroutine = _instance.StartCoroutine(tween.CoroutineFunction);
        }

        public static void AddTween(ref Tween tween)
        {
            if (_instance == null)
                SetInstance();

            _waitingTweens.Enqueue(tween);
        }

        public static TweenSequence Sequence()
        {
            return new TweenSequence();
        }

        public static void KillAll()
        {
            if (_instance == null)
                return;

            _instance.StopAllCoroutines();
        }

        public static void Kill(Tween tween)
        {
            if (tween == null)
                return;

            if (tween.StartedCoroutine == null)
                return;

            _instance.StopCoroutine(tween.StartedCoroutine);
        }
        #endregion

        public void LateUpdate()
        {
            DoWaitingTweens();
        }

        private void DoWaitingTweens()
        {
            if (_waitingTweens.Count == 0)
                return;

            Tween tween;

            for(int i = 0; i < _waitingTweens.Count; i++)
            {
                tween = _waitingTweens.Dequeue();

                if (tween.HasSequence)
                    continue;

                StartTween(ref tween);
            }
        }

        private void OnApplicationQuit()
        {
            _instance = null;
        }
    }
}