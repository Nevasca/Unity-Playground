using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Nixtor.AI
{
    [CreateAssetMenu(fileName = "New Attack State", menuName = "AI/Attack State")]
    public class AttackState : BaseState
    {
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float rotationSpeed = 20f;
        [SerializeField] private Skill skill;

        private ChaseState _chaseState;
        private Transform _target;
        private float _attackRangeSqr;
        private Vector3 _directionToTarget;
        private NavMeshAgent _navMeshAgent;

        public Skill Skill { get { return skill; } }

        public override AIState GetStateType()
        {
            return AIState.Attacking;
        }

        public override void Init()
        {
            _chaseState = gameObject.GetComponent<StateMachine>().
                            GetState(AIState.Chasing) as ChaseState;

            _attackRangeSqr = attackRange * attackRange;
            _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

            skill = Instantiate(skill, gameObject.transform);
            skill.Init(gameObject.GetComponent<SkillEventsHandler>().GetSkillOrigin());
        }

        public override void Start()
        {
            _navMeshAgent.enabled = false;
            skill.gameObject.SetActive(true);            
        }

        public override void Stop()
        {
            //skill.gameObject.SetActive(false);
            //Needs to keep skill active in case it receives animation events for coroutines
        }

        public override AIState Tick()
        {
            _target = _chaseState.GetChaseTarget();

            if (_target == null)
                return AIState.Default;

            _directionToTarget = _target.position - gameObject.transform.position;
            gameObject.transform.rotation = 
                Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(_directionToTarget), rotationSpeed * Time.deltaTime);

            if (_directionToTarget.sqrMagnitude > _attackRangeSqr)
                return AIState.Chasing;

            skill.Use(true);

            return AIState.Attacking;
        }
    }
}