using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PortalableObject : MonoBehaviour
{
    [SerializeField] protected Collider _collider;

    protected GameObject _cloneObject;

    private int _inPortalCount = 0;

    private Portal _inPortal;
    private Portal _outPortal;

    private Rigidbody _rigidbody;

    private static readonly Quaternion halfTurn = Quaternion.Euler(0f, 180f, 0f);

    protected virtual void Awake()
    {
        CreateClone();

        _rigidbody = GetComponent<Rigidbody>();

        if(_collider == null)
            _collider = GetComponent<Collider>();
    }

    protected virtual void CreateClone()
    {
        _cloneObject = new GameObject();
        _cloneObject.SetActive(false);
        _cloneObject.transform.localScale = transform.localScale;

        MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
        if (renderer == null)
            return;

        var meshFilter = _cloneObject.AddComponent<MeshFilter>();
        var meshRenderer = _cloneObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = GetComponentInChildren<MeshFilter>().mesh;
        meshRenderer.materials = renderer.materials;
    }

    private void LateUpdate()
    {
        if (_inPortal == null || _outPortal == null)
            return;

        if(_cloneObject.activeSelf && _inPortal.IsPlaced && _outPortal.IsPlaced)
        {
            var inTransform = _inPortal.transform;
            var outTransform = _outPortal.transform;

            //Update position of clone
            Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
            relativePos = halfTurn * relativePos;
            _cloneObject.transform.position = outTransform.TransformPoint(relativePos);

            //Update rotation of clone
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
            relativeRot = halfTurn * relativeRot;
            _cloneObject.transform.rotation = outTransform.rotation * relativeRot;
        }
        else
        {
            _cloneObject.transform.position = new Vector3(-1000f, 1000f, -1000f);
        }
    }

    public virtual void SetIsInPortal(Portal inPortal, Portal outPortal, Collider wallCollider)
    {
        _inPortal = inPortal;
        _outPortal = outPortal;

        Physics.IgnoreCollision(_collider, wallCollider);

        _cloneObject.SetActive(true);

        ++_inPortalCount;
    }

    public virtual void ExitPortal(Collider wallCollider)
    {
        Physics.IgnoreCollision(_collider, wallCollider, false);
        --_inPortalCount;

        if (_inPortalCount == 0)
            _cloneObject.SetActive(false);
    }

    public virtual void Warp()
    {
        var inTransform = _inPortal.transform;
        var outTransform = _outPortal.transform;

        //Update position of object
        Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
        relativePos = halfTurn * relativePos;
        transform.position = outTransform.TransformPoint(relativePos);

        //Update rotation of object
        Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
        relativeRot = halfTurn * relativeRot;
        transform.rotation = outTransform.rotation * relativeRot;

        //Update velocity of rigidbody
        Vector3 relativeVel = inTransform.InverseTransformDirection(_rigidbody.velocity);
        relativeVel = halfTurn * relativeVel;
        _rigidbody.velocity = outTransform.TransformDirection(relativeVel);

        //Swap portal references
        var tmp = _inPortal;
        _inPortal = _outPortal;
        _outPortal = tmp;

        SoundManager.Instance.PlaySFXAt(34, _inPortal.transform.position, minDistance: 1.2f);
    }
}
