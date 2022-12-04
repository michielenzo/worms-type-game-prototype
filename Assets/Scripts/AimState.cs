using Unity.VisualScripting;
using UnityEngine;

public class AimState : MonoBehaviour
{
    private Transform _transformCamera;
    private Transform _transformPivot;
    
    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float _yaw;
    private float _pitch;

    private CameraConfig _config;

    private void Start()
    {
        _config = GetComponent<CameraConfig>();
        
        var trans = transform;
        _transformCamera = trans;
        _transformPivot = trans.parent;
    }

    public void OnEnter()
    {
        Debug.Log("Enter Aim");
        _yaw = _transformPivot.eulerAngles.y;
        _pitch = 0f;
    }

    public void OnUpdate()
    {
        if(!Input.GetKey(KeyCode.R)) CustomEvent.Trigger(gameObject, "EndAim");
    }

    public void OnLateUpdate()
    {
        AimCam();
    }

    public void OnExit()
    {
        Debug.Log("Exit Aim");
    }
    
    private void AimCam()
    {
        _transformCamera.position = Vector3.Lerp(
            _transformCamera.position, 
            _transformPivot.position, 
            12 * Time.deltaTime);

        _yaw += speedH * Input.GetAxis("Mouse X");
        _pitch -= speedV * Input.GetAxis("Mouse Y");

        Quaternion quaternion = Quaternion.Euler(_pitch, _yaw, 0);
        _transformCamera.rotation = quaternion;
    }
}

