using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AimState : MonoBehaviour
{
    private Transform _transformCamera;
    private Transform _transformPivot;
    
    // Aim camera
    /*public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;*/

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
    }

    public void OnUpdate()
    {
        if(!Input.GetKey(KeyCode.R)) CustomEvent.Trigger(gameObject, "EndAim");
    }

    public void OnLateUpdate()
    {
        Debug.Log("Late Update Aim");
        AimCam();
    }

    public void OnExit()
    {
        Debug.Log("Exit Aim");
    }
    
    private void AimCam()
    {
        transform.position = Vector3.Lerp(transform.position, _transformPivot.position, 12 * Time.deltaTime);

        /*yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");
        
        Debug.Log(transform.eulerAngles);
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);*/
    }
}

