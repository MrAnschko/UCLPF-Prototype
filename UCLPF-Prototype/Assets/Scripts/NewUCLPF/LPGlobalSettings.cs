

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class LPGlobalSettings
{
    static bool useEyeGaze;
    static float qFactor;
    static float cutoffInitialFreq;
    static float cutoffMinimalFreq;
    static float half_angle;
    static float pointDistFactor;
    static float circleDistFactor;
    static float horizonBehavior;
    static float planePosition = -1;

    static List<LPController> SettingsInformer;

    public static float QFactor 
    { 
        get => qFactor;
        set
        {
            qFactor = value;
            UpdateFilters();
        }
    }
    
    public static float CutoffInitialFreq 
    { 
        get => cutoffInitialFreq;
        set
        {
            cutoffInitialFreq = value;
            UpdateFilters();
        }
    }

    public static float CutoffMinimalFreq {
        get => cutoffMinimalFreq;
        set
        {
            cutoffMinimalFreq = value;
            UpdateFilters();
        }
    }

    public static float Half_angle
    {
        get => half_angle;
        set
        {
            half_angle = value;
            UpdateFilters();
        }
    }

    public static float PointDistFactor 
    {
        get => pointDistFactor;
        set
        {
            pointDistFactor = value;
            UpdateFilters();
        }
    }
    
    public static float CircleDistFactor 
    {
        get => circleDistFactor;
        set
        {
            circleDistFactor = value;
            UpdateFilters();
        }
    }

    public static float HorizonBehavior 
    { 
        get => horizonBehavior;
        set
        {
            horizonBehavior = value;
            UpdateFilters();
        }
    }
    public static float PlanePosition 
    { 
        get => planePosition;
        set
        {
            planePosition = value;
            UpdateFilters();
        }
    }
    public static bool UseEyeGaze 
    { 
        get => useEyeGaze;
        set
        {
            useEyeGaze = value;
            UpdateFilters();
        }
    }

    public static void RegisterSelf(LPController controller)
    {
        SettingsInformer.Add(controller);
    }

    public static void UnregisterSelf(LPController controller)
    {
        SettingsInformer.Remove(controller);
    }

    public static void UpdateFilters()
    {
        foreach (LPController controller in SettingsInformer)
        {
            controller.UpdateSettings();
        }
    }
}
