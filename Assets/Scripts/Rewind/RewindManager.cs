using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using Tweening;

public class RewindManager : MonoBehaviour
{
    [SerializeField] private float maxRewindTime = 10f;
    [SerializeField] private float rewindSpeed = 4f;
    [SerializeField] private Volume rewindEffect;

    public static event Action OnTick;
    public static event Action OnStartRewind;
    public static event Action OnRewind;
    public static event Action OnLateRewind;
    public static event Action OnStopRewind;

    private AudioSource _audio;
    private int _rewindablesInScene;
    private int _cantRewind;
    private RewindEvents _rewindEvents;

    public bool IsRewinding { get; private set; }
    public float MaxRewindTime { get { return maxRewindTime; } }
    public float RewindTimeScale { get; private set; } = 1f;
    public float MaxStackSize { get; private set; }

    public static RewindManager Instance;

    private void Awake()
    {
        Instance = this;
        _audio = GetComponent<AudioSource>();
        _rewindEvents = GetComponent<RewindEvents>();
    }

    private void FixedUpdate()
    {
        MaxStackSize = Mathf.Round(RewindManager.Instance.MaxRewindTime / Time.fixedDeltaTime);

        if (IsRewinding)
        {
            if (CanRewind())
            {
                OnRewind?.Invoke();
                OnLateRewind?.Invoke();
            }
            else
            {
                StopRewind();
            }
        }
        else
        {
            OnTick?.Invoke();
        }
    }

    public void AddEvent(Action callback)
    {
        _rewindEvents.AddEventPoint(callback);
    }

    public void AddRewindable()
    {
        _rewindablesInScene++;
    }

    public void RemoveRewindable()
    {
        _rewindablesInScene--;
    }

    //Objetos que nao possuem mais informacoes de voltar
    public void AddCantRewind()
    {
        _cantRewind++;
    }

    public bool CanRewind()
    {
        return _cantRewind < _rewindablesInScene;
    }

    public void StartRewind()
    {
        if (IsRewinding)
            return;

        OnStartRewind?.Invoke();
        _cantRewind = 0;
        RewindTimeScale = -1f;
        IsRewinding = true;
        Time.timeScale = rewindSpeed;
        _audio.Play();
        SoundManager.Instance.PlaySFX(11, 0.2f, pitch: 0.7f);
        TweenVirtual.DoFloat(rewindEffect.weight, 1f, 0.15f, DOVolumeWeight);
    }

    public void StopRewind()
    {
        if (!IsRewinding)
            return;

        IsRewinding = false;
        RewindTimeScale = 1f;
        _audio.Stop();
        SoundManager.Instance.PlaySFX(11, 0.2f, pitch: 0.5f);
        Time.timeScale = 1f;
        TweenVirtual.DoFloat(rewindEffect.weight, 0f, 0.15f, DOVolumeWeight);
        OnStopRewind?.Invoke();
    }

    private void DOVolumeWeight(float x)
    {
        rewindEffect.weight = x;
    }
}