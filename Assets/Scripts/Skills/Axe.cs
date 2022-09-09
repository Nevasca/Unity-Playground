using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Axe : SkillInstance
{
    [SerializeField] private float damage = 100f;
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private Transform pivotVfx;

    private Rigidbody _rb;
    private Transform _author;
    private bool _init;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!_init || _rb.isKinematic)
            return;

        _rb.MoveRotation(_rb.rotation * Quaternion.AngleAxis(360f * 4.6f * Time.deltaTime, Vector3.right));
    }

    public override void Init(Vector3 direction, Transform author)
    {
        _init = true;
        _author = author;

        transform.eulerAngles = new Vector3(0f, _author.eulerAngles.y, 0f);
        _rb.isKinematic = false;
        _rb.velocity = transform.forward * 3f + transform.up * InitialSpeed();

        GetComponent<CinemachineImpulseSource>().GenerateImpulse(Camera.main.transform.forward);
        GetComponent<AudioSource>().PlayPitchRange(0.03f);
        SoundManager.Instance.PlaySFXAt(17, transform.position, pitchRange: 0.05f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_init)
            return;        

        if (other.transform == _author)
            return;

        if (other.isTrigger)
            return;

        if (other.TryGetComponent(out IDamageable damageable))
            damageable.Damage(damage);

        _init = false;
        _rb.isKinematic = true;
        Instantiate(hitVfx, pivotVfx.position, Quaternion.identity);
        transform.parent = other.transform;
        GetComponent<AudioSource>().SmoothStop();
        GetComponentInChildren<GhostEffect>().DisableGhost();
        SoundManager.Instance.PlaySFXAt(16, transform.position, pitch: 0.8f, pitchRange: 0.05f);
    }

    private float InitialSpeed()
    {
        float d = 3f;
        float a = 45f;
        float y = transform.position.y;
        float cos = Mathf.Cos(a);

        return Mathf.Sqrt(d * d * Physics.gravity.magnitude / (d * Mathf.Sin(2 * a) - 2f * -y * cos * cos));
    }
}