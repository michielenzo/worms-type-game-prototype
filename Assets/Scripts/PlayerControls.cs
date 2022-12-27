
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

    [SerializeField] 
    private float movementSpeed = 15;

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
        Move();
    }

    private void ToggleCursor()
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void Move()
    {
        Vector3 movementDirection = Vector3.zero;
        
        Vector3 orbitCameraForwardDirection = Camera.transform.forward.normalized;
        orbitCameraForwardDirection.y = 0;

        if (Input.GetKey(KeyCode.W) && IsOwner) movementDirection += orbitCameraForwardDirection;
        if (Input.GetKey(KeyCode.S) && IsOwner) movementDirection += -orbitCameraForwardDirection;
        if (Input.GetKey(KeyCode.A) && IsOwner) movementDirection +=
            Quaternion.AngleAxis(-90, Vector3.up) * orbitCameraForwardDirection;  
        if (Input.GetKey(KeyCode.D) && IsOwner) movementDirection += 
            Quaternion.AngleAxis(90, Vector3.up) * orbitCameraForwardDirection;  
        
        transform.Translate(movementDirection.normalized * (Time.deltaTime * movementSpeed));
    }
}
