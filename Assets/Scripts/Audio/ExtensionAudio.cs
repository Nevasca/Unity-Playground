using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tweening;

public static class ExtensionAudio
{
    public static void SmoothStop(this AudioSource audio)
    {
        float volume = audio.volume;
        audio.DoVolume(0f, 0.1f).OnComplete(() =>
        {
            audio.Stop();
            audio.volume = volume;
        });
    }

    public static void PlayPitchRange(this AudioSource audio, float pitchRange)
    {        
        audio.pitch = Random.Range(audio.pitch - pitchRange, audio.pitch + pitchRange);
        audio.Play();
    }
}