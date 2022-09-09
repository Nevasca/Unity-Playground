using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nixtor.AI;

public class SkillEventsHandler : MonoBehaviour
{
    [SerializeField] private Transform skillOrigin;

    private AttackState _attackState;

    public void Start()
    {
        _attackState = GetComponent<StateMachine>().GetState(AIState.Attacking) as AttackState;
    }

    public Transform GetSkillOrigin()
    {
        return skillOrigin;
    }

    //Animation event
    private void SkillCast()
    {
        if(gameObject.activeInHierarchy)
            _attackState.Skill.EventSkillCast();
    }
}