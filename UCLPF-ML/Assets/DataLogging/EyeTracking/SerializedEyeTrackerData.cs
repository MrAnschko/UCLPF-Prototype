using JetBrains.Annotations;
using MagicLeap.OpenXR.Features.EyeTracker;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;




[Serializable]
public class SerializedGeometricData
{
    [SerializeField] Eye Eye;
    [SerializeField] long Time;
    [SerializeField] bool Valid;
    [SerializeField] float EyeOpenness;
    [SerializeField] Vector2 EyeInSkullPosition;

    public SerializedGeometricData(GeometricData data)
    {
        Eye = data.Eye;
        Time = data.Time;
        EyeOpenness = data.EyeOpenness;
        EyeInSkullPosition = data.EyeInSkullPosition;
    }
}

[Serializable]
public class SerializedPupilData
{
    [SerializeField] Eye Eye;
    [SerializeField] long Time;
    [SerializeField] bool Valid;
    [SerializeField] float PupilDiameter;

    public SerializedPupilData(PupilData data)
    {
        Eye = data.Eye;
        Time = data.Time;
        Valid = data.Valid;
        PupilDiameter = data.PupilDiameter;
    }
}

[Serializable]
public class SerializedGazeBehavior
{
    [SerializeField] SerializedGazeBehaviorMetaData MetaData;
    [SerializeField] long Time;
    [SerializeField] bool Valid;
    [SerializeField] string GazeBehaviorType;
    [SerializeField] long OnsetTime;
    [SerializeField] ulong Duration;

    static string[] gazeTypes =
    {
        "Unknown",
        "EyesClosed",
        "Blink",
        "BlinkLeft",
        "BlinkRight",
        "Fixation",
        "Pursuit",
        "Saccade"
    };

    public SerializedGazeBehavior(GazeBehavior data)
    {
        MetaData = new SerializedGazeBehaviorMetaData(data.MetaData);
        Time = data.Time;
        Valid = data.Valid;
        GazeBehaviorType = gazeTypes[(int)data.GazeBehaviorType];
        OnsetTime = data.OnsetTime;
        Duration = data.Duration;
    }

}

[Serializable]
public class SerializedGazeBehaviorMetaData
{
    [SerializeField] bool Valid;
    [SerializeField] float Amplitude;
    [SerializeField] float Direction;
    [SerializeField] float Velocity;
    public SerializedGazeBehaviorMetaData(GazeBehaviorMetaData data)
    {
        Valid = data.Valid;
        Amplitude = data.Amplitude;
        Direction = data.Direction;
        Velocity = data.Velocity;
    }
}

[Serializable]
public class SerializedEyeTrackerData 
{
    [SerializeField] SerializedGeometricData[] geometricData;
    [SerializeField] SerializedPupilData[] pupilData;
    [SerializeField] SerializedGazeBehavior gazeBehavior;

    public SerializedEyeTrackerData(EyeTrackerData data)
    {
        geometricData = new SerializedGeometricData[2];
        geometricData[0] = new SerializedGeometricData(data.GeometricData[0]);
        geometricData[1] = new SerializedGeometricData(data.GeometricData[1]);

        pupilData = new SerializedPupilData[2];
        pupilData[0] = new SerializedPupilData(data.PupilData[0]);
        pupilData[1] = new SerializedPupilData(data.PupilData[1]);

        gazeBehavior = new SerializedGazeBehavior(data.GazeBehaviorData);
    }
}
