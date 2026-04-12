using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mover : MonoBehaviour
{
    [SerializeField] InputActionReference movementReference;
    InputAction move;
    [SerializeField] AnimationCurve movementCurve;
    void Awake()
    {
        move = movementReference.action;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveVector = move.ReadValue<Vector2>();
        
        float strength = movementCurve.Evaluate(moveVector.magnitude);
        float division = (moveVector.magnitude > 0.01f) ? moveVector.magnitude : 1f;
        moveVector *= strength / division;
        Vector3 moveDir;
        moveDir = moveVector.y * Camera.main.transform.forward + moveVector.x * Camera.main.transform.right;
        moveDir.y = 0;
        transform.position+= moveDir*Time.deltaTime;
        
    }
}
