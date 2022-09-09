using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nixtor.AI;

public class PigeonInstance : SkillInstance
{
    [SerializeField] private GameObject soulercoaster;

    public override void Init(Vector3 direction, Transform author)
    {
        if(author.TryGetComponent(out Health authorHealth))
            GetComponent<Health>().SetCategory(authorHealth.GetCategory());

        FollowState state = (FollowState) GetComponent<StateMachine>().GetState(AIState.Following);
        state.SetFollowTarget(author);

        Instantiate(soulercoaster, transform.position, soulercoaster.transform.rotation);
        SoundManager.Instance.PlaySFXAt(25, transform.position, pitchRange: 0.05f);
    }
}