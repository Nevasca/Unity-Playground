using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    protected bool _enable = true;
    protected Transform _author;
    [SerializeField] protected bool usingSecondary = false;
    [SerializeField] protected Sprite _icon;

    public bool UsingSecondary => usingSecondary;
    public Sprite Icon => _icon;

    protected virtual void Awake()
    {
        if(_author == null)
            _author = transform.parent;
    }

    public virtual void Init(Transform skillOrigin)
    {
        _author = transform.parent;
    }

    public abstract void Use(bool started);
    public abstract void UseSecondary(bool started);

    public virtual void UseTertiary(bool started) { }

    public void Enable(bool enable)
    {
        _enable = enable;
    }

    public virtual void EventSkillCast() { }
    public virtual void EventSkillEnded() { }
}