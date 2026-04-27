using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//DataContainer for saving the unchanging data of a path
[Serializable]
public class PathData : DataContainer
{
    
    [SerializeField]
    ProcessInfos.InteractionMethod interactionMethod;
    [SerializeField]
    public int PathOrder; // at what time the path has been
    [SerializeField]
    List<StepData> steps = new List<StepData>();

    public void RegisterStepData(StepData stepData) {  steps.Add(stepData); }

    public void Save()
    {
        PathOrder = ProcessInfos.PathCount;
        interactionMethod = ProcessInfos.CurrentMethod;
        Save(ProcessInfos.UserID + "_Path" + ProcessInfos.currentPath+ "_PathData");
    }
}
