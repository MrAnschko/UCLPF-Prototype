using MagicLeap.Android;
using MagicLeap.OpenXR.Features.EyeTracker;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.OpenXR;

[Serializable]
public class EyeTrackingData : DataContainer, LoggerInterface
{
    [DoNotSerialize] bool permissionGranted = false;
    [DoNotSerialize] bool eyeTrackerStarted = false;
    [DoNotSerialize] private MagicLeapEyeTrackerFeature eyeTrackerFeature;

    [SerializeField]List<SerializedEyeTrackerData> eyeTrackerDatas;
    [SerializeField] string test = "test";
    

    

    void RequestPermission()
    {
        Permissions.RequestPermissions(new string[] { Permissions.EyeTracking, Permissions.PupilSize }, (string st) => permissionGranted =true, (string st) => permissionGranted=false);
    }

    public EyeTrackingData()
    {
        LoggingManager.RegisterLogger(this);
        eyeTrackerDatas = new List<SerializedEyeTrackerData>();
    }

    ~EyeTrackingData()
    {
        LoggingManager.DeRegisterLogger(this);
    }

    public void AddData()
    {
        RequestPermission();
        if (permissionGranted && !eyeTrackerStarted)
            StartEyeTracker();
        if (permissionGranted)
            GetEyeData();
    }

    public void Save()
    {
        Save(ProcessInfos.UserID + "_Path" + ProcessInfos.currentPath + "_Step" + ProcessInfos.CurrentStep + "_EyeTracking"); ;
    }

    void StartEyeTracker()
    {
        eyeTrackerFeature = OpenXRSettings.Instance.GetFeature<MagicLeapEyeTrackerFeature>();
        if (eyeTrackerFeature != null && eyeTrackerFeature.enabled)
        {
            eyeTrackerFeature.CreateEyeTracker();
            eyeTrackerStarted = true;   
            Debug.Log("Eye Tracker initialized.");
        }
    }

    void GetEyeData()
    {
        if((eyeTrackerFeature != null) && eyeTrackerFeature.enabled)
        {
            EyeTrackerData data;
            data = eyeTrackerFeature.GetEyeTrackerData();
            eyeTrackerDatas.Add(new SerializedEyeTrackerData(data));
        }

    }
}
