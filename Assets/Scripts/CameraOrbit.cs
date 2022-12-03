using UnityEngine;
using UnityEngine.Serialization;

public class CameraOrbit : MonoBehaviour
{
    private Transform _transformCamera;
    private Transform _transformPivot;
    
    [SerializeField]
    public GameObject followTarget;

    // Orbit camera
    private Vector3 _localRotation;
    
    [SerializeField]
    private float cameraDistance = 10f;
    [SerializeField]
    private float mouseSensitivity = 4f;
    [SerializeField]
    private float scrollSensitivity = 2f;
    [SerializeField]
    private float orbitDampening = 10f;
    [SerializeField]
    private float scrollDampening = 6f;
    [SerializeField]
    private bool cameraDisabled;

    // Aim camera
    /*public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;*/

    private void Start()
    {
        var trans = transform;
        _transformCamera = trans;
        _transformPivot = trans.parent;
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (followTarget != null) _transformPivot.position = followTarget.transform.position;
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) cameraDisabled = !cameraDisabled;
        if (Input.GetKey(KeyCode.R)) AimCam();
        else Orbit();
    }

    private void AimCam()
    {
        transform.position = Vector3.Lerp(transform.position, _transformPivot.position, 12 * Time.deltaTime);

        /*yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");
        
        Debug.Log(transform.eulerAngles);
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);*/
    }

    private void Orbit()
    {
        if (!cameraDisabled)
        {
            Debug.Log(transform.eulerAngles);
            
            //Rotation of the camera based on Mouse Coordinates.
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                _localRotation.x += Input.GetAxis("Mouse X") * mouseSensitivity;
                _localRotation.y -= Input.GetAxis("Mouse Y") * mouseSensitivity;
                
                //Clamp the rotation to the horizon and so that it does not flip over at the top.
                _localRotation.y = Mathf.Clamp(_localRotation.y, 0f, 90f);
            }
            
            //Zooming input from the scrolling wheel.
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                float scrollAmount = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
                
                //Makes the camera zoom speed scale with the distance towards the pivot.
                scrollAmount *= cameraDistance * 0.3f;

                cameraDistance += scrollAmount * -1f;
                
                //Make the camera get closer than 1.5m from the pivot and not further than 100m.
                cameraDistance = Mathf.Clamp(cameraDistance, 1.5f, 100f);
            }
        }
        
        //Actual Camera Rig transformations
        Quaternion quaternion = Quaternion.Euler(_localRotation.y, _localRotation.x, 0);
        _transformPivot.rotation = Quaternion.Lerp(
            _transformPivot.rotation, 
            quaternion, 
            Time.deltaTime * orbitDampening);

        if (_transformCamera.localPosition.z != cameraDistance * -1f)
        {
            _transformCamera.localPosition = new Vector3(
                0f,
                0f,
                Mathf.Lerp(_transformCamera.localPosition.z, 
                    cameraDistance * -1f, 
                    Time.deltaTime * scrollDampening));
        }       
    }
}
