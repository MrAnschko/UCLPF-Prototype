using MagicLeap.Android;
using MagicLeap.OpenXR.Features.EyeTracker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.OpenXR;

public class EyeTrackingData : DataContainer, LoggerInterface
{
    bool permissionGranted = false;

    private MagicLeapEyeTrackerFeature eyeTrackerFeature;

    GeometricData geometricData;

    void RequestPermission()
    {
        Permissions.RequestPermissions(new string[] { Permissions.EyeTracking, Permissions.PupilSize }, (string st) => permissionGranted =true, (string st) => permissionGranted=false);
    }

    public EyeTrackingData()
    { 
        RequestPermission();
        if(permissionGranted)
            StartEyeTracker();
    }

    public void AddData()
    {
        throw new System.NotImplementedException();
    }

    public void Save()
    {
        throw new System.NotImplementedException();
    }

    void StartEyeTracker()
    {
        eyeTrackerFeature = OpenXRSettings.Instance.GetFeature<MagicLeapEyeTrackerFeature>();
        if (eyeTrackerFeature != null && eyeTrackerFeature.enabled)
        {
            eyeTrackerFeature.CreateEyeTracker();
            Debug.Log("Eye Tracker initialized.");
        }
    }

}
