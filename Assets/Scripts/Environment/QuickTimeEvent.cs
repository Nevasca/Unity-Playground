using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "QuickTimeEvent", menuName = "Events/Quick Time")]
public class QuickTimeEvent : ScriptableObject
{
    private UnityEvent _quickTimeEvent = new UnityEvent();

    public void AddListener(UnityAction action)
    {
        _quickTimeEvent.AddListener(action);
    }

    public void RemoveListener(UnityAction action)
    {
        _quickTimeEvent.RemoveListener(action);
    }

    public void Invoke()
    {
        _quickTimeEvent?.Invoke();
    }
}