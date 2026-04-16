using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class AzimuthTracking : DataContainer,LoggerInterface
{
    [SerializeField] public GameObject trackedObject; // The object that is supposed to be tracked.
    [SerializeField] public string desc;
    [SerializeField] List<float> Azimuths = new();
    [SerializeField] List<float> time = new();

    public AzimuthTracking(GameObject gameObject, string desc)
    {
        LoggingManager.RegisterLogger(this);
        trackedObject = gameObject;
        this.desc = desc;
    }

    // Tracks the change of Azimuth since the step beginning.
    public void AddData()
    {
        Vector3 view_direction = LPGlobalSettings.PointObject.transform.forward;
        Vector3 rel_position = Camera.main.transform.position - trackedObject.transform.position;
        rel_position.y = 0f; // set only planar
        float lr_sign = Mathf.Sign(Vector3.Dot(LPGlobalSettings.PointObject.transform.right, trackedObject.transform.position));

        float az = lr_sign * Mathf.Acos(-Vector3.Dot(view_direction.normalized, rel_position) / rel_position.magnitude);
        Azimuths.Add(az);
        time.Add(ProcessInfos.StepTime);
    }

    public void Save()
    {
        Save(ProcessInfos.UserID + "_Path" + ProcessInfos.currentPath + "_Step" + ProcessInfos.CurrentStep+"_Azimuth_"+desc);
    }

    ~AzimuthTracking()
    {
        LoggingManager.DeRegisterLogger(this);
    }

}