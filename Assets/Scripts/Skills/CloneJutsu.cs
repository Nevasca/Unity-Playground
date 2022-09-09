using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneJutsu : Skill
{
    [SerializeField] private SkillInstance clone;
    [SerializeField] private int totalClones = 3;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private bool randomHeightSpawn = false;
    [SerializeField] private float _minRandomInterval = 0.1f;
    [SerializeField] private float _maxRandomInterval = 0.2f;

    private Animator _animator;
    private float _cooldownTimer;
    private bool _attackStarted = false;

    protected override void Awake()
    {
        base.Awake();

        if (clone == null)
            clone = _author.GetComponent<SkillInstance>();
        _animator = _author.GetComponent<Animator>();
    }

    public override void Init(Transform skillOrigin) 
    {
        _cooldownTimer = cooldown;
    }

    private void Update()
    {
        if (!_enable)
            return;

        if (!_attackStarted)
            _cooldownTimer += Time.deltaTime;
    }

    public override void Use(bool started)
    {
        if (started)
            StartCast();
    }

    public override void UseSecondary(bool started) { }

    private void StartCast()
    {
        if (_attackStarted || _cooldownTimer < cooldown)
            return;

        _attackStarted = true;
        _animator.SetTrigger("cast");
        SoundManager.Instance.PlaySFXAt(22, transform.position, minDistance: 2f);
    }


    public override void EventSkillCast()
    {
        StartCoroutine(SpawnClones());
    }

    IEnumerator SpawnClones()
    {
        int spawned = 0;
        Vector3 basePosition = transform.position + transform.forward * 2f;
        //Vector3 basePosition = new Vector3(7f, 0.06f, -17f);
        Vector3 randomPosition;
        float maxRandomRange = totalClones * 0.08f;
        maxRandomRange = Mathf.Clamp(maxRandomRange, 2f, 8f);
        float minRandomRange = maxRandomRange * -1f;

        while(spawned < totalClones)
        {
            randomPosition = basePosition + new Vector3(Random.Range(minRandomRange, maxRandomRange), randomHeightSpawn ? Random.Range(0f, maxRandomRange) : 0f, Random.Range(minRandomRange, maxRandomRange));

            SkillInstance instance = Instantiate(clone, randomPosition, Quaternion.identity);
            instance.Init(transform.forward, _author);
            spawned++;

            yield return new WaitForSeconds(Random.Range(_minRandomInterval, _maxRandomInterval));
        }

        _cooldownTimer = 0f;
        _attackStarted = false;
    }
}