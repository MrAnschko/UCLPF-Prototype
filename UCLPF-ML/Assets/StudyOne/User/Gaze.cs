using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicLeap.Android;
using System.Linq;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR;


// Gaze tracking. Strongly inspired by the Gaze Sample provided by the Magic Leap samples.
public class Gaze : MonoBehaviour
{
    private List<InputDevice> InputDeviceList = new();
    private InputDevice eyeTracking;
    private Camera mainCamera;

    private bool permissionGranted;

    public static Gaze instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log($"Tried making multiple Gaze Trackers. \n Deleting most recent one: {this}");
        }
        instance = this;
        Permissions.RequestPermission(Permissions.EyeTracking, OnPermissionGranted, OnPermissionDenied);
        LPGlobalSettings.PointObject = gameObject;
    }

    private void Update()
    {
        if (!permissionGranted)
            return;

        if (!eyeTracking.isValid)
        {
            List<InputDevice> tests = new();
            InputDevices.GetDevices(tests);
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.EyeTracking, InputDeviceList);
            eyeTracking = InputDeviceList.FirstOrDefault();

            if (!eyeTracking.isValid)
            {
                Debug.LogError("Unable to acquire eye tracking device. Have permissions been granted?");
                return;
            }
        }

        bool hasData = eyeTracking.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked);
        hasData &= eyeTracking.TryGetFeatureValue(EyeTrackingUsages.gazePosition, out Vector3 position);
        hasData &= eyeTracking.TryGetFeatureValue(EyeTrackingUsages.gazeRotation, out Quaternion rotation);

        if (isTracked && hasData)
        {
            transform.SetPositionAndRotation(position, rotation);
        }


    }

    private void OnPermissionGranted(string permission)
    {
        permissionGranted = true;
        mainCamera = Camera.main;
    }

    private void OnPermissionDenied(string permission)
    {
        Debug.LogError($"{permission} denied, example won't function");
    }
}
