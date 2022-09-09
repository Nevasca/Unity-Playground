using System.Collections;
using UnityEngine;

public class OptionAudioVolume : MonoBehaviour
{
    [SerializeField] private float _maxValue = 1f;

    #region Slider OnChange
    public void UpdateMasterVolume(float value)
    {
        SoundManager.Instance.SetMasterVolume(GetNormalizeValue(value));
    }

    public void UpdateMusicVolume(float value)
    {
        SoundManager.Instance.SetMusicVolume(GetNormalizeValue(value));
    }

    public void UpdateSFXVolume(float value)
    {
        SoundManager.Instance.SetSFXVolume(GetNormalizeValue(value));
    }
    #endregion

    private float GetNormalizeValue(float value)
    {
        float normalized = value / _maxValue;
        //If clamped from 0 makes audiomixer set as maximum value
        normalized = Mathf.Clamp(normalized, 0.0001f, 1f);
        return normalized;
    }
}