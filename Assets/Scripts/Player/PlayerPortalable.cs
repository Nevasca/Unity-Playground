using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerPortalable : SkinnedPortalableObject
{
    [Header("Camera")]
    [SerializeField] private GameObject crossingCamera;
    [SerializeField] private CinemachineVirtualCamera crossedCamera;

    private Transform _cameraFollow;
    private Transform _outPortalTransform;

    protected override void CreateClone()
    {
        base.CreateClone();
        _cameraFollow = new GameObject("crossedCameraFollow").transform;
        crossedCamera.Follow = _cameraFollow;
    }

    public override void SetIsInPortal(Portal inPortal, Portal outPortal, Collider wallCollider)
    {
        base.SetIsInPortal(inPortal, outPortal, wallCollider);
        _outPortalTransform = outPortal.transform;
        crossingCamera.SetActive(true);
    }

    public override void ExitPortal(Collider wallCollider)
    {
        base.ExitPortal(wallCollider);
        crossingCamera.SetActive(false);
    }

    public override void Warp()
    {
        base.Warp();
        _cameraFollow.position = _outPortalTransform.position;
        _cameraFollow.rotation = _outPortalTransform.rotation * Quaternion.Euler(0f, 180f, 0f);
        crossedCamera.gameObject.SetActive(true);
        StartCoroutine(WaitAndDisable());
    }

    IEnumerator WaitAndDisable()
    {
        yield return new WaitForSeconds(0.1f);
        crossedCamera.gameObject.SetActive(false);
    }
}