using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Tweening
{
    public class Tween
    {
        public event Action OnCompleted;

        public IEnumerator CoroutineFunction { get; set; }
        public Coroutine StartedCoroutine { get; set; }
        public float Duration { get; private set; }
        public bool HasSequence { get; set; }
        public int SequenceOrder { get; set; }
        public TweenEase.EaseFunction Evaluate { get; private set; }
        public int Loops { get; private set; }
        public int CurrentLoops { get; set; }

        public Tween(float duration)
        {
            Duration = duration;
            Evaluate = Tweener.DefaultEaseFunction;
        }

        public Tween OnComplete(Action callback)
        {
            OnCompleted += callback;
            return this;
        }

        public Tween SetEase(Ease ease)
        {
            Evaluate = TweenEase.GetEaseFunction(ease);
            return this;
        }

        public void InvokeOnCompleted()
        {
            OnCompleted?.Invoke();
        }

        public void Kill()
        {
            Tweener.Kill(this);
        }
    }
}