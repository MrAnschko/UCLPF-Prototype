using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "StandardSonClip", menuName = "UCLPF/SonificationClip", order = 1)]
[Serializable]
public class SonificationClip:ScriptableObject
{
    [SerializeField] public String clipName; // Name of the clip, for human reading
    [SerializeField] public float frequency;  // an ID, used for comparison
    [SerializeField] public AudioClip clip; // the Sonification clip

    public static bool operator <(SonificationClip a, SonificationClip b) { return a.frequency < b.frequency; }
    public static bool operator >(SonificationClip a, SonificationClip b) { return a.frequency > b.frequency; }
    public static bool operator ==(SonificationClip a, SonificationClip b) { return a.frequency == b.frequency; }
    public static bool operator !=(SonificationClip a, SonificationClip b) { return a.frequency != b.frequency; }

    public override bool Equals(object obj)
    {
        if (obj.GetType() != typeof(SonificationClip))
            return false;
        else
            return this == (SonificationClip)obj;
    }

}
