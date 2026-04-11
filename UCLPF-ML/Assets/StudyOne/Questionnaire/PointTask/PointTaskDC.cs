using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PointTaskDC : DataContainer
{
    [SerializeField] public string desc;
    [SerializeField] public Vector2 point;

    public PointTaskDC(string desc) {  this.desc = desc; }

    public void Save()
    {
        Save(ProcessInfos.UserID + "_Path" + ProcessInfos.currentPath + "_Step" + ProcessInfos.CurrentStep + "_PointTasc_" + desc);
    }
}
