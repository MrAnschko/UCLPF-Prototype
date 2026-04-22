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
    [SerializeField] public float lowAngle;
    [SerializeField] public Vector2 highestPt;
    [SerializeField] public float highAngle;
    public PointTaskDC(string desc) {  this.desc = desc; }

    public void Save()
    {
        lowestPt = new Vector2(PathHandler.low.transform.position.x, PathHandler.low.transform.position.z);
        highestPt = new Vector2(PathHandler.high.transform.position.x, PathHandler.high.transform.position.z);
        
        
        Vector3 lowDir = PathHandler.low.transform.position-Gaze.instance.transform.position;
        lowAngle = Vector3.Angle(Gaze.instance.transform.rotation*Vector3.forward,lowDir.normalized );

        Vector3 highDir = PathHandler.high.transform.position - Gaze.instance.transform.position;
        highAngle = Vector3.Angle(Gaze.instance.transform.rotation * Vector3.forward, highDir.normalized);


        Save(ProcessInfos.UserID + "_Path" + ProcessInfos.currentPath + "_Step" + ProcessInfos.CurrentStep + "_PointTasc_" + desc);
    }
}
