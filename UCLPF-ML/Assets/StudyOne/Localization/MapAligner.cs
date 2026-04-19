using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAligner : MonoBehaviour
{
    Quaternion initialRot;
    Vector3 initialPos;

    void Start()
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
