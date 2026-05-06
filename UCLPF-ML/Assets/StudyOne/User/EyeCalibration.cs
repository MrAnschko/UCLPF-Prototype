using System;
using System.Collections;
using UnityEngine;

public class EyeCalibration : MonoBehaviour
{
    private string intentID = "com.magicleap.intent.action.EYE_CALIBRATION";
    private float startDelay = 3f;

    private IEnumerator Start()
    {

        yield return new WaitForSeconds(startDelay);
        try
        {
            if (!Application.isEditor)
            {
                OpenActivity();
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void OpenActivity()
    {
#if UNITY_MAGICLEAP || UNITY_ANDROID
        using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var intentObject = new AndroidJavaObject("android.content.Intent", intentID))
        {
            currentActivityObject.Call("startActivity", intentObject);
        }
#endif
    }
}