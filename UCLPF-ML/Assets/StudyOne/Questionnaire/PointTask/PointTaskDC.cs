using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] public BeaconData closestBeaconData;
    [SerializeField] public float closesAngle;

    public PointTaskDC(string desc) {  this.desc = desc; }

    public void Save()
    {
        Vector3 lowPos = PathHandler.low.transform.position;
        Vector3 highPos = PathHandler.high.transform.position;
        lowestPt = new Vector2(lowPos.x, lowPos.z);
        highestPt = new Vector2(highPos.x, highPos.z);
        
        
        Vector3 lowDir = lowPos - Gaze.instance.transform.position;
        lowAngle = Vector3.Angle(Gaze.instance.transform.rotation*Vector3.forward,lowDir.normalized );

        Vector3 highDir = highPos - Gaze.instance.transform.position;
        highAngle = Vector3.Angle(Gaze.instance.transform.rotation * Vector3.forward, highDir.normalized);

        List<Beacon> beacons = PathHandler.ActiveBeacons;
        Beacon closestBeacon = beacons.MinItem(item => Vector2.Distance(new Vector2(item.transform.position.x, item.transform.position.z), point));
        closestBeaconData = closestBeacon.beaconData;

        Vector3 closestDir = closestBeacon.transform.position - Gaze.instance.transform.position;
        closesAngle = Vector3.Angle(Gaze.instance.transform.rotation * Vector3.forward, closestDir.normalized);


        Save(ProcessInfos.UserID + "_Path" + ProcessInfos.currentPath + "_Step" + ProcessInfos.CurrentStep + "_PointTasc_" + desc);
    }
}
