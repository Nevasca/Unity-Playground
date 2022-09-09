using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasLookAtCamera : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private Transform _camTransform;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _camTransform = Camera.main.transform;

        //LookAt deixa o objeto invertido horizontalmente
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    private void Update()
    {
        if (_canvasGroup.alpha == 0f)
            return;

        transform.LookAt(_camTransform);
    }
}