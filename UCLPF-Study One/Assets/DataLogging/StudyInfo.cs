using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StudyInfo:DataContainer
{
    [SerializeField]public string UserIdentifier = "UserID";
    [SerializeField]public List<ProcessInfos.InteractionMethod> MethodOrder;
    [SerializeField]public List<ProcessInfos.Path> PathOrder;
    public void Save()
    {
        Save("StudyInfo_" + UserIdentifier);
    }
}
