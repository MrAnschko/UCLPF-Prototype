using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class MapAligner : MonoBehaviour
{
    Quaternion initialRot;
    Vector3 initialPos;
    void Awake()
    {
        initialPos = transform.position;
        initialRot = transform.rotation;
        CustomWorldOrigin.ReOrderedCallback += Align;

        Align();
        
    }

    void Align()
    {


        transform.position = CustomWorldOrigin.PosMapToUnity(initialPos);
        transform.rotation = CustomWorldOrigin.RotMapToUnity(initialRot);
    }

}
