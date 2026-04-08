

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class LPGlobalSettings
{
    private static GameObject pointObject;
    static float qFactor = 0.707f;
    static float cutoffInitialFreq = 22050;
    static float cutoffMinimalFreq = 20;
    static float half_angle = 314f;
    static float pointDistFactor= 0;
    static float circleDistFactor=0;
    static float horizonBehavior = 0;
    static float planePosition = -1;

    static List<LPSettingsInformer> SettingsInformer = new();

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

    public static GameObject PointObject 
    {
        get => pointObject;
        set
        {
            pointObject = value;
            UpdateFilters();
        }
    }

    public static void RegisterSelf(LPSettingsInformer controller)
    {
        SettingsInformer.Add(controller);
    }

    public static void UnregisterSelf(LPSettingsInformer controller)
    {
        SettingsInformer.Remove(controller);
    }

    public static void UpdateFilters()
    {
        foreach (LPSettingsInformer settingsInformer in SettingsInformer)
        {
            settingsInformer.UpdateSettings();
        }
    }
}
