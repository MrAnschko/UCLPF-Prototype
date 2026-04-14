using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class LatinSquare<T_Block> : DataContainer where T_Block:Enum
{
    // Alternaively look into williams sqares : https://statpages.info/latinsq.html


    [SerializeField] private string block;
    [SerializeField] private int index; //

    
    

    public void Save()
    {
        block = typeof(T_Block).ToString();
        
        Save("LatinSquare_"+block);
    }

    public void Load()
    {
        string filename = "LatinSquare_"+block;
        string path = Path.Combine(Application.persistentDataPath, filename + ".json");
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, this);
        }

    }


    public List<T_Block> StepChoice()
    {
        T_Block[] array = (T_Block[])Enum.GetValues(typeof(T_Block));
        
        List<T_Block> list = new ();
        for(int i = 0; i < array.Length; i++)
        {
            list.Add(array[(i + index) % array.Length]);
        }
        return list;
    }
}
