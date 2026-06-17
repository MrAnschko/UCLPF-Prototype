using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PositionTracking : DataContainer,LoggerInterface
{

    [SerializeField] List<Vector2> rel_positions = new();
    [SerializeField] Vector2 goal;
    [SerializeField] List<float> goalDists= new();
    [SerializeField] List<float> time = new();

    public PositionTracking(Vector3 goal)
    {
        this.goal = new();
        this.goal.x = goal.x; // Only use xy, for tracking xz position -> elevation disregarded
        this.goal.y = goal.z;
        LoggingManager.RegisterLogger(this);
        
    }

    // Tracks the change of Azimuth since the step beginning.
    public void AddData()
    {
        Vector2 rel_position = new();
        Vector3 cameraPos = Camera.main.transform.position;
        goalDists.Add(new Vector2(goal.x - cameraPos.x,goal.y - cameraPos.z).magnitude);
        cameraPos = CustomWorldOrigin.PosUnityToMap(cameraPos);
        rel_position.x = cameraPos.x;
        rel_position.y = cameraPos.z;
        //rel_position = CustomWorldOrigin.PosUnityToMap(rel_position);
        rel_positions.Add(rel_position);
        //goalDists.Add(rel_position.magnitude);
        time.Add(ProcessInfos.StepTime);
    }

    public void Save()
    {
        Save(ProcessInfos.UserID + "_Path" + ProcessInfos.currentPath + "_Step" + ProcessInfos.CurrentStep + "_Position");
    }

    ~PositionTracking()
    {
        LoggingManager.DeRegisterLogger(this);
    }
}
