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

    [SerializeField] static float planePosition = -1;
    [SerializeField] AnimationCurve responseCurve;

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
            Vector3 pointerFwd = transform.rotation * Vector3.forward;
            Vector3 gazeFwd = rotation * Vector3.forward;
            float similarity = Vector3.Dot(pointerFwd.normalized, gazeFwd.normalized);
            CustomDebug.Log(similarity.ToString());
            transform.rotation = Quaternion.Lerp(transform.rotation,rotation, responseCurve.Evaluate((1-similarity)/2));
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

    public static Vector2 GetGroundPlaneIntersection()
    {
        Matrix4x4 m = instance.mainCamera.transform.worldToLocalMatrix;

        //Position (of the listener) Creating the last column of the inverse of m. (-> a matrix that transforms from listener to world coordinates.)
        float l_x = -(m[12] * m[0] + m[13] * m[1] + m[14] * m[2]);
        float l_y = -(m[12] * m[4] + m[13] * m[5] + m[14] * m[6]);
        float l_z = -(m[12] * m[8] + m[13] * m[9] + m[14] * m[10]);

        float d_x = m[2];
        float d_y = m[6];
        float d_z = m[10];

        planePosition = LPGlobalSettings.PlanePosition;
        float alpha = (d_y < -0.001f) ? (planePosition - l_y) / d_y : 0; // set the alpha to zero in case there is no (positive) intersection

        // position on the plane
        float g_x = alpha * d_x + l_x;
        float g_z = alpha * d_z + l_z;

        return new Vector2(g_x, g_z);
    }
}
