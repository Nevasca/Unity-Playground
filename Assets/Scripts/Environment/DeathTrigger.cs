using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamageable damageable))
        {
            SoundManager.Instance.PlaySFXAt(20, transform.position);
            damageable.Damage(99999);
        }
    }
}