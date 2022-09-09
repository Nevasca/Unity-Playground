using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nixtor.AI
{
    [CreateAssetMenu(fileName = "New Dead State", menuName = "AI/Dead State")]
    public class DeadState : BaseState
    {
        public const string ANIM_DEAD = "isDead";

        public override AIState GetStateType()
        {
            return AIState.Dead;
        }

        public override void Init() { }

        public override void Start()
        {
            SetDead(true);
        }

        public override void Stop()
        {
            SetDead(false);
        }

        private void SetDead(bool dead)
        {
            gameObject.GetComponent<Animator>()?.SetBool(ANIM_DEAD, dead);
            gameObject.GetComponent<Collider>().enabled = !dead;
            if (gameObject.TryGetComponent(out LookAtNearby lookAtNearby))
                lookAtNearby.Active = !dead;
        }

        public override AIState Tick()
        {
            return AIState.Dead;
        }
    }
}