using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCreator : Skill
{
    [SerializeField] private PortalPair portals;
    [SerializeField] private LayerMask layerMask;

    private Animator _animator;
    private Transform _camera;
    private PlayerAim _aim;
    private PlayerMovement _movement;

    public override void Init(Transform skillOrigin)
    {
        _animator = _author.GetComponent<Animator>();
        _aim = _author.GetComponent<PlayerAim>();
        _movement = _author.GetComponent<PlayerMovement>();
        _camera = Camera.main.transform;
        portals = GameObject.FindGameObjectWithTag("PortalPair").GetComponent<PortalPair>();
    }

    public override void Use(bool started)
    {
        if (started)
        {
            _movement.SmoothVelocityScale(0f);
            _aim.EnableAim();
        }
        else
        {
            FirePortal(0, _camera.position, _camera.forward, 250f);
            _aim.DisableAim();
            _movement.SmoothVelocityScale(1f);
        }
    }

    public override void UseSecondary(bool started)
    {
        if (started)
        {
            _movement.SmoothVelocityScale(0f);
            _aim.EnableAim();
        }
        else
        {
            FirePortal(1, _camera.position, _camera.forward, 250f);
            _aim.DisableAim();
            _movement.SmoothVelocityScale(1f);
        }
    }

    private void FirePortal(int portalID, Vector3 pos, Vector3 dir, float distance)
    {
        RaycastHit hit;
        Physics.Raycast(pos, dir, out hit, distance, layerMask);

        if (hit.collider == null)
            return;

        // Orient the portal according to camera look direction and surface direction.
        var cameraRotation = _camera.rotation;
        var portalRight = cameraRotation * Vector3.right;

        if (Mathf.Abs(portalRight.x) >= Mathf.Abs(portalRight.z))
            portalRight = (portalRight.x >= 0) ? Vector3.right : -Vector3.right;
        else
            portalRight = (portalRight.z >= 0) ? Vector3.forward : -Vector3.forward;

        var portalForward = -hit.normal;
        var portalUp = -Vector3.Cross(portalRight, portalForward);

        var portalRotation = Quaternion.LookRotation(portalForward, portalUp);

        //Attempt to place the portal
        if(portals.Portals[portalID].PlacePortal(hit.collider, hit.point, portalRotation))
        {
            _animator.SetTrigger("cast");
        }
    }
}