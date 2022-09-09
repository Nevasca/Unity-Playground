using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nixtor.AI;

public class HealGrenade : ThrowHandler
{
    [Header("Heal")]
    [SerializeField] private float _healPerSecond = 10f;
    [SerializeField] private float _areaRadius = 3f;
    [SerializeField] private float _duration = 3f;
    [SerializeField] private HealArea _healAreaPrefab;

    private AICategory _category;

    public override void Init(Vector3 force, Transform author)
    {
        _category = author.GetComponent<IDamageable>().GetCategory();

        base.Init(force, author);
    }

    private void CreateHealArea()
    {
        HealArea healArea = Instantiate(_healAreaPrefab, transform.position, Quaternion.identity);
        healArea.Init(_healPerSecond, _areaRadius, _duration, _category);

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_init)
            return;

        CreateHealArea();
    }
}