using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Nixtor.AI
{
    [CreateAssetMenu(fileName = "New Chase State", menuName = "AI/Chase State")]
    public class ChaseState : BaseState
    {
        [SerializeField] private float reachDistance = 1.5f;
        [SerializeField] private float chaseSpeed = 2.5f;
        [SerializeField] private string _animationRunning = "isRunning";

        private Transform _chaseTarget;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;

        public override AIState GetStateType()
        {
            return AIState.Chasing;
        }

        public override void Init()
        {
            _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
            _animator = gameObject.GetComponent<Animator>();
        }

        public override void Start()
        {
            _animator.SetBool("isWalking", false);
            _navMeshAgent.stoppingDistance = reachDistance;
            _navMeshAgent.speed = chaseSpeed;
            _navMeshAgent.enabled = true;
        }

        public override void Stop()
        {

        }

        public override AIState Tick()
        {
            if (_chaseTarget == null)
                return AIState.Default;

            _navMeshAgent.SetDestination(_chaseTarget.position);

            if(!_navMeshAgent.pathPending && 
                _navMeshAgent.remainingDistance < _navMeshAgent.stoppingDistance)
            {
                //_animator.SetBool("isRunning", false);
                return AIState.Attacking;
            }

            //_animator.SetBool("isRunning", _navMeshAgent.velocity.sqrMagnitude > 0.1f);
            _animator.SetBool(_animationRunning, true);

            return AIState.Chasing;
        }

        public void SetChaseTarget(Transform chaseTarget)
        {
            _chaseTarget = chaseTarget;
        }

        public Transform GetChaseTarget()
        {
            return _chaseTarget;
        }
    }
}
