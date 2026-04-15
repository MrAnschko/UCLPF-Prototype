using System;
using System.Collections.Generic;
using UnityEngine;


// Data Container for 'nonchanging' Data of a Step
[Serializable]
public class StepData : DataContainer
{
    ProcessInfos.Path path;
    [SerializeField]
    int stepIndex;
    [SerializeField]
    public List<BeaconData> Beacons;
    [SerializeField] ProcessInfos.BeaconClass beaconClass;
    [SerializeField] public List<Vector3> intermediatePoints;
    [SerializeField] public Vector3 StepEnd;
    

    
    public void Save()
    {
        stepIndex = ProcessInfos.CurrentStep;
        path = ProcessInfos.currentPath;
        beaconClass = ProcessInfos.currentBeaconClass;
        Save(ProcessInfos.UserID + "_Path" + path + "_Step" + stepIndex+"_StepData");
    }
}
