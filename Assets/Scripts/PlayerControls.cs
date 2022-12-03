
using Unity.Netcode;
using UnityEngine;

public class PlayerControls : NetworkBehaviour
{
    [SerializeField]
    private GameObject orbitCameraPrefab;

    private GameObject _orbitCamera;

    public override void OnNetworkSpawn()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        if(IsOwner) {
            // Instantiate the camera and assign the player as its follow target
            _orbitCamera = _orbitCamera = Instantiate(orbitCameraPrefab, Vector3.zero, Quaternion.identity);
            _orbitCamera.GetComponentInChildren<CameraOrbit>().followTarget = gameObject;
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
        Vector3 orbitCameraForwardDirection = _orbitCamera.transform.forward;
        orbitCameraForwardDirection.y = 0;
        transform.Translate(orbitCameraForwardDirection * (Time.deltaTime * 10));
    }
}
