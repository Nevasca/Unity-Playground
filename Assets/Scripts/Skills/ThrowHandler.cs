using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrowHandler : SkillInstance
{
    [SerializeField] private bool _isSolid = false;

    protected bool _init;

    private void Awake()
    {
        GetComponent<Rigidbody>().isKinematic = true;

        if (_isSolid && TryGetComponent(out Collider collider))
            collider.isTrigger = true;
    }

    public override void Init(Vector3 force, Transform author)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = force;

        if(_isSolid && TryGetComponent(out Collider collider))
        {
            StartCoroutine(IgnoreAuthorCoroutine(collider, author));
            collider.isTrigger = false;
        }

        _init = true;
    }

    private IEnumerator IgnoreAuthorCoroutine(Collider handlerCollider, Transform author)
    {
        CapsuleCollider authorCollider = author.GetComponent<CapsuleCollider>();
        Physics.IgnoreCollision(handlerCollider, authorCollider, true);

        yield return new WaitForSeconds(0.5f);

        Physics.IgnoreCollision(handlerCollider, authorCollider, false);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}