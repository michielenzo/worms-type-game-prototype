using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class PlayerControls : NetworkBehaviour
{
    [SerializeField]
    private GameObject orbitCameraPrefab;

    public override void OnNetworkSpawn()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        if(IsOwner) {
            // Instantiate the camera and assign the player as its follow target
            GameObject orbitCamera = Instantiate(orbitCameraPrefab, Vector3.zero, Quaternion.identity);
            orbitCamera.GetComponentInChildren<CameraOrbit>().followTarget = gameObject;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && IsOwner) ToggleCursor();
    }

    private void ToggleCursor()
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
