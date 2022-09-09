using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nixtor.AI
{
    [CreateAssetMenu(fileName = "New Inactive State", menuName = "AI/Inactive State")]
    public class InactiveState : BaseState
    {
        public override AIState GetStateType()
        {
            return AIState.Inactive;
        }

        public override void Init() { }

        public override void Start() { }

        public override void Stop() { }

        public override AIState Tick()
        {
            return AIState.Inactive;
        }
    }
}