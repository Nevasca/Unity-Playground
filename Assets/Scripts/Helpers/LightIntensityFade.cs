using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightIntensityFade : MonoBehaviour
{
    [SerializeField] private Light _light;
    [SerializeField] private float _duration = 0.3f;
    [SerializeField] private float _finalIntensity = 0f;

    private float _fromIntensity;
    private float _lifeTime;

    private void Awake()
    {
        _fromIntensity = _light.intensity;
    }

    private void Update()
    {
        if(_lifeTime / _duration > 1f)
        {
            enabled = false;
            _light.enabled = false;
            return;
        }

        _light.intensity = Mathf.Lerp(_fromIntensity, _finalIntensity, _lifeTime / _duration);
        _lifeTime += Time.deltaTime;
    }

    private void Reset()
    {
        _light = GetComponent<Light>();
    }
}