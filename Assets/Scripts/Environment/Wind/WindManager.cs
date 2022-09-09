using System;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

public class WindManager : Singleton<WindManager>
{
    [SerializeField] private WindZone _windZone;
    [SerializeField] private float _maxWindScale = 10f;

    [Header("Ambience")]
    [SerializeField] private float _minFogDensity = 0.03f;
    [SerializeField] private float _maxFogDensity = 0.08f;
    [SerializeField] private Material[] _foliageMaterials;

    [Header("Audio")]
    [SerializeField] private float _maxWindVolume = 0.8f;
    [SerializeField] private float _minAudioPitch = 0.4f;
    [SerializeField] private float _maxAudioPitch = 0.8f;

    private Vector3 _windDesiredDirection;
    private float _directionChangeSpeed = 10f;
    private Vector3 _windDirection;
    private float _windPulseForce;
    private float _timer;
    private Vector3 _windForce;
    private float _defaultWindMain;
    private AudioSource _audioSource;

    private bool _rewinding;

    public static event Action<Vector3> OnWind;

    public float WindMain { get => _windZone.windMain; set => _windZone.windMain = value; }
    public float WindScale { get; private set; } = 1f;
    public float MaxWindScale => _maxWindScale;

    protected override void Awake()
    {
        base.Awake();

        _audioSource = GetComponent<AudioSource>();

        ResetPulse();
        _windDesiredDirection = transform.forward;
        _windDirection = _windDesiredDirection;
        _defaultWindMain = _windZone.windMain;

        SetWindScale(1f);
        _audioSource.Play();

        RewindManager.OnStartRewind += OnStartRewind;
        RewindManager.OnStartRewind += OnStopRewind;
    }

    private void Start()
    {
        StartCoroutine(ChangeWindCoroutine());
    }

    private void Update()
    {
        if (_rewinding)
            return;

        _timer -= Time.deltaTime;
        _windPulseForce = _windZone.windPulseMagnitude * _timer * WindScale;

        if (_timer <= 0f)
            ResetPulse();

        _windDirection = Vector3.MoveTowards(_windDirection, _windDesiredDirection, _directionChangeSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (_rewinding)
            return;

        _windForce = _windZone.windMain * _windPulseForce * _windDirection;
        OnWind?.Invoke(_windForce);
    }

    private void ResetPulse()
    {
        _timer = _windZone.windPulseFrequency;
    }

    public void SetWindScale(float value)
    {
        WindScale = value;
        WindMain = _defaultWindMain * WindScale;
        _directionChangeSpeed = _windZone.windTurbulence * WindScale / 2f;
        UpdateAmbience();
    }

    private void UpdateAmbience()
    {
        float t = WindScale / MaxWindScale;
        RenderSettings.fogDensity = Mathf.Lerp(_minFogDensity, _maxFogDensity, t);

        _audioSource.pitch = Mathf.Lerp(_minAudioPitch, _maxAudioPitch, t);
        _audioSource.volume = Mathf.Lerp(0f, _maxWindVolume, t);

        foreach (var material in _foliageMaterials)
            material.SetFloat("_MaxWindStrength", t);
    }

    private IEnumerator ChangeWindCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / _windZone.windTurbulence * WindScale);

            Vector3 random = Random.insideUnitSphere;
            random.y *= random.y > 0f ? WindScale : 0f;

            _windDesiredDirection = random.normalized;
        }
    }

    #region Rewind
    private void OnStartRewind() => _rewinding = true;
    private void OnStopRewind() => _rewinding = false;
    #endregion

    private void OnDestroy()
    {
        RewindManager.OnStartRewind -= OnStartRewind;
        RewindManager.OnStartRewind -= OnStopRewind;
    }
}
