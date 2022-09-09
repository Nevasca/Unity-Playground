using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Animations.Rigging;
using Tweening;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Transform followTransform;
    [SerializeField] private float rotationPower = 10f;
    [SerializeField] private GameObject aimCamera;
    [SerializeField] private Transform followCamera;
    [SerializeField] private GameObject aimReticle;

    [Header("Slow Motion Aim")]
    [SerializeField] private Volume aimSlowVolume;

    [Header("Aim Constraint")]
    [SerializeField] private MultiAimConstraint _spineConstraint;
    [SerializeField] private Transform _targetConstraint;

    private LookAtNearby _playerLookNearby;
    private Vector2 _look;
    private bool _aiming;
    private Vector3 _followPositionOffset;
    private Vector3 _direction;
    private CinemachineBrain _cinemachineBrain;
    private Transform _camera;

    private bool _slowMotion;
    private float _maxHorizontalAngle = 80f;
    private float _maxVerticalAngle = 60f;

    private void Awake()
    {
        _followPositionOffset = followTransform.position - transform.position;
        followTransform.SetParent(null);
        _playerLookNearby = GetComponent<LookAtNearby>();
        _cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        _camera = _cinemachineBrain.transform;
    }

    public void OnLook(InputAction.CallbackContext inputAction)
    {
        _look = inputAction.ReadValue<Vector2>();
    }

    private void Update()
    {
        SyncPosition();

        if (_aiming)
            Aim();
        else if(followTransform.parent == null)
            SyncRotation();
    }

    private void SyncRotation()
    {
        _direction = followTransform.position - followCamera.position;
        followTransform.rotation = Quaternion.LookRotation(_direction);
    }

    private void SyncPosition()
    {
        followTransform.position = transform.position + _followPositionOffset;
    }

    private void Aim()
    {
        //Horizontal and vertical rotation
        followTransform.transform.rotation *= Quaternion.AngleAxis(_look.x * rotationPower, Vector3.up);
        followTransform.transform.rotation *= Quaternion.AngleAxis(-_look.y * rotationPower, Vector3.right);

        var angles = followTransform.transform.localEulerAngles;

        angles.z = 0f;
        angles.x = GetVerticalClampedAngle(angles);
        angles.y = GetHorizontalClampedAngle(angles);

        followTransform.transform.localEulerAngles = angles;

        _playerLookNearby.LookAt(followTransform.position + followTransform.forward * 5f);

        UpdateAimTargetPosition();
    }

    private float GetVerticalClampedAngle(Vector3 eulerAngles)
    {
        var angle = eulerAngles.x;

        if (angle > 180f && angle < 360f - _maxVerticalAngle)
            return 360f - _maxVerticalAngle;
        else if (angle < 180f && angle > _maxVerticalAngle)
            return _maxVerticalAngle;

        return angle;
    }

    private float GetHorizontalClampedAngle(Vector3 eulerAngles)
    {
        var angle = eulerAngles.y;

        if (angle > 180f && angle < 360f - _maxHorizontalAngle)
            return 360f - _maxHorizontalAngle;
        else if (angle < 180 && angle > _maxHorizontalAngle)
            return _maxHorizontalAngle;

        return angle;
    }

    public void EnableAim(float maxHorizontalAngle = 360f, float maxVertivalAngle = 60f, bool slowMotion = false)
    {
        _aiming = true;
        _maxHorizontalAngle = maxHorizontalAngle;
        _maxVerticalAngle = maxVertivalAngle;
        aimCamera.SetActive(true);
        aimReticle.SetActive(true);
        TweenVirtual.DoFloat(0f, 1f, 0.15f, DoConstraintWeight);
        SoundManager.Instance.PlaySFX(40, 1f, 1f);

        followTransform.SetParent(transform);
        followTransform.localEulerAngles = Vector3.zero;

        if (slowMotion)
            EnableSlowMotion();
    }

    private void UpdateAimTargetPosition()
    {
        _targetConstraint.position = _camera.position + _camera.transform.forward * 50f;
    }

    //public void SetAimTargetPosition(Vector3 position)
    //{
    //    _targetConstraint.position = position;
    //}

    public void DisableAim()
    {
        _aiming = false;
        aimCamera.SetActive(false);
        aimReticle.SetActive(false);
        TweenVirtual.DoFloat(1f, 0f, 0.15f, DoConstraintWeight);
        SoundManager.Instance.PlaySFX(40, 0.5f, pitch: 0.8f);

        DisableSlowMotion();

        followTransform.SetParent(null);
        _playerLookNearby.CancelLookAt();
    }

    private void EnableSlowMotion()
    {
        _slowMotion = true;

        _cinemachineBrain.m_IgnoreTimeScale = true;
        Time.timeScale = 0.1f;
        TweenVirtual.DoFloat(0f, 1f, 0.25f, DOVolumeWeight);
    }

    private void DisableSlowMotion()
    {
        if (!_slowMotion)
            return;

        _slowMotion = false;

        TweenVirtual.DoFloat(1f, 0f, 0.1f, DOVolumeWeight);
        _cinemachineBrain.m_IgnoreTimeScale = false;
        Time.timeScale = 1f;
    }

    private void DOVolumeWeight(float value)
    {
        aimSlowVolume.weight = value;
    }

    private void DoConstraintWeight(float value)
    {
        _spineConstraint.weight = value;
    }
}