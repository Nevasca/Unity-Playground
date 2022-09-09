using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Nixtor.AI;

public class SurveillanceCamera : MonoBehaviour, IActivable
{
    [Header("General")]
    [SerializeField] private Animator _animator;
    [SerializeField] private MultiAimConstraint _aimConstraint;
    [SerializeField] private Transform _aimTarget;
    [SerializeField] private bool _startActive = true;
    [SerializeField] private AICategory _category = AICategory.Enemy;
    [SerializeField] private Camera _camera;

    [Header("Audio")]
    [SerializeField] private AudioClip _sfxAlert;

    [Header("Visual")]
    [SerializeField] private Light _spotLight;
    [SerializeField] private Color _enemyColor = Color.red;
    [SerializeField] private Color _allieColor = Color.cyan;
    [SerializeField] private GameObject _vfxAlert;

    public static event Action<Transform, IDamageable> OnTargetSpotted;
    public event Action<bool> OnCameraToggled;

    public Camera Camera => _camera;

    private const string ANIM_ON = "on";
    private const string ANIM_AIMING = "aiming";

    private Transform _spotedTarget;
    private bool _isFocusing;
    private bool _isActive = true;

    private void Awake()
    {
        if(!_startActive)
        {
            ToggleActive();
        }
        else
        {
            _animator.SetBool(ANIM_ON, _startActive);
            SetCameraColor();
        }
    }

    private void Update()
    {
        if(!_isActive)
            return;

        UpdateAimPosition();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isActive)
            return;

        if (!other.TryGetComponent(out IDamageable damageable))
            return;

        StartFocusAt(other.transform, damageable);
    }

    private void StartFocusAt(Transform target, IDamageable damageable)
    {
        if (_spotedTarget != null)
            return;

        if (damageable.GetCategory() == _category)
            return;

        SetTarget(target);

        SoundManager.Instance.PlaySFXAt(_sfxAlert, transform.position, minDistance: 1.5f);
        _vfxAlert.SetActive(true);

        OnTargetSpotted?.Invoke(target, damageable);
    }

    private void SetTarget(Transform target)
    {
        bool hasTarget = target != null;

        _spotedTarget = target;
        _isFocusing = hasTarget;
        _animator.SetBool(ANIM_AIMING, hasTarget);
        UpdateAimPosition();
        _aimConstraint.weight = hasTarget ? 1f : 0f;
    }

    private void UpdateAimPosition()
    {
        if (_spotedTarget == null)
        {
            if(_isFocusing)
                SetTarget(null);

            return;
        }

        _aimTarget.position = _spotedTarget.position;
    }

    private void SetCameraColor()
    {
        if(!_spotLight.enabled)
        {
            GetComponent<MeshRenderer>().materials[^1].SetColor("_EmissionColor", Color.black);
            return;
        }

        Color color = _category == AICategory.Enemy ? _enemyColor : _allieColor;

        _spotLight.color = color;
        GetComponent<MeshRenderer>().materials[^1].SetColor("_EmissionColor", color);
    }

    #region Activable
    public void ToggleActive()
    {
        _isActive = !_isActive;
        _animator.SetBool(ANIM_ON, _isActive);
        _spotLight.enabled = _isActive;
        _camera.enabled = _isActive;
        SetCameraColor();

        if(!_isActive)
            SetTarget(null);

        OnCameraToggled?.Invoke(_isActive);
    }

    public bool IsActive() => _isActive;
    #endregion
}