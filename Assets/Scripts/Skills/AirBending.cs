using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBending : Skill
{
    [SerializeField] private float _minWindScale = 0.001f;
    [SerializeField] private float _changeSpeed = 1f;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _minPitch = 0.8f;
    [SerializeField] private float _maxPitch = 1.3f;
    [SerializeField] private float _minVolume = 0.2f;
    [SerializeField] private float _maxVolume = 0.8f;

    public const string ANIM_HOLDING_MAGIC = "isHoldingMagic";

    private float _maxWindScale;
    private float _currentScale = 1f;
    private bool _holdingButton;
    private float _speed;
    private Animator _animator;

    public override void Init(Transform skillOrigin)
    {
        _animator = _author.GetComponent<Animator>();
        _maxWindScale = WindManager.Instance.MaxWindScale;
    }

    public override void Use(bool started)
    {
        SetHolding(started);
        _speed = _changeSpeed;
    }

    public override void UseSecondary(bool started)
    {
        SetHolding(started);
        _speed = -_changeSpeed;
    }

    private void SetHolding(bool value)
    {
        _holdingButton = value;
        _animator.SetBool(ANIM_HOLDING_MAGIC, _holdingButton);

        if (_holdingButton)
        {
            UpdateEffects();
            _audioSource.Play();
        }
        else
        {
            _audioSource.SmoothStop();
        }
    }

    private void Update()
    {
        if (!_holdingButton)
            return;

        _currentScale += _speed * Time.deltaTime;
        _currentScale = Mathf.Clamp(_currentScale, _minWindScale, _maxWindScale);

        WindManager.Instance.SetWindScale(_currentScale);
        UpdateEffects();
    }

    private void UpdateEffects()
    {
        float t = _currentScale / _maxWindScale;

        _audioSource.pitch = Mathf.Lerp(_minPitch, _maxPitch, t);
        _audioSource.volume = Mathf.Lerp(_minVolume, _maxVolume, t);
    }
}
