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
    [SerializeField]
    public Vector3 StepEnd;
    
    public void Save()
    {
        stepIndex = ProcessInfos.CurrentStep;
        path = ProcessInfos.currentPath;
        Save(ProcessInfos.UserID + "_Path" + path + "_Step" + stepIndex+"_StepData");
    }
}
