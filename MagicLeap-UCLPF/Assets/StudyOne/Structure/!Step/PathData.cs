using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class PathData : DataContainer
{
    public ProcessInfos.Path path;
    [SerializeField]
    public List<BeaconData> Beacons;
    [SerializeField]
    public Vector3 StepEnd;


}
