using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nixtor.AI
{
    [CreateAssetMenu(fileName = "Rest State", menuName = "AI/Rest State")]
    public class RestState : BaseState
    {
        [SerializeField] private float restTime = 2f;

        private float _timer;
        private Animator _animator;

        public override AIState GetStateType()
        {
            return AIState.Resting;
        }

        public override void Init()
        {
            _animator = gameObject.GetComponent<Animator>();
        }

        public override void Start()
        {
            _animator.SetBool("isWalking", false);
            _timer = 0f;
        }

        public override void Stop()
        {

        }

        public override AIState Tick()
        {
            _timer += Time.deltaTime;
            if (_timer >= restTime)
                return AIState.Patrolling;

            return AIState.Resting;
        }
    }
}