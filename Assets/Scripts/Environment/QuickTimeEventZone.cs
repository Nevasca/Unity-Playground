using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using Tweening;

public class QuickTimeEventZone : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] private float timeScale = 0.1f;
    [SerializeField] private float timeToReact = 0.8f;

    [Header("Event")]
    [SerializeField] private InputAction inputAction;
    [SerializeField] private QuickTimeEvent quickTimeEventPerformed;
    [SerializeField] private Transform aimTarget;
    [SerializeField] private Health enemyHealth;
    [SerializeField] private EnemyTarget enemyTarget;

    [Header("VFX")]
    [SerializeField] private Volume volumeEvent;

    private bool _quickTimeZoneStarted;
    private bool _quickTimePerformed;
    private float _timer;

    private void Update()
    {
        if (!_quickTimeZoneStarted)
            return;

        if (_quickTimePerformed)
            return;

        _timer += Time.deltaTime;

        if (_timer >= timeToReact)
            CancelQuickTimeEvent();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_quickTimePerformed)
            return;

        if (other.CompareTag("Player"))
            StartQuickTimeEvent();
    }
    
    private void StartQuickTimeEvent()
    {
        if (_quickTimeZoneStarted)
            return;

        _timer = 0f;
        CameraManager.Instance.IgnoreTimeScale(true);
        CameraManager.Instance.EnableCamera(CameraType.QuickTimeEvent, lookAt: aimTarget);
        Time.timeScale = timeScale;
        volumeEvent.DoWeight(1f, 0.15f);
        
        _quickTimeZoneStarted = true;
    }

    private void CancelQuickTimeEvent()
    {
        _quickTimeZoneStarted = false;
        Time.timeScale = 1f;
        CameraManager.Instance.IgnoreTimeScale(false);
        CameraManager.Instance.DisableCamera(CameraType.QuickTimeEvent);
        volumeEvent.DoWeight(0f, 0.15f);
    }

    private void PerformQuickTimeEvent(InputAction.CallbackContext input)
    {
        if (!_quickTimeZoneStarted)
            return;

        _quickTimePerformed = true;
        CancelQuickTimeEvent();
        enemyTarget.SetEnemy(enemyHealth, aimTarget);
        quickTimeEventPerformed.Invoke();
    }

    private void OnEnable()
    {
        inputAction.started += PerformQuickTimeEvent;
        inputAction.Enable();
    }

    private void OnDisable()
    {
        inputAction.started -= PerformQuickTimeEvent;
        inputAction.Disable();
    }
}