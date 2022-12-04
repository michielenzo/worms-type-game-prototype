
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerControls : NetworkBehaviour
{
    [SerializeField]
    private GameObject cameraPrefab;
    [SerializeField]
    private GameObject ballisticMissilePrefab;

    private GameObject _camera;

    public override void OnNetworkSpawn()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        if(IsOwner) {
            // Instantiate the camera and assign the player as its follow target
            _camera = _camera = Instantiate(cameraPrefab, Vector3.zero, Quaternion.identity);
            _camera.GetComponentInChildren<CameraConfig>().followTarget = gameObject;
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
        Vector3 orbitCameraForwardDirection = _camera.transform.forward;
        orbitCameraForwardDirection.y = 0;
        transform.Translate(orbitCameraForwardDirection * (Time.deltaTime * 10));
    }
}
