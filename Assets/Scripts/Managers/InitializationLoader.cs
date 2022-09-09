using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitializationLoader : MonoBehaviour
{
    [SerializeField] private AssetReference _sceneReference;
    [SerializeField] private Slider _progressSlider;

    private IEnumerator Start()
    {
        var op = _sceneReference.LoadSceneAsync(LoadSceneMode.Single, true);
        
        while(!op.IsDone)
        {
            _progressSlider.value = op.PercentComplete;
            yield return null;
        }

        _progressSlider.value = op.PercentComplete;
    }
}