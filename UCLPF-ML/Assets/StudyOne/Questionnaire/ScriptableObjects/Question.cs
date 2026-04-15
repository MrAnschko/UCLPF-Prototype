using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Question
{
    public enum Type
    {
        Likert,
        BeaconNumber,
        PointTask,
        TLX,
        Choice
    }
    [SerializeField] public string question;
    [SerializeField] public Type type;

    [SerializeField] public string SliderStartDesc;
    [SerializeField] public string SliderEndDesc;
    
}
