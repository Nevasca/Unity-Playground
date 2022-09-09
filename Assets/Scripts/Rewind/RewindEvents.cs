using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RewindEvents : RewindObject
{
    private List<Action> _callbacksQueue;

    protected override void Awake()
    {
        base.Awake();
        _callbacksQueue = new List<Action>();
    }

    protected override PointInTime CreateTickData()
    {
        //Adiciona um ponto no tempo com callbacks aguardando tick se existirem e limpa fila
        if (_callbacksQueue.Count > 0)
        {
            PointInTimeEvent point = new PointInTimeEvent(_callbacksQueue.ToArray());
            _callbacksQueue.Clear();
            return point;
        }

        return null;
    }

    //Adiciona callback na fila para o proximo tick
    public void AddEventPoint(Action callback)
    {
        _callbacksQueue.Add(callback);
    }

    protected override void Rewind()
    {
        if (!HasPointInTime())
            return;

        PointInTimeEvent point = (PointInTimeEvent)PopPointInTime();
        if(point != null)
            foreach (Action callback in point.callbacks)
                callback.Invoke();
    }

    protected override void StartRewind() { }
    protected override void StopRewind() { }
}