using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PositionTracking : DataContainer
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
        
    }

    // Tracks the change of Azimuth since the step beginning.
    public void AddData()
    {
        Vector2 rel_position = new();
        Vector3 cameraPos = Camera.main.transform.position;
        rel_position.x = goal.x - cameraPos.x;
        rel_position.y = goal.y - cameraPos.z;
        rel_positions.Add(rel_position);
        goalDists.Add(rel_position.magnitude);
        time.Add(ProcessInfos.StepTime);
    }

    public void Save()
    {
        Save(ProcessInfos.UserID + "_Path" + ProcessInfos.currentPath + "_Step" + ProcessInfos.CurrentStep + "_Position");
    }

    ~PositionTracking()
    {
        Save();
    }
}
