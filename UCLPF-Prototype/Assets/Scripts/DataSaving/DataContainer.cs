using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


// Object to track the user behaviour data during a test.
[Serializable]
public class DataContainer 
{

    public string probant_identifier; // Information to track the individual 
    public string map_identifier; // Identifier to track the map
    public string step_identifier; // Identifier to track the step of the map
    public float start_time; // internal start time

    public List<Vector3> relative_position;
    public List<float> rel_azimuth;
    public List<float> rel_time; // List to track distance and direction at each timestep

    public DataContainer(string probant_identifier, string map_identifier, string step_identifier)
    {
        this.probant_identifier = probant_identifier; this.map_identifier = map_identifier; this.step_identifier = step_identifier;

        start_time = Time.time;
        relative_position = new List<Vector3>();
        rel_azimuth = new List<float>();
        rel_time = new List<float>();   
    }

    public void AddStep(Vector3 rel_position, float azimuth)
    {
        
        relative_position.Add(rel_position);
        rel_azimuth.Add(azimuth);
        rel_time.Add(Time.time - start_time);
    }

    public void Save()
    {
        string path = Path.Combine(Application.persistentDataPath, "User_Data_"+System.DateTime.Now.ToFileTime() +"_"+ probant_identifier+"_"+map_identifier+"_"+step_identifier+".json");
        var json = JsonUtility.ToJson(this);
        File.WriteAllText(path, json);
    }


}
