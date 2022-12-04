using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class AimState : NetworkBehaviour
{
    private PlayerControls _playerControls;
    private GameObject _playerCamera;
    
    private Transform _transformCamera;
    private Transform _transformPivot;
    
    [SerializeField]
    private float yawSensitivity = 2.0f;
    [SerializeField]
    public float pitchSensitivity = 2.0f;

    private float _yaw;
    private float _pitch;

    private CameraConfig _config;

    private void Start()
    {
        _config = GetComponent<CameraConfig>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public void OnEnter()
    {
        Debug.Log("Enter Aim");
        if(_playerControls == null) _playerControls = GetComponent<PlayerControls>();
        if(_playerCamera == null) _playerCamera = _playerControls.Camera;
        if(_transformPivot == null) _transformPivot = _playerCamera.transform;
        if (_transformCamera == null) _transformCamera = _playerCamera.transform.Find("Camera");
        
        _yaw = _transformPivot.eulerAngles.y;
        _pitch = 0f;
    }

    public void OnUpdate()
    {
        if(!Input.GetKey(KeyCode.R)) CustomEvent.Trigger(gameObject, "EndAim");
        if (Input.GetKeyDown(KeyCode.Mouse0)) FireBallisticMissile();
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

        _yaw += yawSensitivity * Input.GetAxis("Mouse X");
        _pitch -= pitchSensitivity * Input.GetAxis("Mouse Y");

        Quaternion quaternion = Quaternion.Euler(_pitch, _yaw, 0);
        _transformCamera.rotation = quaternion;
    }
    
    private void FireBallisticMissile()
    {
        Vector3 rotation = _transformCamera.eulerAngles; rotation.x += 90;
        Vector3 cameraForward = _transformCamera.forward;
        Vector3 startingPosition = _transformCamera.position += cameraForward * 5;

        SpawnBallisticMissileOnNetworkServerRpc(startingPosition, rotation, cameraForward);
    }

    [ServerRpc]
    private void SpawnBallisticMissileOnNetworkServerRpc(Vector3 startingPosition, Vector3 rotation, Vector3 direction)
    {
        if(_playerControls == null) _playerControls = GetComponent<PlayerControls>();
        GameObject missile = Instantiate(
            _playerControls.ballisticMissilePrefab, 
            startingPosition, 
            Quaternion.Euler(rotation));
        missile.GetComponent<NetworkObject>().Spawn();
        missile.transform.GetComponent<Rigidbody>().AddForce(direction * 2000);        
    }
}

