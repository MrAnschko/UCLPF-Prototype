using System;
using System.Collections.Generic;
using UnityEngine;


// Data Container for 'nonchanging' Data of a Step
[Serializable]
public class StepData : DataContainer
{
    public ProcessInfos.Path path;
    [SerializeField]
    int stepIndex;
    [SerializeField]
    public List<BeaconData> Beacons;
    [SerializeField]
    public Vector3 StepEnd;
    
    //
}
