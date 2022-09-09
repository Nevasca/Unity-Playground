using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitSound : MonoBehaviour
{
    [SerializeField] private AudioClip _clip;

    private bool _clipPlayed;

    private void OnEnable()
    {
        Application.wantsToQuit += CheckQuitSound;
    }

    private bool CheckQuitSound()
    {
        if(!_clipPlayed)
            StartCoroutine(PlaySoundCoroutine());

        return _clipPlayed;
    }

    private IEnumerator PlaySoundCoroutine()
    {
        SoundManager.Instance.PlaySFX(_clip);

        yield return new WaitForSecondsRealtime(_clip.length);

        _clipPlayed = true;
        Application.Quit();
    }

    private void OnDisable()
    {
        Application.wantsToQuit -= CheckQuitSound;
    }
}