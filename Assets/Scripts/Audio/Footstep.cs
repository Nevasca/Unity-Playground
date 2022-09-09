using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Footstep : MonoBehaviour
{
    [SerializeField] private int[] clipIDs;
    [SerializeField] private float stepVolume = 0.6f;
    [SerializeField] private float stepInterval = 0f;
    [SerializeField] private float stepPitch = 0.9f;

    public bool ShakeOnStep = false;

    private float lastStepTime;
    private CinemachineImpulseSource _impulseSource;
    private Transform _camera;

    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _camera = Camera.main.transform;
    }

    //Chamado por eventos de animacao
    protected virtual void Step()
    {
        if(Time.time - lastStepTime >= stepInterval)
        {
            lastStepTime = Time.time;
            SoundManager.Instance.PlaySFXAt(GetRandomClip(), transform.position, stepVolume, stepPitch, pitchRange: 0.05f);

            if (ShakeOnStep)
                Shake();
        }
    }

    private void Shake()
    {
        _impulseSource?.GenerateImpulse(_camera.forward);
    }

    protected virtual void Land()
    {
        SoundManager.Instance.PlaySFXAt(6, transform.position, pitchRange: 0.05f);
    }

    private int GetRandomClip()
    {
        return clipIDs[Random.Range(0, clipIDs.Length)];
    }
}