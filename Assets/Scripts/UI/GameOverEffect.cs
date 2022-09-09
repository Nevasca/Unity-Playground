using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Cinemachine;
using Tweening;

using Random = UnityEngine.Random;

public class GameOverEffect : MonoBehaviour
{
    [Header("Post Processing")]
    [SerializeField] private Volume volumeStart;
    [SerializeField] private Volume volumeMiddle;
    [SerializeField] private Volume volumeEnd;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera deathCamera;

    [Header("UI")]
    [SerializeField] private GameObject gameOverText;

    private CinemachineFramingTransposer _cinemachineFraming;

    private void Start()
    {
        _cinemachineFraming = (CinemachineFramingTransposer) deathCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);        
    }

    public void StartEffect(Action callbackComplete)
    {
        if(_cinemachineFraming == null)
            _cinemachineFraming = (CinemachineFramingTransposer)deathCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);

        RewindManager.Instance.AddEvent(CancelEffect);
        SoundManager.Instance.PlaySFX(18, 0.5f);
        _cinemachineFraming.m_CameraDistance = 4f;

        Vector3 cameraEuler = Camera.main.transform.eulerAngles;
        deathCamera.transform.eulerAngles = cameraEuler;
        deathCamera.gameObject.SetActive(true);

        volumeStart.weight = 1f;
        volumeMiddle.weight = 1f;
        Time.timeScale = 0.3f;

        TweenSequence seq = Tweener.Sequence();
        seq.Append(volumeStart.DoWeight(0f, 1f));
        seq.AppendInterval(1.5f);
        seq.AppendCallback(() => {
            gameOverText.SetActive(true);
            volumeEnd.weight = 1f;
            volumeStart.weight = 1f;
            _cinemachineFraming.m_CameraDistance = 2f;

            int[] clips = new int[] { 19, 46 };
            int randomClip = clips[Random.Range(0, clips.Length)];
            SoundManager.Instance.PlaySFX(randomClip);

            Vector3 euler = deathCamera.transform.eulerAngles;
            deathCamera.transform.eulerAngles = new Vector3(45f, euler.y, euler.z);
            volumeStart.weight = 1f;
        });
        seq.Append(TweenVirtual.DoFloat(2f, 7f, 4f, DOCameraDistance));
        seq.Join(volumeStart.DoWeight(0f, 0.6f));

        seq.AppendCallback(() => callbackComplete?.Invoke());

        seq.Play();
    }

    private void DOCameraDistance(float x)
    {
        _cinemachineFraming.m_CameraDistance = x;
    }

    private void CancelEffect()
    {
        Tweener.KillAll();
        gameOverText.SetActive(false);
        volumeStart.weight = 0f;
        volumeMiddle.weight = 0f;
        volumeEnd.weight = 0f;

        SoundManager.Instance.StopSFX(18);
        SoundManager.Instance.StopSFX(19);

        deathCamera.gameObject.SetActive(false);
        _cinemachineFraming.m_CameraDistance = 4f;
    }
}