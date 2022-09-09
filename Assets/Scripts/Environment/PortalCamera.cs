using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class PortalCamera : MonoBehaviour
{
    [SerializeField]
    private Portal[] portals = new Portal[2];

    [SerializeField]
    private Camera portalCamera;

    [SerializeField]
    private int iterations = 7;

    private RenderTexture _tempTexture1;
    private RenderTexture _tempTexture2;

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = GetComponent<Camera>();

        _tempTexture1 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        _tempTexture2 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
    }

    private void Start()
    {
        portals[0].Renderer.material.mainTexture = _tempTexture1;
        portals[1].Renderer.material.mainTexture = _tempTexture2;
    }

    private void OnEnable()
    {
        RenderPipeline.beginCameraRendering += UpdateCamera;
    }

    private void OnDisable()
    {
        RenderPipeline.beginCameraRendering -= UpdateCamera;
    }

    private void UpdateCamera(ScriptableRenderContext SRC, Camera camera)
    {
        if (!portals[0].IsPlaced || !portals[1].IsPlaced)
            return;

        if(portals[0].Renderer.isVisible)
        {
            portalCamera.targetTexture = _tempTexture1;
            for (int i = iterations - 1; i >= 0; --i)
            {
                RendererCamera(portals[0], portals[1], i, SRC);
            }
        }

        if(portals[1].Renderer.isVisible)
        {
            portalCamera.targetTexture = _tempTexture2;
            for (int i = iterations - 1; i >= 0; --i)
            {
                RendererCamera(portals[1], portals[0], i, SRC);
            }
        }
    }

    private void RendererCamera(Portal inPortal, Portal outPortal, int iterationID, ScriptableRenderContext SRC)
    {
        Transform inTransform = inPortal.transform;
        Transform outTransform = outPortal.transform;

        Transform cameraTransform = portalCamera.transform;
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;

        for (int i = 0; i <= iterationID; ++i)
        {
            //Position the camera behind the portal
            Vector3 relativePos = inTransform.InverseTransformPoint(cameraTransform.position);
            relativePos = Quaternion.Euler(0f, 180f, 0f) * relativePos;
            cameraTransform.position = outTransform.TransformPoint(relativePos);

            //Rotate the camera to look through the other portal
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
            relativeRot = Quaternion.Euler(0f, 180f, 0f) * relativeRot;
            cameraTransform.rotation = outTransform.rotation * relativeRot;
        }

        //Set the camera's oblique view frustum
        Plane p = new Plane(-outTransform.forward, outTransform.position);
        Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) *clipPlaneWorldSpace;

        var newMatrix = _mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCamera.projectionMatrix = newMatrix;

        //Render the camera to its target
        UniversalRenderPipeline.RenderSingleCamera(SRC, portalCamera);
    }
}