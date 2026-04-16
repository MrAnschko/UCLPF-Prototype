using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.XR.MagicLeap.MLMediaRecorder;

[Serializable]
public class PathBuilder : MonoBehaviour
{
    [Header("Waypoint Placement")]
    [SerializeField] GameObject waypointVisualizer;
    [SerializeField] List<Vector3> waypoints;
    [SerializeField] InputActionReference placeWaypointAReference;
    InputAction placeWaypointA;

    [Header("Audio Source Placement")]
    [SerializeField] GameObject AudioSource;
    [SerializeField] List<Vector3> audioSources;
    [SerializeField] InputActionReference placeAudioSourceAReference;
    InputAction placeAudioSourceA;


    [Header("Controller Stuff")]
    [SerializeField] InputActionReference controllerRotAR;
                     InputAction controllerRotA;
    [SerializeField] InputActionReference controllerPosAR;
                     InputAction controllerPosA;
    [SerializeField] InputActionReference saveAR;
    InputAction SaveA;

    Quaternion controllerDirection;
    Vector3 controllerPosition;

    private void Awake()
    {
        audioSources = new();
        waypoints = new();
        placeWaypointA = placeWaypointAReference.action;
        placeWaypointA.performed += PlaceWaypoint;

        placeAudioSourceA = placeAudioSourceAReference.action;
        placeAudioSourceA.performed += PlaceAudioSource;

        controllerRotA = controllerRotAR.action;
        controllerPosA = controllerPosAR.action;

        SaveA = saveAR.action;
        SaveA.performed += Save;
    }


    void PlaceWaypoint(InputAction.CallbackContext context)
    {
        Vector3 hp = CalculateIntersection();
        waypoints.Add(hp);
        Instantiate(waypointVisualizer, hp, Quaternion.identity);
    }

    void PlaceAudioSource(InputAction.CallbackContext context)
    {
        Vector3 hp = CalculateIntersection();
        audioSources.Add(hp);
        Instantiate(AudioSource, hp, Quaternion.identity);


    }

    Vector3 CalculateIntersection()
    {
        
        controllerDirection = controllerRotA.ReadValue<Quaternion>();
        controllerPosition = controllerPosA.ReadValue<Vector3>();

        Vector3 dir = controllerDirection * Vector3.forward;
        float alpha = (dir.y < 0f) ? (-1f - controllerPosition.y) / dir.y : 0f;
        Vector3 ret = controllerPosition + alpha * dir;
        return ret;

    }

    void Save(InputAction.CallbackContext context)
    {
        string path = Path.Combine(Application.persistentDataPath, "Map"+UnityEngine.Random.Range(0,10000) + ".json");
        var json = JsonUtility.ToJson(this);
        File.WriteAllText(path, json);
    }

    private void FixedUpdate()
    {
        transform.rotation = controllerRotA.ReadValue<Quaternion>();
        transform.position = controllerPosA.ReadValue<Vector3>();
    }
}
