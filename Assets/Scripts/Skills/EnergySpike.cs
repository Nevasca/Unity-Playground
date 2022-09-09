using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class EnergySpike : SkillInstance
{
    [SerializeField] private float damage = 50f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifeTime = 5f;

    [Header("Explosion")]
    [SerializeField] private float explosionPower;
    [SerializeField] private float explosionRadius;
    [SerializeField] private GameObject explosionVFX;

    [Header("Secondary Explosions")]
    [SerializeField] private SkillInstance secondaryInstance;
    [SerializeField] private float minTimeStronger = 2f;
    [SerializeField] private Transform raycastTest;

    private float _timer;
    private Transform _author; 
    private bool _enable = false;
    private bool _heldEnough;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (!_heldEnough && _timer >= minTimeStronger)
            EnableSecondaryExplosions();

        if (!_enable)
            return;

        //Controls lifetime
        if(_timer >= lifeTime)
            Explode();
    }

    public override void Init(Vector3 direction, Transform author)
    {
        //Enables movement
        _author = author;
        rb.velocity = direction * speed;
        rb.isKinematic = false;
        _timer = 0f;
        _enable = true;

        //Throw effects
        GetComponent<CinemachineImpulseSource>().GenerateImpulse(Camera.main.transform.forward);
        SoundManager.Instance.PlaySFXAt(8, transform.position, 1f, pitchRange: 0.2f);
    }

    private void EnableSecondaryExplosions()
    {
        _heldEnough = true;

        if (secondaryInstance == null)
            return;

        SoundManager.Instance.PlaySFXAt(37, transform.position, 0.6f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_enable)
            return;

        if (other.isTrigger)
            return;

        //Prevents exploding in case it hits caster on throw
        if (other.transform == _author)
            return;

        Explode();
        SpawnComplementaries();
    }

    private void Explode()
    {
        Vector3 explosionPosition = transform.position;

        //Explosion effects
        SoundManager.Instance.PlaySFXAt(9, transform.position, pitchRange: 0.1f, minDistance: 4f);
        Instantiate(explosionVFX, explosionPosition + new Vector3(0f, 0.2f, 0f), Quaternion.identity);

        //Adds explosion force to rigibody's in range
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);
        foreach(Collider c in colliders)
        {            
            if (c.TryGetComponent(out Rigidbody rb))
                rb.AddExplosionForce(explosionPower, explosionPosition, explosionRadius, 3f);

            if (c.TryGetComponent(out IDamageable damageable))
                damageable.Damage(damage);
        }

        gameObject.SetActive(false);
    }

    private void SpawnComplementaries()
    {
        if (!_heldEnough || secondaryInstance == null)
            return;

        Vector3 spawnPosition;

        if (Physics.Raycast(raycastTest.position, transform.position - raycastTest.position, out RaycastHit hit, 20f))
        {
            Debug.DrawLine(raycastTest.position, hit.point, Color.red, 3f);
            spawnPosition = hit.point + hit.normal.normalized * 0.2f;
            raycastTest.position = hit.point;
            raycastTest.up = hit.normal;
        }
        else
        {
            spawnPosition = transform.position + new Vector3(0f, 1f, 0f);
            raycastTest.position = spawnPosition;
            raycastTest.up = Vector3.up;
        }

        Vector3[] directions = new Vector3[]
        {
            new Vector3(1f, 0.5f, 0f),
            new Vector3(-1f, 0.5f, 0f),
            new Vector3(0f, 0.5f, 1f),
            new Vector3(0f, 0.5f, -1f),
            new Vector3(1f, 0.5f, 1f),
            new Vector3(-1f, 0.5f, -1f),
            new Vector3(-1f, 0.5f, 1f),
            new Vector3(1f, 0.5f, -1f),
        };

        float randomFactor;
        foreach (var direction in directions)
        {
            randomFactor = Random.Range(0.3f, 1.3f);
            Instantiate(secondaryInstance, spawnPosition, Quaternion.identity)
                .Init(raycastTest.TransformDirection(direction * randomFactor), _author);
        }
    }
}