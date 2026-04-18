using MagicLeap.OpenXR.Features.LocalizationMaps;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.NativeTypes;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(XROrigin))]
public class LocalizeUser : MonoBehaviour
{
    private MagicLeapLocalizationMapFeature localizationMapFeature = null;
    bool localized = false;
    [SerializeField] XROrigin XROrigin;

    private void Start()
    {
        // Obtain the instance of the localization Map Feature
        localizationMapFeature = OpenXRSettings.Instance.GetFeature<MagicLeapLocalizationMapFeature>();
        // If it is not present return
        XROrigin = GetComponent<XROrigin>();
        if (localizationMapFeature == null || !localizationMapFeature.enabled)
        {
            Debug.LogError("Magic Leap Localization Map Feature does not exists or is disabled.");
            return;
        }

        // Enable the localization Events
        XrResult result = localizationMapFeature.EnableLocalizationEvents(true);
        if (result != XrResult.Success)
        {
            Debug.LogError($"Failed to enable localization events with result: {result}");
        }
    }

    private void Update()
    {
        CheckLocalizationStatus();
    }

    private void CheckLocalizationStatus()
    {
        LocalizationEventData data;
        if (localizationMapFeature.GetLatestLocalizationMapData(out data))
        {
            
            CustomDebug.Log(data.Confidence.ToString());
            if (data.State == LocalizationMapState.Localized && (!localized || data.Confidence >= LocalizationMapConfidence.Fair))
            {
                localizationMapFeature.RequestMapLocalization(data.Map.MapUUID);
                Pose pose = localizationMapFeature.GetMapOrigin();
                
                Debug.Log($"Localized to space: {data.Map.Name}. Origin : {localizationMapFeature.GetMapOrigin()}");
                //this.transform.position = pose.position;
                //this.transform.rotation = pose.rotation;
                //CustomDebug.Log(Camera.main.transform.position.ToString());
                
                //XROrigin.MoveCameraToWorldLocation(-pose.position);
                //Quaternion inverseRot = Quaternion.Inverse(pose.rotation);
                //bool test = XROrigin.MatchOriginUpCameraForward(pose.rotation * Vector3.up, inverseRot * Vector3.forward);
                localized = true;
            }
            else
            {
                Debug.Log($"Localization State: {data.State}");
            }
        }
    }
}


