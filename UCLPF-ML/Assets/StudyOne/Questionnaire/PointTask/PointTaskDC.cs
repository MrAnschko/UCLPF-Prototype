using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PointTaskDC : DataContainer
{
    [SerializeField] public string desc;
    [SerializeField] public Vector2 point;
    [SerializeField] public Vector2 lowestPt;
    [SerializeField] public Vector2 highestPt;
    public PointTaskDC(string desc) {  this.desc = desc; }

    public void Save()
    {
        lowestPt = new Vector2(PathHandler.low.transform.position.x, PathHandler.low.transform.position.z);
        highestPt = new Vector2(PathHandler.high.transform.position.x, PathHandler.high.transform.position.z);
        Save(ProcessInfos.UserID + "_Path" + ProcessInfos.currentPath + "_Step" + ProcessInfos.CurrentStep + "_PointTasc_" + desc);
    }
}
