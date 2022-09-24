using Cinemachine;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using Tweening;
using UnityEngine;

public class AreaStun : Skill
{
    [Header("Params")]
    [SerializeField] private float _stunRadius = 2f;
    [SerializeField] private float _stunDuration = 3f;
    [SerializeField] private float _stunDelay = 0.5f;
    [SerializeField] private float _poseDuration = 0.5f;
    [SerializeField] private float _cooldownTime = 3.5f;

    [Header("Visual")]
    [SerializeField] private GameObject _vfxStunTemplate;

    [Header("Audio")]
    [SerializeField] private AudioClip _sfxRing;

    private Animator _animator;
    private PlayerMovement _playerMovement;
    private Rigidbody _authorRigidbody;
    private int _poseStateHash;
    private float _lastUsedTime;

    public override void Init(Transform skillOrigin)
    {
        base.Init(skillOrigin);

        _animator = _author.GetComponent<Animator>();
        _poseStateHash = Animator.StringToHash("GroundPose");
        _playerMovement = _author.GetComponent<PlayerMovement>();
        _authorRigidbody = _author.GetComponent<Rigidbody>();
        _lastUsedTime = -_cooldownTime;
    }

    public override void Use(bool started)
    {
        if (!started)
            return;

        if (Time.time - _lastUsedTime < _cooldownTime)
            return;

        StartCoroutine(StartStunCoroutine());
    }

    public override void UseSecondary(bool started) { }

    private IEnumerator StartStunCoroutine()
    {
        _lastUsedTime = Time.time;
        _playerMovement.Enable(false);
        _animator.CrossFadeInFixedTime(_poseStateHash, _stunDelay);

        yield return new WaitForSeconds(_stunDelay);

        StunNearby();

        yield return new WaitForSeconds(_poseDuration);

        _playerMovement.Enable(true);
    }

    private void StunNearby()
    {
        Collider[] colliders = Physics.OverlapSphere(_author.position, _stunRadius);

        foreach (var c in colliders)
            if (c.TryGetComponent(out IDanceable danceable))
                danceable.Dance(_stunDuration);

        SpawnVFX();
    }

    private void SpawnVFX()
    {
        if (_vfxStunTemplate == null)
            return;

        Vector3 spawnPosition = _author.position;
        spawnPosition.y += 0.01f;

        GameObject vfx = Instantiate(_vfxStunTemplate, spawnPosition, _vfxStunTemplate.transform.rotation);
        vfx.transform.localScale = _stunRadius * 2f * Vector3.one;

        GetComponent<CinemachineImpulseSource>().GenerateImpulse(Camera.main.transform.forward);

        SoundManager.Instance.PlaySFXAt(_sfxRing, spawnPosition, minDistance: 2f);
    }

    private void OnDrawGizmos()
    {
        if (_author == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_author.position, _stunRadius);
    }
}