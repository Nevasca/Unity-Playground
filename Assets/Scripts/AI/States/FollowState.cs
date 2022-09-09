using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Nixtor.AI
{
    [CreateAssetMenu(fileName = "New Follow State", menuName = "AI/Follow State")]
    public class FollowState : BaseState
    {
        [SerializeField] private float followDistance = 1.5f;
        [SerializeField] private float followSpeed = 2f;

        private Transform _followTarget;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;

        public override AIState GetStateType()
        {
            return AIState.Following;
        }

        public override void Init()
        {
            _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
            _animator = gameObject.GetComponent<Animator>();
        }

        public override void Start()
        {
            _navMeshAgent.stoppingDistance = followDistance;
            _navMeshAgent.speed = followSpeed;
            _navMeshAgent.enabled = true;
        }

        public override void Stop()
        {

        }

        public override AIState Tick()
        {
            if (_followTarget == null)
                return AIState.Following;

            _navMeshAgent.SetDestination(_followTarget.position);
            _animator.SetBool("isWalking", _navMeshAgent.velocity.sqrMagnitude > 0.1f);

            return AIState.Following;
        }

        public void SetFollowTarget(Transform followTarget)
        {
            _followTarget = followTarget;
        }
    }
}