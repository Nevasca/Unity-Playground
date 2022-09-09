using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.AddressableAssets;

public class Machinegun : Skill
{
    [Header("Machinegun")]
    [SerializeField] private float _damage = 20f;
    [SerializeField] private float _shootStrength = 10f;
    [SerializeField] private Transform _gunTip;
    [SerializeField] private float _fireRate = 0.2f;
    [SerializeField] private LayerMask _shootMask;

    [Header("Ammo")]
    [SerializeField] private int _maxAmmo = 150;
    [SerializeField] private int _maxAmmoMagazine = 50;

    [Header("Audio")]
    [SerializeField] private AudioClip _shootClip;
    [SerializeField] private AudioClip _hitClip;
    [SerializeField] private AudioClip _noAmmoClip;
    [SerializeField] private AudioClip _reloadStartClip;
    [SerializeField] private AudioClip _reloadEndClip;


    [Header("Effects")]
    [SerializeField] private AssetReference _muzzleVfxReference;
    [SerializeField] private AssetReference _hitVfxReference;
    [SerializeField] private EffectTracer _effectTracer;
    [SerializeField] private BulletShellCasing _bulletShellCasing;
    [SerializeField] private CinemachineImpulseSource _impulseSource;

    #region Events
    public event Action<int> OnAmmoMagazinelChanged;
    public event Action<int> OnAmmoChanged;
    #endregion

    public int CurrentAmmo => _currentAmmo;
    public int CurrentAmmoMagazine => _currentAmmoMagazine;

    public const string ANIM_HOLDING_RIFLE = "isHoldingRifle";
    public const string ANIM_FIRING_RIFLE = "isFiringRifle";
    public const string ANIM_AIMING_RIFLE = "isAimingRifle";
    public const string ANIM_RELOADING_RIFLE = "isReloadingRifle";

    private PlayerAim _playerAim;
    private Animator _playerAnimator;
    private bool _aiming;
    private Transform _camera;

    private float _lastShootTime;
    private bool _holdingShoot;
    private Ray _shootRay;
    private RaycastHit _hit;

    private bool _reloading;
    private int _currentAmmo;
    private int _currentAmmoMagazine;
    private float _lastNoAmmoPlayed;

    private float _tracerDistance;
    private Vector3 _tracerDirection;
    private EffectTracer _tracer;

    private ObjectPool _poolMuzzle;
    private ObjectPool _poolHit;

    public override void Init(Transform skillOrigin)
    {
        base.Init(skillOrigin);

        _playerAim = _author.GetComponent<PlayerAim>();
        _playerAnimator = _author.GetComponent<Animator>();

        _camera = Camera.main.transform;

        transform.parent = skillOrigin;
        transform.localPosition = Vector3.zero;

        _currentAmmo = _maxAmmo;
        _currentAmmoMagazine = _maxAmmoMagazine;

        _poolMuzzle = ObjectPool.CreatePool(_muzzleVfxReference, 10, true);
        _poolHit = ObjectPool.CreatePool(_hitVfxReference, 10, true);

        gameObject.SetActive(false);
    }

    #region Input handlers
    public override void Use(bool started)
    {
        _holdingShoot = started;
        _playerAnimator.SetBool(ANIM_FIRING_RIFLE, started);

        
    }

    public override void UseSecondary(bool started)
    {
        if (started)
            _playerAim.EnableAim(50f);
        else
            _playerAim.DisableAim();

        _aiming = started;
        _playerAnimator.SetBool(ANIM_AIMING_RIFLE, started);
    }

    public override void UseTertiary(bool started)
    {
        if (!started)
            return;

        Reload();
    }
    #endregion

    private void Update()
    {
        if (!_holdingShoot)
            return;

        Shoot();
    }

    private void Shoot()
    {
        if (_reloading)
            return;

        if (Time.time - _lastShootTime < _fireRate)
            return;

        if (!SpendAmmo())
            return;

        _lastShootTime = Time.time;

        SoundManager.Instance.PlaySFXAt(_shootClip, _gunTip.position, 0.8f, pitchRange: 0.2f);
        _impulseSource.GenerateImpulse(_camera.forward);
        Instantiate(_bulletShellCasing, _gunTip.position, Quaternion.identity);

        _tracerDistance = 50f;
        SetShootRay();

        if (Physics.Raycast(_shootRay, out _hit, 50f, _shootMask))
        {
            if (_hit.collider.TryGetComponent(out IDamageable damageable))
                damageable.Damage(_damage);

            if (_hit.collider.TryGetComponent(out IShootable shootable))
                shootable.OnShoot(_shootRay.direction * _shootStrength);

            //Effects
            SoundManager.Instance.PlaySFXAt(_hitClip, _hit.point, pitchRange: 0.1f);

            _poolHit.TryGetObject(_hit.point, Quaternion.LookRotation(_hit.normal), out _,
                (GameObject go) => { Debug.Log($"Demorou mas chegou o {go.name}"); });

            _tracerDistance = Vector3.Distance(_gunTip.position, _hit.point);
            _tracerDirection = _hit.point - _gunTip.position;
            _tracerDirection.Normalize();            
        }
        else
        {
            _tracerDirection = (_shootRay.origin + _shootRay.direction * _tracerDistance) - _gunTip.position;
            _tracerDirection.Normalize();            
        }

        _poolMuzzle.TryGetObject(_gunTip.position, Quaternion.LookRotation(_gunTip.forward), out _, null);
        _tracer = Instantiate(_effectTracer, _gunTip.position, Quaternion.LookRotation(_tracerDirection));
        _tracer.DoEffect(_tracerDistance);
    }

    private void SetShootRay()
    {
        _shootRay.origin = _aiming ? _camera.position : _gunTip.position;
        _shootRay.direction = _aiming ? _camera.forward : _gunTip.forward;
    }

    private bool SpendAmmo()
    {
        if(_currentAmmoMagazine == 0)
        {
            Reload();
            return false;
        }

        _currentAmmoMagazine -= 1;
        OnAmmoMagazinelChanged?.Invoke(_currentAmmoMagazine);
        return true;
    }

    private void Reload()
    {
        if (_reloading)
            return;

        if (_currentAmmoMagazine == _maxAmmoMagazine)
            return;

        if (_currentAmmo == 0)
        {
            if(Time.time - _lastNoAmmoPlayed >= _noAmmoClip.length)
            {
                _lastNoAmmoPlayed = Time.time;
                SoundManager.Instance.PlaySFXAt(_noAmmoClip, _gunTip.position, pitchRange: 0.1f);
            }

            return;
        }

        _reloading = true;
        _holdingShoot = false;
        SoundManager.Instance.PlaySFXAt(_reloadStartClip, _gunTip.position, pitchRange: 0.05f);
        _playerAnimator.SetBool(ANIM_RELOADING_RIFLE, true);
    }
    private void OnReloadCompleted()
    {
        _reloading = false;

        int reloadedAmmo = Mathf.Min(_maxAmmoMagazine - _currentAmmoMagazine, _currentAmmo);
        _currentAmmo -= reloadedAmmo;
        _currentAmmoMagazine += reloadedAmmo;
        OnAmmoMagazinelChanged?.Invoke(_currentAmmoMagazine);
        OnAmmoChanged?.Invoke(_currentAmmo);

        SoundManager.Instance.PlaySFXAt(_reloadEndClip, _gunTip.position, pitchRange: 0.05f);
        _playerAnimator.SetBool(ANIM_RELOADING_RIFLE, false);
    }

    #region Animation Events
    public override void EventSkillEnded()
    {
        base.EventSkillEnded();
        OnReloadCompleted();
    }
    #endregion

    private void OnEnable()
    {
        _poolHit.InstantiatePool();
        _poolMuzzle.InstantiatePool();

        if (_playerAnimator == null)
            return;

        _playerAnimator.SetBool(ANIM_HOLDING_RIFLE, true);
    }

    private void OnDisable()
    {
        _playerAnimator.SetBool(ANIM_HOLDING_RIFLE, false);

        _poolHit.DeletePool();
        _poolMuzzle.DeletePool();
    }
}