using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Nixtor.AI
{
    [CreateAssetMenu(fileName = "Patrol State", menuName = "AI/Patrol State")]
    public class PatrolState : BaseState
    {
        [SerializeField] private float patrolSpeed = 1f;

        private RegionPatrolPoints _regionPoints;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private Transform _currentPoint;
        //private int _currentPoint = -1;

        public override AIState GetStateType()
        {
            return AIState.Patrolling;
        }

        public override void Init()
        {
            _regionPoints = gameObject.GetComponentInParent<RegionPatrolPoints>();
            _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
            _animator = gameObject.GetComponent<Animator>();
        }

        public override void Start()
        {
            //_currentPoint = ++_currentPoint % _regionPoints.points.Length;
            _currentPoint = _regionPoints.GetRandomPoint();
            _navMeshAgent.stoppingDistance = 0.3f;
            _navMeshAgent.speed = patrolSpeed;
            _navMeshAgent.enabled = true;
        }

        public override void Stop()
        {
            _regionPoints.AddPoint(_currentPoint);
        }

        public override AIState Tick()
        {
            if (_currentPoint == null)
                return AIState.Resting;

            _navMeshAgent.SetDestination(_currentPoint.position);

            if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= 0f)
                return AIState.Resting;

            _animator.SetBool("isWalking", true);
            return AIState.Patrolling;
        }
    }
}