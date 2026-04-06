using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public abstract class DataContainer
{
    public void Save(string filename)
    {
        string path = Path.Combine(Application.persistentDataPath, filename + ".json");
        var json = JsonUtility.ToJson(this);
        File.WriteAllText(path, json);
    }

}
