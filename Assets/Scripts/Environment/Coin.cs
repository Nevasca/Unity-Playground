using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tweening;

public class Coin : MonoBehaviour
{
    private bool _collected;

    private void OnTriggerEnter(Collider other)
    {
        if (_collected)
            return;

        if(other.CompareTag("Player"))
        {
            _collected = true;
            SoundManager.Instance.PlaySFXAt(44, transform.position, minDistance: 1.5f);
            transform.DoMoveY(transform.position.y + 0.2f, 0.1f).OnComplete(() => Destroy(gameObject));
        }
    }
}