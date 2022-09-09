using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameOverEffect gameOverEffect;
    [SerializeField] private AssetReference _sceneReference;

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;

        PauseMenu.OnPaused += OnPaused;
    }

    private void OnPaused(bool paused)
    {
        Time.timeScale = paused ? 0f : 1f;
    }

    public void OnPlayerDead()
    {
        gameOverEffect.StartEffect(RestartScene);
    }

    private void RestartScene()
    {
        Time.timeScale = 1f;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        _sceneReference.LoadSceneAsync();
    }

    private void OnDestroy()
    {
        PauseMenu.OnPaused -= OnPaused;
    }
}