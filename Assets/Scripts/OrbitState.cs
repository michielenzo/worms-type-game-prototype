using Unity.VisualScripting;
using UnityEngine;

public class OrbitState : MonoBehaviour
{
    private Transform _transformCamera;
    private Transform _transformPivot;
    
    private Vector3 _localRotation;
    private Vector3 _savedEulers;
    
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

    private CameraConfig _config;
    
    private void Start()
    {
        _config = GetComponent<CameraConfig>();
        
        _transformPivot = transform.parent;

        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void OnEnter()
    {
        Debug.Log("Enter Orbit");
        if (_transformCamera == null) _transformCamera = transform;
        _transformCamera.eulerAngles = _savedEulers;
    }

    public void OnUpdate()
    {
        if(Input.GetKeyDown(KeyCode.R)) CustomEvent.Trigger(gameObject, "StartAim");
        Debug.Log("pivot: "+ _transformPivot.eulerAngles.y);  
    }

    public void OnLateUpdate()
    {
        Orbit();
    }

    public void OnExit()
    {
        Debug.Log("Exit Orbit");
        _savedEulers = _transformCamera.eulerAngles;
    }
    
    private void Orbit()
    {
        if (!cameraDisabled)
        {
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
