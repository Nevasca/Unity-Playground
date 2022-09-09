using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nixtor.AI;

public class PlayerClone : SkillInstance
{
    [SerializeField] private float cloneDuration = 5f;
    [SerializeField] private GameObject smokeVFX;

    private bool _init;
    private float _timer;

    public bool IsClone { get { return _init; } }

    private void Update()
    {
        if (!_init)
            return;

        _timer += Time.deltaTime;
        if (_timer >= cloneDuration)
            DestroyClone();
    }

    public override void Init(Vector3 direction, Transform author)
    {
        FollowState state = (FollowState)GetComponent<StateMachine>().GetState(AIState.Following);
        if (state != null)
            state.SetFollowTarget(author);

        SoundManager.Instance.PlaySFXAt(23, transform.position, minDistance: 1.5f);
        Instantiate(smokeVFX, transform.position + new Vector3(0f, 0.5f, 0f), smokeVFX.transform.rotation);

        _init = true;
    }

    public void DestroyClone()
    {
        Instantiate(smokeVFX, transform.position + new Vector3(0f, 0.5f, 0f), smokeVFX.transform.rotation);
        SoundManager.Instance.PlaySFXAt(24, transform.position, minDistance: 1.5f);
        Destroy(gameObject);
    }
}