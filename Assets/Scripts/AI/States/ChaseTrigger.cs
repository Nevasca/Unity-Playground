using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nixtor.AI
{
    public class ChaseTrigger : MonoBehaviour
    {
        [SerializeField] private float sightMaxRange = -0.15f;
        [SerializeField] private GameObject vfxAlert;

        private AICategory _category;
        private Vector3 _lookDir;
        private Transform _inRange;
        private bool _chaseStarted;

        private void Start()
        {
            _category = GetComponentInParent<IDamageable>().GetCategory();
        }

        private void Update()
        {
            if (_chaseStarted || _inRange == null)
                return;

            _lookDir = _inRange.position - transform.position;
            if (Vector3.Dot(_lookDir, transform.forward) < sightMaxRange)
                return;

            SetToChase();
        }

        private void SetToChase()
        {
            _chaseStarted = true;

            if(vfxAlert != null)
            {
                vfxAlert.SetActive(true);
                SoundManager.Instance.PlaySFXAt(36, transform.position, minDistance: 2f);
            }

            StateMachine stateMachine = GetComponentInParent<StateMachine>();
            ChaseState chaseState = stateMachine.GetState(AIState.Chasing) as ChaseState;
            chaseState.SetChaseTarget(_inRange.transform);
            stateMachine.SwitchToNewState(AIState.Chasing);
        }

        public void SetToChase(Transform target, IDamageable damageable)
        {
            if (damageable.GetCategory() == _category)
                return;

            _inRange = target;
            _chaseStarted = false;
            SetToChase();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_chaseStarted)
                return;

            if(other.TryGetComponent(out IDamageable damageableInrange))
            {
                if (damageableInrange.GetCategory() == _category)
                    return;

                _inRange = other.transform;
                _chaseStarted = false;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_chaseStarted)
                return;

            if(other.TryGetComponent(out IDamageable damageableOutOfRange))
            {
                if (damageableOutOfRange.GetCategory() == _category)
                    return;

                _inRange = null;
                StateMachine stateMachine = GetComponentInParent<StateMachine>();
                ChaseState chaseState = stateMachine.GetState(AIState.Chasing) as ChaseState;
                if (chaseState.GetChaseTarget() == other.transform)
                    chaseState.SetChaseTarget(null);
            }
        }
    }
}