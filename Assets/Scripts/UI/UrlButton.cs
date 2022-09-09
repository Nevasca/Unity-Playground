using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UrlButton : MonoBehaviour
{
    [SerializeField] private string _url;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OpenURL);
    }

    private void OpenURL()
    {
        Application.OpenURL(_url);
    }
}