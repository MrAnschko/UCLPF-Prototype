using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapConstructorFromFile : MonoBehaviour
{
    [SerializeField]
    GameObject prefab;
    [SerializeField]
    SonificationHandler handler;
    [SerializeField]
    string fileName;

    public MapData data;


    public void MakeMap()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName+ ".json");
        if (!File.Exists(path))
        {
            Debug.LogError("Map File not Found");
            return;
        }
        var json = File.ReadAllText(path);
        data = new MapData();

        JsonUtility.FromJsonOverwrite(json, data);

        for (int step = 0; step < data.steps.Count; step++)
        {
            GameObject obj = new GameObject($"Set{step}");
            obj.transform.parent = transform;
            SetupStep(obj, data,step);
        }

    }

    void SetupStep(GameObject parent, MapData data, int step)
    {
        // Setup Target
        GameObject target_obj = Instantiate(prefab, parent.transform);
        POI_Info target_data = data.steps[step].target;
        target_obj.transform.position = target_data.pos;
        target_obj.GetComponent<AudioSourceHandler>().AudioID = target_data.id;
        target_obj.name = "TargetAudioSource";

        List<POI_Info> distractor_pois = data.steps[step].distractor_poi;
        for (int poi_number =0; poi_number < distractor_pois.Count; poi_number++)
        {
            GameObject distractor_obj = Instantiate(prefab, parent.transform);
            POI_Info distractor_data = distractor_pois[poi_number];
            distractor_obj.transform.position = distractor_data.pos;
            distractor_obj.GetComponent<AudioSourceHandler>().AudioID = distractor_data.id;
            
        }
    }
}
