using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GazeTesting : MonoBehaviour
{
    public InputActionReference gazePositionRef;
    public InputActionReference gazeDirectionRef;
    public InputActionReference TrackingStateRef;

    InputAction gazePositionIA;
    InputAction gazeDirectionIA;
    InputAction trackingStateIA;
    private void Start()
    {
        gazePositionIA = gazePositionRef.action;
        gazeDirectionIA= gazeDirectionRef.action;
        trackingStateIA = TrackingStateRef.action;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 position = gazePositionIA.ReadValue<Vector3>();
        
        Vector3 direction = gazeDirectionIA.ReadValue<Quaternion>()*Vector3.forward;
        gameObject.transform.position = position+direction;

        Debug.Log(trackingStateIA.ReadValue<int>());

        
    }
}
