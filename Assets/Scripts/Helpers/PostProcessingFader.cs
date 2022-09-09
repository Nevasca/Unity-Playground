using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Tweening;

public class PostProcessingFader : MonoBehaviour
{
    [SerializeField] private float _fadeDuration = 0.2f;

    public void SetFadeDuration(float duration)
    {
        _fadeDuration = duration;
    }

    public void FadeInVolume(Volume volume)
    {
        volume.DoWeight(1f, _fadeDuration);
    }

    public void FadeOutVolume(Volume volume)
    {
        volume.DoWeight(0f, _fadeDuration);
    }
}