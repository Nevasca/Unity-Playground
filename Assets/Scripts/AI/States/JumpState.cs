using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tweening;

namespace Nixtor.AI
{
    [CreateAssetMenu(fileName = "New Jump State", menuName = "AI/Jump State")]
    public class JumpState : BaseState
    {
        [SerializeField] private float jumpPower = 3f;

        private float _timer = 2f;
        private Transform _transform;

        public override AIState GetStateType()
        {
            return AIState.Jumping;
        }

        public override void Init()
        {
            _transform = gameObject.transform;
        }

        public override void Start()
        {
            _timer = 2f;
        }

        public override void Stop()
        {
            
        }

        public override AIState Tick()
        {
            _timer += Time.deltaTime;

            if (_timer > 2f)
            {
                TweenSequence sequence = Tweener.Sequence();
                sequence.Append(gameObject.transform.DoMoveY(jumpPower, 0.4f).SetEase(Ease.Linear));
                sequence.Append(gameObject.transform.DoMoveY(-jumpPower, 0.3f).SetEase(Ease.InSine));
                sequence.Play();

                _timer = 0f;
            }

            return AIState.Jumping;
        }
    }
}
