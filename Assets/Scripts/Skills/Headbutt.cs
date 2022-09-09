using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tweening;

public class Headbutt : Skill
{
    [SerializeField] private float damage = 20f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private int headbuttClipId = 32;
    [SerializeField] private int headbuttHitClipId = 33;

    private float _cooldownTimer;
    private bool _attackStarted;
    private Collider _hitbox;

    public override void Init(Transform skillOrigin)
    {
        _cooldownTimer = cooldown;
        _hitbox = GetComponent<Collider>();
        _hitbox.enabled = false;
    }

    private void Update()
    {
        if (!_enable || _attackStarted)
            return;

        _cooldownTimer += Time.deltaTime;
    }

    public override void Use(bool started)
    {
        if (started)
            StartHeadbutt();
    }

    public override void UseSecondary(bool started) { }

    private void StartHeadbutt()
    {
        if (_attackStarted || _cooldownTimer < cooldown)
            return;

        _attackStarted = true;
        _hitbox.enabled = true;
        SoundManager.Instance.PlaySFXAt(headbuttClipId, transform.position, pitchRange: 0.1f);

        TweenSequence seq = Tweener.Sequence();
        seq.Append(_author.transform.DoMove(_author.transform.position + _author.transform.forward * 0.7f, 0.15f).SetEase(Ease.Linear));
        seq.Append(_author.transform.DoMove(_author.transform.position, 0.15f).SetEase(Ease.OutQuad));
        seq.AppendCallback(() => 
        { 
            _hitbox.enabled = false;
            _cooldownTimer = 0f;
            _attackStarted = false;
        });
        seq.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == _author)
            return;

        if(other.TryGetComponent(out IDamageable damageable))
        {
            damageable.Damage(damage);
            SoundManager.Instance.PlaySFXAt(headbuttHitClipId, transform.position, pitchRange: 0.05f);
        }
    }
}