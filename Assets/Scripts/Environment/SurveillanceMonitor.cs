using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveillanceMonitor : MonoBehaviour
{
    [SerializeField] private RenderTexture _renderTextureTemplate;
    [SerializeField] private MeshRenderer _screenRenderer;
    [SerializeField] private SurveillanceCamera _surveillanceCamera;

    private const string SHADER_TEXTURE = "_Texture";
    private const string SHADER_ACTIVE = "_Active";

    private void Awake()
    {
        InitMonitor();
        _surveillanceCamera.OnCameraToggled += OnCameraToggled;
    }

    private void InitMonitor()
    {
        var renderTexture = new RenderTexture(_renderTextureTemplate);
        renderTexture.Create();

        _surveillanceCamera.Camera.targetTexture = renderTexture;
        _screenRenderer.material.SetTexture(SHADER_TEXTURE, renderTexture);
    }

    private void OnCameraToggled(bool active)
    {
        _screenRenderer.material.SetInt(SHADER_ACTIVE, active ? 1 : 0);
    }

    private void OnDestroy()
    {
        _surveillanceCamera.OnCameraToggled -= OnCameraToggled;
    }
}