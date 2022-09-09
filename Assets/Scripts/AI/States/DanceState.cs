using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nixtor.AI
{
    [CreateAssetMenu(fileName = "New Dance State", menuName = "AI/Dance State")]
    public class DanceState : BaseState
    {
        [SerializeField] private string _animationDance = "isDancing";

        public override AIState GetStateType() => AIState.Dancing;

        private Animator _animator;
        private float _danceDuration;
        private float _timer;

        public override void Init()
        {
            _animator = gameObject.GetComponent<Animator>();
        }
        public override void Start()
        {
            _timer = 0f;
            _animator.SetBool(_animationDance, true);
        }

        public override void Stop()
        {
            _animator.SetBool(_animationDance, false);
        }

        public void SetDanceDuration(float duration)
        {
            _danceDuration = duration;
        }

        public override AIState Tick()
        {
            _timer += Time.deltaTime;

            if (_timer < _danceDuration)
                return AIState.Dancing;

            return AIState.Default;
        }
    }
}