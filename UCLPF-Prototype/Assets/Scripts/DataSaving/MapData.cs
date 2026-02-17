using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class POI_Info{
    public Vector3 pos;
    public int id;
}

[Serializable]
public class StepInfo
{
    public POI_Info target;
    public List<POI_Info> distractor_poi;
}

// Object to track the data during a test.
[Serializable]
public class MapData
{

    public string map_identifier; // Identifier to track the map
    
    public List<StepInfo> steps; // World Position of each of the targets
    public List<float> relative_distance; // Distance of the goals to the previous target. (or distance to (0,0,0) in case of the first target)
    public List<float> rel_time; // List to track distance and direction at each timestep
    public float top_speed; // The assumed top speed on this map.
    public string interaction_mode;
    
    public MapData()
    {
        this.map_identifier = "";
        this.top_speed = 0.0f;
        this.interaction_mode = "";

        steps = new List<StepInfo>();
        relative_distance = new List<float>();
        rel_time = new List<float>();
    }

    public MapData(string map_identifier, float top_speed, string interaction_mode)
    {
        this.map_identifier = map_identifier;
        this.top_speed = top_speed;
        this.interaction_mode = interaction_mode;

        steps = new List<StepInfo>();
        relative_distance = new List<float>();
        rel_time = new List<float>();
    }
    
    public void AddStep(List<POI_Info> distractor_info,POI_Info target)
    {
        Vector3 prev_point = (steps.Count > 0) ? steps.Last<StepInfo>().target.pos: Vector3.zero;
        Vector3 diff_vector = (target.pos - prev_point);
        diff_vector.y = 0;
        float dist = diff_vector.sqrMagnitude;
        relative_distance.Add(dist);
        
        StepInfo step = new StepInfo();

        step.target = target;
        rel_time.Add(dist / top_speed);
        step.distractor_poi = distractor_info;

        steps.Add(step);
    }    


    public void Save()
    {
        string path = Path.Combine(Application.persistentDataPath, "MapData_" + map_identifier +".json");
        var json = JsonUtility.ToJson(this);
        File.WriteAllText(path, json);
    }

}
