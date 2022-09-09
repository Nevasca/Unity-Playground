using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Tweening
{
    public class TweenSequence
    {
        private Queue<Tween> _tweens = new Queue<Tween>();
        private List<Tween> _playingTweens;

        private int _current = -1;
        private int _playing = 0;

        public TweenSequence Append(Tween tween)
        {
            _current++;

            SetSequence(ref tween);
            _tweens.Enqueue(tween);

            return this;
        }

        public TweenSequence Join(Tween tween)
        {
            SetSequence(ref tween);
            _tweens.Enqueue(tween);

            return this;
        }

        public TweenSequence AppendCallback(Action callback)
        {
            _current++;

            var tween = new Tween(0f);
            SetSequence(ref tween);

            tween.OnComplete(callback);

            _tweens.Enqueue(tween);

            return this;
        }

        public TweenSequence AppendInterval(float duration)
        {
            _current++;

            var tween = new Tween(duration);
            SetSequence(ref tween);

            tween.CoroutineFunction = WaitCoroutine(tween);

            _tweens.Enqueue(tween);

            return this;
        }

        private void SetSequence(ref Tween tween)
        {
            tween.HasSequence = true;
            tween.SequenceOrder = _current;
            tween.OnCompleted += Next;
        }

        public void Play()
        {
            _current = -1;
            _playing = 1;
            _playingTweens = new List<Tween>();
            Next();
        }

        public void Kill()
        {
            if (_playingTweens == null)
                return;

            foreach(var tween in _playingTweens)
                tween.Kill();

            _tweens.Clear();
        }

        private void Next()
        {
            _playing--;

            if (_playing != 0)
                return;

            _current++;
            _playingTweens.Clear();

            while (_tweens.Count > 0 && _tweens.Peek().SequenceOrder == _current)
            {
                _playing++;
                Tween tween = _tweens.Dequeue();

                Tweener.StartTween(ref tween);
                _playingTweens.Add(tween);
            }
        }

        private IEnumerator WaitCoroutine(Tween tween)
        {
            yield return new WaitForSecondsRealtime(tween.Duration);

            tween.InvokeOnCompleted();
        }
    }
}