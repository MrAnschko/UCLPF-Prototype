using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


// Class to track which methods had been chosen in which order so that they can be counterbalanced
[Serializable]
public class OveralDC : DataContainer
{
    static string filename = "GlobalInfo";



    [SerializeField]
    public List<int> MethodOrders;


    ~OveralDC()
    {
        Save();
    }

    public void Load()
    {
        string path = Path.Combine(Application.persistentDataPath, filename + ".json");
        var json = File.ReadAllText(path);
        JsonUtility.FromJsonOverwrite(json, this);
        int list_length = ProcessInfos.COMBINATION_POSSIBILITIES;
        if (MethodOrders.Count < list_length)
        {
            MethodOrders = new List<int>(new int[list_length]);

        }

    }

    public void Save()
    {
        Save(filename);
    }



    // Gives the index of all minimum Occurences
    public List<int> MinimumMethodOrdersIndex()
    {
        Load();

        List<int> indices = new();
        int minimum = MethodOrders[0];

        for (int index = 0; index < MethodOrders.Count; index++)
        {
            if(minimum > MethodOrders[index])
            {
                minimum = MethodOrders[index];
                indices.Clear();
                indices.Add(index);
            }
            else if( minimum == MethodOrders[index])
            {
                indices.Add(index);
            }

        }
        return indices;

    }

    public void AddOccurrence(List<ProcessInfos.InteractionMethod> methodOrder)
    {
        int index = ProcessInfos.MethodOrderToInt(methodOrder);
        MethodOrders[index]++;
    }

}
