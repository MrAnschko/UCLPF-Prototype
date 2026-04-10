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
        BeaconNumber
    }
    [SerializeField] public string question;
    [SerializeField] public Type type;
}
