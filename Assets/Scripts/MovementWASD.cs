using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementWASD : MonoBehaviour
{
    [SerializeField]
    private float speed = 4;

    private Vector3 _movementDirection;
    
    void Update()
    {
       Move(); 
    }
    
    private void Move()
    {
        CalculateDirection();
        transform.Translate(_movementDirection * (speed * Time.deltaTime));       
    }

    private void CalculateDirection()
    {
        _movementDirection = Vector3.zero;
        
        // My keyboard has 3 N key rollover meaning that only 3 keys pressed are registered at the same time.
        if(Input.GetKey(KeyCode.W)) _movementDirection += Vector3.forward;
        if(Input.GetKey(KeyCode.S)) _movementDirection += Vector3.back;
        if(Input.GetKey(KeyCode.A)) _movementDirection += Vector3.left;
        if(Input.GetKey(KeyCode.D)) _movementDirection += Vector3.right;

        _movementDirection = _movementDirection.normalized;
    }
}
