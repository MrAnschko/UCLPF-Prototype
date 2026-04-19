using MagicLeap.OpenXR.Features.LocalizationMaps;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.NativeTypes;

public class CustomWorldOrigin : MonoBehaviour
{
    Action ReOrderedCallback;

    private static CustomWorldOrigin instance;
    private MagicLeapLocalizationMapFeature localizationMapFeature = null;
    bool localized = false;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Created multiple CustomWorldOrigins. Deleting most recent.");
            Destroy(this);
            return;
        }
        instance = this;


        // Obtain the instance of the localization Map Feature
        localizationMapFeature = OpenXRSettings.Instance.GetFeature<MagicLeapLocalizationMapFeature>();
        // If it is not present return
        if (localizationMapFeature == null || !localizationMapFeature.enabled)
        {
            
            return;
        }

        // Enable the localization Events
        XrResult result = localizationMapFeature.EnableLocalizationEvents(true);
        if (result != XrResult.Success)
        {
            Debug.LogError($"Failed to enable localization events with result: {result}");
        }
        else
        {
            MagicLeapLocalizationMapFeature.OnLocalizationChangedEvent += AlignToMapOrign;

        }
        CheckLocalizationStatus();
    }

    // Update is called once per frame
    void Update()
    {
        CheckLocalizationStatus();
    }


    private void CheckLocalizationStatus()
    {
        LocalizationEventData data;
        if (localizationMapFeature.GetLatestLocalizationMapData(out data))
        {
            CustomDebug.Log(data.Confidence.ToString());
            AlignToMapOrign(data);

        }
    }


    void AlignToMapOrign(LocalizationEventData data)
    {
        CustomDebug.Log(data.Confidence.ToString());
        if (data.State == LocalizationMapState.Localized && (!localized || data.Confidence >= LocalizationMapConfidence.Fair))
        {
            Pose pose = localizationMapFeature.GetMapOrigin();


            this.transform.position = pose.position;

            this.transform.rotation = pose.rotation;
            localized = true;
        }
    }


    //makes new Transform transformed according to the transform defined by the world origing
    // Local Coordinates -> Unity Coordinates;
    public static Vector3 PosMapToUnity(Vector3 position)
    {
        if (instance == null)
        {
            return position;
        }
        return instance.transform.TransformPoint(position);
    }

    public static Vector3 DirMapToUnity(Vector3 direction)
    {
        if (instance == null)
        {
            return direction;
        }
        return instance.transform.TransformDirection(direction);
    }


    // Rotates a rotation to fit the unity coordinates
    public static Quaternion RotMapToUnity(Quaternion rotation)
    {
        if (instance == null)
        {
            return rotation;
        }
        return instance.transform.rotation*rotation;
    }

    public static Vector3 PosUnityToMap(Vector3 position)
    {
        if (instance == null) { return position; }
        return instance.transform.InverseTransformPoint(position);
    }


    public static Vector3 DirUnityToMap(Vector3 direction)
    {
        if (instance == null) { return direction; }
        return instance.transform.InverseTransformDirection(direction);
    }

    public static Quaternion RotUnityToMap(Quaternion rotation)
    {
        if (instance == null)
        {
            return rotation;
        }
        return Quaternion.Inverse(instance.transform.rotation) * rotation;
    }

}
