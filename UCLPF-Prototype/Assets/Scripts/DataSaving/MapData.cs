using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


// Object to track the data during a test.
[Serializable]
public class MapData
{

    public string map_identifier; // Identifier to track the map
    
    public List<Vector3> target_positions; // World Position of each of the targets
    public List<List<Vector3>> all_poi; // Position of each of points of interest
    public List<float> relative_distance; // Distance of the goals to the previous target. (or distance to (0,0,0) in case of the first target)
    public List<float> rel_time; // List to track distance and direction at each timestep
    public float top_speed; // The assumed top speed on this map.
    public string interaction_mode;
    
    public MapData(string map_identifier, float top_speed, string interaction_mode)
    {
        this.map_identifier = map_identifier;
        this.top_speed = top_speed;
        this.interaction_mode = interaction_mode;

        target_positions = new List<Vector3>();
        all_poi = new List<List<Vector3>>();
        relative_distance = new List<float>();
        rel_time = new List<float>();
    }
    
    public void AddStep(List<Vector3> object_positions,Vector3 target_position)
    {
        Vector3 prev_point = (target_positions.Count > 0) ? target_positions.Last<Vector3>() : Vector3.zero;
        float dist = (target_position - prev_point).magnitude;
        relative_distance.Add(dist);
        target_positions.Add(target_position);
        rel_time.Add(dist / top_speed);
        all_poi.Add(object_positions);
    }    


    public void Save()
    {
        string path = Path.Combine(Application.persistentDataPath, "MapData_" + map_identifier +".json");
        var json = JsonUtility.ToJson(this);
        File.WriteAllText(path, json);
    }

}
