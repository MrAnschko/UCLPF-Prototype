using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//DataContainer for saving the unchanging data of a path
[Serializable]
public class PathData : DataContainer
{
    [SerializeField] string UserIdentifier = "";

    [SerializeField]
    ProcessInfos.InteractionMethod interactionMethod;
    [SerializeField]
    int PathOrder; // at what time the path has been
    [SerializeField]
    List<StepData> steps = new List<StepData>();

    public void RegisterStepData(StepData stepData) {  steps.Add(stepData); }

    public void Save()
    {
        Save(UserIdentifier + "_" + interactionMethod);
    }
}
