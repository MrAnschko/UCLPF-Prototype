using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StudyInfo:DataContainer
{
    [SerializeField]string userIdentifier = "UserID";
    [SerializeField]public List<ProcessInfos.InteractionMethod> MethodOrder;
    [SerializeField]public List<ProcessInfos.Path> PathOrder;

    public string UserIdentifier 
    { 
        get => ProcessInfos.UserID;
        set 
        {
            userIdentifier = value;
            ProcessInfos.UserID = value;
        }
    }

    public void Save()
    {
        
        Save("StudyInfo_" + UserIdentifier);
    }
}
