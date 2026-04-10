using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BeaconData : DataContainer
{
    [SerializeField]
    private SonificationClip clipInfo;
    public float Frequency;
    public string ClipName = "";

    public Vector3 position;

    public SonificationClip ClipInfo 
    { 
        get => clipInfo;
        set 
        {
            clipInfo = value;
            Frequency = clipInfo.frequency;
            ClipName = clipInfo.name;
        }
    }
}
