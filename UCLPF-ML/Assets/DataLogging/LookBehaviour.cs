using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class BehaviorData
{
    [SerializeField] public float timeStep;
    [SerializeField] public List<Vector3> activePositions;
    [SerializeField] public List<BeaconData> beacons;

    public BehaviorData()
    {
        this.timeStep = ProcessInfos.StepTime;
        this.activePositions = new List<Vector3>();
        this.beacons = new List<BeaconData>();
    }


}

[Serializable]
public class LookBehaviour : DataContainer, LoggerInterface
{

    [SerializeField] public List<BehaviorData> Data;

    public void AddData()
    {
        if (Data == null) Data = new();

        BehaviorData new_dp = new BehaviorData();
        foreach (SourceVisualization vis in SourceVisualization.Visualizations)
        {
            if (vis.IsActive)
            {
                new_dp.activePositions.Add(CustomWorldOrigin.PosUnityToMap(vis.transform.position));
                new_dp.beacons.Add(vis.beaconData);
            }
        }
        Data.Add(new_dp);
    }

    public void Save()
    {
        Save(ProcessInfos.UserID + "_Path" + ProcessInfos.currentPath + "_Step" + ProcessInfos.CurrentStep + "_LookBehavior");
    }

    public void StartLogging()
    {
        LoggingManager.RegisterLogger(this);
    }

    public void StopLogging() 
    {
        Save();
        LoggingManager.DeRegisterLogger(this);
    }


}
