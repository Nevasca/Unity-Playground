using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tweening;

public class Telekinesis : Skill
{
    [SerializeField] private float releaseForce = 10f;
    [SerializeField] private LayerMask layerMask;

    private Animator _animator;
    private PlayerAim _aim;
    private PlayerMovement _movement;
    private Rigidbody _holdingObject;
    private Transform _camera;
    private Transform _skillOrigin;
    private bool _syncPosition;
    private Vector3 _offset = new Vector3(1f, 1f, 1f);

    public override void Init(Transform skillOrigin)
    {
        _animator = _author.GetComponent<Animator>();
        _aim = _author.GetComponent<PlayerAim>();
        _movement = _author.GetComponent<PlayerMovement>();
        _camera = Camera.main.transform;
        _skillOrigin = skillOrigin;
    }

    public override void Use(bool started)
    {
        if(started)
        {
            _movement.Enable(false);
            _aim.EnableAim(slowMotion: true);
        }
        else if(_holdingObject == null)
        {
            TryControlling();
        }
        else
        {
            Release();
        }
    }

    private void FixedUpdate()
    {
        if(!_syncPosition || _holdingObject == null)
            return;
        
        _holdingObject.position = _skillOrigin.position + _offset;
    }    

    public override void UseSecondary(bool started) { }

    private void TryControlling()
    {
        if(Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, 200f, layerMask))
        {            
            if(hit.collider.TryGetComponent(out Rigidbody rb))
            {
                _holdingObject = rb;                
                _holdingObject.isKinematic = true;
                    ToggleHoldingColliders(false);
                    rb.DoMove(_skillOrigin.position + _offset, 0.2f)
                        .OnComplete(() => _syncPosition = true);
            }
        } 

        _aim.DisableAim();
        _movement.Enable(true);
    }

    private void Release()
    {
        _holdingObject.transform.parent = null;
        _holdingObject.isKinematic = false;
        ToggleHoldingColliders(true);
        _holdingObject.AddForce(_camera.forward * releaseForce);
        _holdingObject = null;
        _syncPosition = false;

        _aim.DisableAim();
        _movement.Enable(true);
    }

    private void ToggleHoldingColliders(bool value)
    {
        Collider[] cols = _holdingObject.GetComponentsInChildren<Collider>();
        foreach(var c in cols)
            c.enabled = value;
    }
}