using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.NativeTypes;
using MagicLeap.OpenXR.Features.LocalizationMaps;

public class QueryLocalizationSample : MonoBehaviour
{
    private MagicLeapLocalizationMapFeature localizationMapFeature = null;
    bool localized = false;
    private void Start()
    {
        // Obtain the instance of the localization Map Feature
        localizationMapFeature = OpenXRSettings.Instance.GetFeature<MagicLeapLocalizationMapFeature>();
        // If it is not present return
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
            if (data.State == LocalizationMapState.Localized && (!localized || data.Confidence >= LocalizationMapConfidence.Good))
            {
                Pose pose = localizationMapFeature.GetMapOrigin();
                
                Debug.Log($"Localized to space: {data.Map.Name}. Origin : {localizationMapFeature.GetMapOrigin()}");
                this.transform.position = pose.position;
                
                this.transform.rotation = pose.rotation;
                localized = true;
            }
            else
            {
                Debug.Log($"Localization State: {data.State}");
            }
        }
    }
}


