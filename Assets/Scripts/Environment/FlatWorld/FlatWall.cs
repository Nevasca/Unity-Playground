using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlatWall : MonoBehaviour
{
    [SerializeField] private Transform _startFlatPoint;
    [SerializeField] private Transform _exitPoint;
    [SerializeField] private PlayerStickerMovement _sticker;
    [SerializeField] private Transform _originCorner;
    [SerializeField] private Transform _targetCorner;

    private List<WallPoint> _corners;
    private GameObject _playerEntered;
    private FlatCheckpoint _lastCheckpoint;

    public Transform StartPoint => _lastCheckpoint != null ? _lastCheckpoint.StartPoint : _startFlatPoint;
    public List<WallPoint> Corners => _corners;

    private void Awake()
    {
        _corners = new List<WallPoint>();
        foreach (Transform t in transform)
            _corners.Add(new WallPoint(t.position, t.forward));
    }

    public void EnterWall(GameObject playerEntering)
    {
        _playerEntered = playerEntering;
        _playerEntered.gameObject.SetActive(false);
        _sticker.SetOnWall(this, _originCorner.position, _targetCorner.position);

        CameraManager.Instance.EnableCamera(CameraType.Wall, _sticker.transform, _sticker.transform);
    }

    public void ExitWall()
    {
        if (_playerEntered == null)
            return;

        CameraManager.Instance.DisableCamera(CameraType.Wall);
        _sticker.ExitWall();

        _playerEntered.transform.position = _exitPoint.position;
        _playerEntered.transform.forward = _exitPoint.forward;
        _playerEntered.SetActive(true);
        _playerEntered = null;
        _lastCheckpoint = null;
    }

    public void SetCheckpoint(FlatCheckpoint checkpoint)
    {
        _lastCheckpoint = checkpoint;
    }

    public void RestartFromLastCheckpoint()
    {
        if(_lastCheckpoint != null)
            _sticker.SetOnWall(this, _lastCheckpoint.OrginCornerPosition, _lastCheckpoint.TargetCornerPosition);
        else
            _sticker.SetOnWall(this, _originCorner.position, _targetCorner.position);

    }

    private void OnDrawGizmos()
    {
        foreach(Transform t in transform)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(t.position, 0.15f);
            Gizmos.DrawLine(t.position, t.position + t.forward * 0.2f);
        }

        if(_startFlatPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(_startFlatPoint.position, 0.15f);
        }
    }
}

public struct WallPoint
{
    public Vector3 position;
    public Vector3 normal;

    public WallPoint(Vector3 position, Vector3 normal)
    {
        this.position = position;
        this.normal = normal;
    }
}