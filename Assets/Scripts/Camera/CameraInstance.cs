using UnityEngine;
using Cinemachine;

public class CameraInstance : MonoBehaviour
{
    public CameraType cameraType;
    public bool setOnStart = false;

    private void Start()
    {
        if (!setOnStart)
            return;

        CameraManager.Instance.AddCamera(cameraType, 
            GetComponent<CinemachineVirtualCameraBase>());        
    }
}