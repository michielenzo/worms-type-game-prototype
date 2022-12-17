
using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerControls : NetworkBehaviour
{
    [SerializeField]
    private GameObject cameraPrefab;
    [SerializeField]
    public GameObject ballisticMissilePrefab;
    [NonSerialized]
    public GameObject Camera;

    public override void OnNetworkSpawn()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        if(IsOwner) {
            // Instantiate the camera and assign the player as its follow target
            Camera = Instantiate(cameraPrefab, Vector3.zero, Quaternion.identity);
            Camera.GetComponentInChildren<CameraConfig>().followTarget = gameObject;
            CustomEvent.Trigger(gameObject, "StartMachine");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && IsOwner) ToggleCursor();
        if(Input.GetKey(KeyCode.W) && IsOwner) Move();
    }

    private void ToggleCursor()
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void Move()
    {
        Vector3 orbitCameraForwardDirection = Camera.transform.forward;
        orbitCameraForwardDirection.y = 0;
        transform.Translate(orbitCameraForwardDirection.normalized * (Time.deltaTime * 10));
    }
}
