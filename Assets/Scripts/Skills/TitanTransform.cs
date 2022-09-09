using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;
using Tweening;

public class TitanTransform : Skill
{
    [SerializeField] private PlayableAsset _playable;
    [SerializeField] private float _titanDuration = 5f;
    [SerializeField] private float _scaleSize = 5f;
    [SerializeField] private float _titanMass = 20f;
    [SerializeField] private GameObject _vfxSmoke;
    [SerializeField] private AudioClip _clipTransformation;

    private Animator _animator;
    private PlayerMovementRigidbody _movement;
    private Footstep _footstep;
    private float _defaultMass;

    private bool _turnedTitan;
    private bool _transformationReady;
    private float _titanTimer;
    private float _defaultGroundRadiusCheck;

    public override void Init(Transform skillOrigin)
    {
        _animator = _author.GetComponent<Animator>();
        _movement = _author.GetComponent<PlayerMovementRigidbody>();
        _footstep = _author.GetComponent<Footstep>();
        _defaultMass = _movement.Rigidbody.mass;

        _defaultGroundRadiusCheck = _movement.GroundCheckRadius;
    }

    private void Update()
    {
        if (!_transformationReady)
            return;

        _titanTimer += Time.deltaTime;

        if (_titanTimer >= _titanDuration)
            CancelTitanTransform();
    }

    public override void Use(bool started)
    {
        if (!started) 
            return;

        StartTitanTransform();
    }

    public override void UseSecondary(bool started) { }

    private void StartTitanTransform()
    {
        if (_turnedTitan)
            return;

        _transformationReady = false;
        _titanTimer = 0f;
        _turnedTitan = true;
        _movement.Enable(false);
        CameraManager.Instance.Director.Play(_playable);
        SoundManager.Instance.PlayBGM(_clipTransformation);
    }

    private void CancelTitanTransform()
    {
        _transformationReady = false;
        _movement.Enable(false);

        //
        SoundManager.Instance.PlayBGM(null);
        //CameraManager.Instance.Director.Stop();
        TweenSequence sequence = Tweener.Sequence();
        sequence.AppendCallback(() => Instantiate(_vfxSmoke, _author.position, _vfxSmoke.transform.rotation));
        sequence.AppendInterval(0.1f);
        sequence.Append(_author.DoScale(1f, 1f));
        sequence.AppendCallback(() => 
        {
            CameraManager.Instance.DisableCamera(CameraType.FollowTitan);
            CameraManager.Instance.DisableCamera(CameraType.CloseTitan);
            _movement.SetAnimationScale(1f);
            _movement.SetGravityScale(1f);
            _movement.Rigidbody.mass = _defaultMass;
            _movement.Enable(true);
            _turnedTitan = false;
            _footstep.ShakeOnStep = false;
        });
        sequence.Play();

        _author.DoScale(1f, 1f);
    }

    public override void EventSkillCast()
    {        
        _author.DoScale(_scaleSize, 1f);
    }

    public override void EventSkillEnded()
    {
        base.EventSkillEnded();

        CameraManager.Instance.EnableCamera(CameraType.FollowTitan);
        CameraManager.Instance.DisableCamera(CameraType.CloseTitan);
        _movement.GroundCheckRadius = 0.5f;
        _footstep.ShakeOnStep = true;
        _movement.SetAnimationScale(0.5f);
        _movement.SetGravityScale(0.5f);
        _movement.Rigidbody.mass = _titanMass;
        _movement.Enable(true);
        _transformationReady = true;
    }
}