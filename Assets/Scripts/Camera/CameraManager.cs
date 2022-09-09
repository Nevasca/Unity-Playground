using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform rootCameraList;
    [SerializeField] private Animator _animator;

    [Header("Cutscenes")]
    [SerializeField] private PlayableDirector _playableDirector;

    public PlayableDirector Director => _playableDirector;

    private CinemachineBrain _cinemachineBrain;
    private Transform _mainCamera;
    private Dictionary<CameraType, CinemachineVirtualCameraBase> _cameras;

    public static CameraManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _mainCamera = Camera.main.transform;
        _cinemachineBrain = _mainCamera.GetComponent<CinemachineBrain>();

        _cameras = new Dictionary<CameraType, CinemachineVirtualCameraBase>();
        GetCamerasFromRoot();
    }

    private void GetCamerasFromRoot()
    {
        foreach(Transform camera in rootCameraList)
        {
            if(camera.TryGetComponent(out CameraInstance cameraInstance))
            {
                AddCamera(cameraInstance.cameraType,
                    cameraInstance.GetComponent<CinemachineVirtualCameraBase>());
            }
        }
    }

    public void AddCamera(CameraType camera, CinemachineVirtualCameraBase virtualCamera)
    {
        if (_cameras.ContainsKey(camera))
            return;

        _cameras.Add(camera, virtualCamera);
    }

    public void IgnoreTimeScale(bool value)
    {
        _cinemachineBrain.m_IgnoreTimeScale = value;
    }

    public void EnableCamera(CameraType camera, Transform follow = null, Transform lookAt = null)
    {
        var virtualCamera = _cameras[camera];

        if (follow != null)
            virtualCamera.Follow = follow;

        if (lookAt != null)
            virtualCamera.LookAt = lookAt;

        virtualCamera.gameObject.SetActive(true);
    }

    public void DisableCamera(CameraType camera)
    {
        _cameras[camera].gameObject.SetActive(false);
    }

    public T GetCinemachineComponet<T>(CameraType camera, CinemachineCore.Stage stage) where T : CinemachineComponentBase
    {
        CinemachineVirtualCamera virtualCamera = (CinemachineVirtualCamera) _cameras[camera];

        return (T) virtualCamera.GetCinemachineComponent(stage);
    }

    public void SetAnimatorBool(string name, bool value)
    {
        _animator.SetBool(name, value);
    }
}