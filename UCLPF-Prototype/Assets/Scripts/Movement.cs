using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// A class that contains code for moving the subject
// should be a parent of the rig.
public class Movement : MonoBehaviour
{
    public AnimationCurve MovementStrength;
    public GameObject HRTF_Rig;



    void Update()
    {
        Vector3 direction = HRTF_Rig.transform.position - transform.position; 
        direction.y = 0; // only interested in direction regarding to xz plane

        // move this.

        Vector3 movement_vector = direction * MovementStrength.Evaluate(direction.magnitude);
        transform.position += Time.deltaTime*movement_vector;

    }
}
