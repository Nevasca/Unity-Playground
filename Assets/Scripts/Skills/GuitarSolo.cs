using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GuitarSolo : Skill
{
    [Header("Guitar")]
    [SerializeField] private float _danceRange = 3f;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private CinemachineImpulseSource _impulseSource;

    [Header("Position")]
    [SerializeField] private Vector3 _positionOffset;
    [SerializeField] private Vector3 _rotationOffset;
    [SerializeField] private Transform _fretboardPivot;

    private const string ANIM_GUITAR_PLAYING = "isPlayingGuitar";

    private Animator _animator;
    private PlayerMovement _playerMovement;
    private PlayerRig _playerRig;

    private bool _isPlaying;
    private float _soloDuration;

    public override void Init(Transform skillOrigin)
    {
        _animator = _author.GetComponent<Animator>();
        _playerMovement = _author.GetComponent<PlayerMovement>();
        _playerRig = _author.GetComponent<PlayerRig>();

        transform.parent = _playerRig.SpineMiddle;
        transform.localPosition = _positionOffset;
        transform.localRotation *= Quaternion.Euler(_rotationOffset);

        _soloDuration = _audioSource.clip.length;
        _impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = _soloDuration;

        gameObject.SetActive(false);
    }

    public override void Use(bool started)
    {
        if (_isPlaying)
            return;

        if (!started)
            return;

        PlayGuitarSolo();
    }

    public override void UseSecondary(bool started) { }

    private void PlayGuitarSolo()
    {
        _isPlaying = true;
        _playerMovement.Enable(false);
        _animator.SetBool(ANIM_GUITAR_PLAYING, true);
        _impulseSource.GenerateImpulse(Camera.main.transform.forward);
        _audioSource.Play();

        CheckDanceablesNearby();

        StartCoroutine(WaitSoloCoroutine());
    }

    private void CheckDanceablesNearby()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _danceRange);

        foreach(var c in colliders)
            if (c.TryGetComponent(out IDanceable danceable))
                danceable.Dance(_soloDuration);
    }

    private IEnumerator WaitSoloCoroutine()
    {
        yield return new WaitForSeconds(_soloDuration);
        OnGuitarSoloFinished();
    }

    private void OnGuitarSoloFinished()
    {
        _isPlaying = false;
        _playerMovement.Enable(true);
        _animator.SetBool(ANIM_GUITAR_PLAYING, false);
    }

    private void OnDisable()
    {
        if (!_isPlaying)
            return;

        StopAllCoroutines();
        OnGuitarSoloFinished();
    }
}