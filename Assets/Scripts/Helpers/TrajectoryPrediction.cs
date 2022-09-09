using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryPrediction : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _smoothIncrement = 0.2f;
    [SerializeField] private float _maxTime = 10f;

    private Vector3 _origin;
    private Vector3 _velocity;
    private List<Vector3> _samples = new List<Vector3>();
    private Vector3 _acceleration;

    private Vector3 _predPosition;
    private float _t;

    private void Awake()
    {
        _acceleration = Physics.gravity;
    }

    public void SetTrajectoryColor(Color color)
    {
        _lineRenderer.material.SetColor("_BaseColor", color);
    }

    public void ShowTrajectory(Vector3 origin, Vector3 velocity)
    {
        _origin = origin;
        _velocity = velocity;

        _samples.Clear();

        _t = 0f;

        while(_t < _maxTime)
        {
            _predPosition = GetPositionAt(_t);
            _samples.Add(_predPosition);

            _t += _smoothIncrement;
        }

        _lineRenderer.positionCount = _samples.Count;
        _lineRenderer.SetPositions(_samples.ToArray());
    }

    public void HideTrajectory()
    {
        _lineRenderer.positionCount = 0;
    }

    private Vector3 GetPositionAt(float time)
    {
        //S = So + VoT + at²/2
        return _origin + _velocity * time + _acceleration * (time * time) / 2f;
    }

    private void Reset()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }
}