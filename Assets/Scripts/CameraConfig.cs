using UnityEngine;
using UnityEngine.Serialization;

public class CameraConfig : MonoBehaviour
{
    private Transform _transformCamera;
    private Transform _transformPivot;

    [SerializeField]
    public GameObject followTarget;
    
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
}
