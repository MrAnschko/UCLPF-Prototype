using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject : MonoBehaviour
{
    Quaternion initialRot;
    Vector3 intitialPos;

    [SerializeField] GameObject twin; // for testing;
    void Start()
    {
        intitialPos = transform.position;
        initialRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = CustomWorldOrigin.PosUnityToMap(twin.transform.position);
        transform.rotation = CustomWorldOrigin.RotUnityToMap(twin.transform.rotation);
    }
}
