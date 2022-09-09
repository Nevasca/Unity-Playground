using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nixtor.AI;

public class EntityDanceHandler : MonoBehaviour, IDanceable
{
    private StateMachine _stateMachine;

    private void Awake()
    {
        _stateMachine = GetComponent<StateMachine>();
    }

    public void Dance(float duration)
    {
        DanceState state = (DanceState)_stateMachine.GetState(AIState.Dancing);

        if (state == null)
        {
            Debug.Log($"{gameObject.name} has no dance state!");
            return;
        }

        state.SetDanceDuration(duration);
        _stateMachine.SwitchToNewState(AIState.Dancing);
    }
}