using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class RewindObject : MonoBehaviour
{
    private List<PointInTime> _pointsInTime;

    protected abstract PointInTime CreateTickData();

    protected abstract void StartRewind();
    protected abstract void StopRewind();
    protected abstract void Rewind();

    protected virtual void Awake()
    {
        _pointsInTime = new List<PointInTime>();
    }

    protected virtual void Start()
    {
        RewindManager.OnTick += Tick;
        RewindManager.OnStartRewind += StartRewind;
        RewindManager.OnRewind += Rewind;
        RewindManager.OnLateRewind += LateRewind;
        RewindManager.OnStopRewind += StopRewind;
        RewindManager.Instance.AddRewindable();
    }

    private void Tick()
    {
        AddPointInTime(CreateTickData());
    }

    protected virtual void AddPointInTime(PointInTime point)
    {
        if (_pointsInTime.Count > RewindManager.Instance.MaxStackSize)
            _pointsInTime.RemoveAt(_pointsInTime.Count - 1);

        _pointsInTime.Insert(0, point);
    }

    private void LateRewind()
    {
        //Se o jogo ainda pode voltar no tempo mas o objeto nao,
        //significa que foi instanciado nesse ponto, entao destruir
        if (!HasPointInTime() && RewindManager.Instance.CanRewind())
            Destroy(gameObject);
    }

    protected PointInTime PopPointInTime()
    {
        PointInTime point = _pointsInTime[0];
        _pointsInTime.RemoveAt(0);

        //Caso era o ultimo ponto na lista, informar que objeto voltou tudo que podia
        if(_pointsInTime.Count == 0)
            RewindManager.Instance.AddCantRewind();

        return point;
    }

    protected bool HasPointInTime()
    {
        return _pointsInTime.Count > 0;
    }

    protected virtual void OnDestroy()
    {
        RewindManager.OnTick -= Tick;
        RewindManager.OnStartRewind -= StartRewind;
        RewindManager.OnRewind -= Rewind;
        RewindManager.OnLateRewind -= LateRewind;
        RewindManager.OnStopRewind -= StopRewind;
        RewindManager.Instance.RemoveRewindable();
    }
}