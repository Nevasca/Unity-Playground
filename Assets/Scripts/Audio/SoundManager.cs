using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Tweening;

public class SoundManager : MonoBehaviour {

    [Header("Params")]
    [SerializeField] private int maxSFX;
    [SerializeField] private int maxSpatialSFX;
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private AudioClip[] sfxClips;

    [Header("Output")]
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private AudioMixerGroup _sfxOutputMixer;

    [Header("Snapshots")]
    [SerializeField] private float _snapshotTransition = 0.15f;
    [SerializeField] private AudioMixerSnapshot _gameplaySnapshot;
    [SerializeField] private AudioMixerSnapshot _menuSnapshot;

    private const string MIXER_MUSIC_VOLUME = "MusicVolume";
    private const string MIXER_SFX_VOLUME = "SFXVolume";
    private const string MIXER_MASTER_VOLUME = "MasterVolume";

    //Referencias
    private AudioSource _bgmSource;
    private float _fadeDuration = 0.5f;
    private AudioSource[] _sfxSources;
    private int _currentSFXSource;
    private AudioSource[] _spatialSfxSources;
    private int _currentSpatialSFXSource;

    public static SoundManager Instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            //Referencia do bgmSource
            _bgmSource = GetComponent<AudioSource>();

            //Cria os AudioSource's para sfx como children do SoundManager e seta as referencias

            //Static SFXs
            GameObject staticContainer = new GameObject();
            staticContainer.transform.SetParent(transform);
            staticContainer.name = "StaticSFXs";
            _sfxSources = new AudioSource[maxSFX];
            for (int i = 0; i < maxSFX; i++)
            {
                GameObject go = new GameObject();
                go.transform.SetParent(staticContainer.transform);
                go.name = "SFXSource" + i.ToString("D2");
                AudioSource source = go.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 0f;
                source.outputAudioMixerGroup = _sfxOutputMixer;
                _sfxSources[i] = source;
            }
            _currentSFXSource = -1;

            //Spatial SFXs
            GameObject spatialContainer = new GameObject();
            spatialContainer.transform.SetParent(transform);
            spatialContainer.name = "SpatialSFXs";
            _spatialSfxSources = new AudioSource[maxSpatialSFX];
            for (int i = 0; i < maxSpatialSFX; i++)
            {
                GameObject go = new GameObject();
                go.transform.SetParent(spatialContainer.transform);
                go.name = "SpatialSFXSource" + i.ToString("D2");
                AudioSource source = go.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 1f;

                source.outputAudioMixerGroup = _sfxOutputMixer;
                _spatialSfxSources[i] = source;
            }
            _currentSpatialSFXSource = -1;
        }
    }

    public void PlayBGM(AudioClip bgmClip, float volume = 1f, float newFadeDuration = -1f)
    {
        float fade = newFadeDuration > 0f ? newFadeDuration : _fadeDuration;

        if (_bgmSource.isPlaying)
        {
            //Esta tocando uma musica diferente da solicitada
            if(_bgmSource.clip != bgmClip)
            {
                //Fade out bgm antiga e fade in na nova
                TweenSequence clipChangeSequence = Tweener.Sequence();
                clipChangeSequence.Append(_bgmSource.DoVolume(0f, fade).OnComplete(() => {_bgmSource.clip = bgmClip; _bgmSource.Play(); }))
                    .Append(_bgmSource.DoVolume(volume, fade));
                clipChangeSequence.Play();
            }
        }
        else
        {
            //Nao tinha nada tocando, apenas fade in na bgm
            _bgmSource.volume = 0f;
            _bgmSource.clip = bgmClip;
            _bgmSource.Play();
            _bgmSource.DoVolume(volume, fade);                        
        }
    }

    //Toca uma BGM existente na lista de bgms
    public void PlayBGM(int clipID, float volume = 1f)
    {
        PlayBGM(bgmClips[clipID], volume);
    }

    public void FadeBGM(float endValue, float duration)
    {
        _bgmSource.DoVolume(endValue, duration);
    }

    //DOFade nao funciona com TimeScale = 0
    public void SetBGMVolume(float volume)
    {
        _bgmSource.volume = volume;
    }

    //Toca um SFX com o volume e pitch especificados. Aplica uma variacao aleatoria de pitch se o pitchRange for especificado
    public void PlaySFX(AudioClip sfxClip, float volume = 1f, float pitch = 1f, float pitchRange = 0f)
    {
        //Incrementa index, volta pro 0 se chegar no max
        _currentSFXSource = ++_currentSFXSource % maxSFX;
        
        _sfxSources[_currentSFXSource].clip = sfxClip;
        _sfxSources[_currentSFXSource].volume = volume;
        if (pitchRange > 0f)
            _sfxSources[_currentSFXSource].pitch = Random.Range(pitch - pitchRange, pitch + pitchRange);
        else
            _sfxSources[_currentSFXSource].pitch = pitch;

        _sfxSources[_currentSFXSource].Play();
    }

    public void PlaySFX(int clipID, float volume = 1f, float pitch = 1f, float pitchRange = 0)
    {
        if (clipID != -1)
            PlaySFX(sfxClips[clipID], volume, pitch, pitchRange);
    }

    public void PlaySFX(int clipID)
    {
        if (clipID != -1)
            PlaySFX(sfxClips[clipID]);        
    }

    //Toca um SFX na posicao especificada
    public void PlaySFXAt(AudioClip sfxClip, Vector3 position, float volume = 1f, float pitch = 1f, float pitchRange = 0f, float spatialBlend = 1f, float minDistance = 1f)
    {
        //Incrementa index, volta pro 0 se chegar no max
        _currentSpatialSFXSource = ++_currentSpatialSFXSource % maxSpatialSFX;

        AudioSource source = _spatialSfxSources[_currentSpatialSFXSource];

        source.clip = sfxClip;
        source.transform.position = position;
        source.volume = volume;

        source.spatialBlend = spatialBlend;
        source.minDistance = minDistance;

        if (pitchRange > 0f)
            source.pitch = Random.Range(pitch - pitchRange, pitch + pitchRange);
        else
            source.pitch = pitch;

        source.Play();
    }

    public void PlaySFXAt(int clipID, Vector3 position, float volume = 1f, float pitch = 1f, float pitchRange = 0f, float spatialBlend = 1f, float minDistance = 1f)
    {
        if (clipID != -1)
            PlaySFXAt(sfxClips[clipID], position, volume, pitch, pitchRange, spatialBlend, minDistance);
    }

    public void StopSFX(int clipID)
    {
        AudioClip clip = sfxClips[clipID];
        foreach (var s in _sfxSources)
        {
            if (s.isPlaying && s.clip == clip)
            {
                s.SmoothStop();
                break;
            }
        }
    }

    public AudioClip GetSFXClip(int clipID)
    {
        return sfxClips[clipID];
    }

    public void SetTransitionToGameplay()
    {
        _gameplaySnapshot.TransitionTo(_snapshotTransition);
    }

    public void SetTransitionToMenu()
    {
        _menuSnapshot.TransitionTo(_snapshotTransition);
    }

    #region Options
    public void SetVolume(string name, float normalizedVolume)
    {
        float v = Mathf.Log10(normalizedVolume) * 20f;
        _mixer.SetFloat(name, v);
    }

    public void SetMasterVolume(float normalizedVolume)
    {
        SetVolume(MIXER_MASTER_VOLUME, normalizedVolume);
    }

    public void SetMusicVolume(float normalizedVolume)
    {
        SetVolume(MIXER_MUSIC_VOLUME, normalizedVolume);
    }

    public void SetSFXVolume(float normalizedVolume)
    {
        SetVolume(MIXER_SFX_VOLUME, normalizedVolume);
    }
    #endregion
}