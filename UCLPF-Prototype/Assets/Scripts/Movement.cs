using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// A class that contains code for moving the subject
// should be a parent of the rig.
public class Movement : MonoBehaviour
{
    public AnimationCurve MovementStrength; // Curve to handle the Speed depending on the distance to the rig/camera
    public GameObject HRTF_Rig; // The object that can be controlled by the user 



    void Update()
    {
        Vector3 direction = HRTF_Rig.transform.position - transform.position; 
        direction.y = 0; // only interested in direction regarding to xz plane

        // move this. Direction based on where the user is, strength based on the distance

        Vector3 movement_vector = direction.normalized * MovementStrength.Evaluate(direction.magnitude);
        transform.position += Time.deltaTime*movement_vector;

    }
}
