
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.VolumeComponent;

[Serializable]
public class LatinSquare<T_Block> : DataContainer where T_Block:Enum
{
    // Alternaively look into williams sqares : https://statpages.info/latinsq.html


    [SerializeField] private string block;
    [SerializeField] private int index; //

    private int[] choice3 = {
        0, 1, 2, 
        1, 2, 0,
        2, 0, 1,
        2, 1, 0,
        0, 2, 1,
        1, 0, 2 }; // taken from https://statpages.info/latinsq.html



    public void Save()
    {
        block = typeof(T_Block).ToString();
        
        Save("LatinSquare_"+block);
    }

    public void Load()
    {
        block = typeof(T_Block).ToString();
        string filename = "LatinSquare_"+block;
        string path = Path.Combine(Application.persistentDataPath, filename + ".json");
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, this);
        }

    }


    public List<T_Block> Choice()
    {
        
        T_Block[] array = (T_Block[])Enum.GetValues(typeof(T_Block));
        if(array.Length !=4)
            throw new NotImplementedException();
        List<T_Block> list = new();
        for (int i  = 0; i < array.Length-1; i++)
        {
            list.Add(array[choice3[(index)*3+i]]);
        }
        index++;
        index %= 6;
        return list;
    }
}
