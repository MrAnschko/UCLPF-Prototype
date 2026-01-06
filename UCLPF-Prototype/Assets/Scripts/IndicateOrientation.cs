using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicateOrientation : MonoBehaviour
{
    public GameObject otherObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = otherObj.transform.position;
        transform.localRotation = otherObj.transform.localRotation;
    }
}
