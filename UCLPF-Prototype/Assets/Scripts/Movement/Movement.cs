using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// A class that contains code for moving the subject
// should be a parent of the rig.
public class Movement : MonoBehaviour
{
    public AnimationCurve MovementStrength; // Curve to handle the Speed depending on the distance to the rig/camera
    public AnimationCurve JoystickMovementStrength; // Curve to handle the Speed depending on the distance to the rig/camera
    public GameObject HRTF_Rig; // The object that can be controlled by the user 

    private MovementActions actions;

    private void Start()
    {
        actions = new();
        actions.Default.Enable();
        
    }

    void Update()
    {
        
        Vector3 movement_vector = ActualJoystick();
        transform.position += Time.deltaTime*movement_vector;

    }

    private Vector3 HumanJoystick()
    {
        Vector3 direction = HRTF_Rig.transform.position - transform.position;
        direction.y = 0; // only interested in direction regarding to xz plane

        // move this. Direction based on where the user is, strength based on the distance

        Vector3 movement_vector = direction.normalized * MovementStrength.Evaluate(direction.magnitude);
        return movement_vector;
    }

    private Vector3 ActualJoystick()
    {
        Vector2 delta = actions.Default.Move.ReadValue<Vector2>();
        Debug.Log(delta);
        Vector3 fwd = Camera.main.transform.forward;
        fwd.y = 0;
        fwd = fwd.normalized;

        Vector3 right = Camera.main.transform.right;
        right.y = 0;
        right = right.normalized;
        
        delta*=JoystickMovementStrength.Evaluate(delta.magnitude);
        
        return delta.x*right+delta.y*fwd;
    }
}
