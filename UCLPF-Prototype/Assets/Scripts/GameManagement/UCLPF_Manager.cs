using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class UCLPF_Manager : MonoBehaviour
{
    public AudioMixer mixer;

    [SerializeField]
    Material visualizationMaterial;

    // Bookkeeping for running the Test
    [SerializeField]
    string map_identifier;

    [SerializeField]
    string probant_identifier;
    int current_set = 0;
    [SerializeField]
    List<GameObject> pointSets; // Gameobject to be enabled containing all the sources for a step.
    [SerializeField]
    List<AudioSourceHandler> goalAudioSourceHandler; // Goal at each set of steps.
    [SerializeField]
    float acceptance_radius; // the radius at which a goal is presumed to be reached.
    [SerializeField]
    float acceptance_time; // the time one needs to stand at a point until it counts as accepted /based on camera position, projected onto the plane
    float time_in_radius = 0.0f; // used to track how long a user has been standing in the Radius.
    [SerializeField]
    float top_speed; // the radius at which a goal is presumed to be reached.

    [SerializeField]
    UCLPF_Settings settings;
    [SerializeField]
    UCLPF_Settings.Mode mode;


    // DATA
    public DataContainer user_data;
    [SerializeField]
    public MapData map_data;




    private void Start()
    {
        mode = PersistentData.Mode;

        GetComponent<GoalVisualization>()?.Setup();
        Debug.Log($"Mode Set to {mode}");

        MapConstructorFromFile mapConstructor = GetComponent<MapConstructorFromFile>();
        if (mapConstructor != null)
            mapConstructor.MakeMap();

        settings.SetMode(mode);

        SetupPathsAndMapData();
        SetStep(0);

       

    }


    public void SetStep(int i)
    {
        SoundPlayer.instance.ClearAudioSources();
        pointSets[current_set].gameObject.SetActive(false);
        goalAudioSourceHandler[i].MakeGoalVis();
        current_set = i;
        pointSets[i].gameObject.SetActive(true);
        SoundPlayer.instance.StartPlaying();
        if(i!=0)
            user_data.Save();
        user_data = new DataContainer(probant_identifier, map_identifier, current_set.ToString());
    }
    

    private void Update()
    {
        
        ContinueTest();
        

    }


    private void FixedUpdate()
    {
        UpdateData();
    }





    // Method to 
    private void SetupPathsAndMapData()
    {
        pointSets.Clear();

        map_data = new MapData(map_identifier, top_speed, mode.ToString());
        
        int i = 0;

        Transform found_set = transform.Find("Set" + i.ToString());
        while (found_set !=null)
        {
            Transform target_transform = found_set.Find("TargetAudioSource");
            goalAudioSourceHandler.Add(target_transform.gameObject.GetComponent<AudioSourceHandler>());
            pointSets.Add(found_set.gameObject);


            // Add map Data:
            List<POI_Info> distractor_poi = new List<POI_Info>();
            foreach (Transform child in found_set)
            {
                if(child.name != "TargetAudioSource"){
                    POI_Info info = new POI_Info();
                    info.pos = child.transform.position;
                    info.id = child.GetComponent<AudioSourceHandler>().AudioID;
                    distractor_poi.Add(info);
                    
                }

            }

            POI_Info targetInfo = new POI_Info();
            targetInfo.pos = target_transform.position;
            targetInfo.id = target_transform.GetComponent<AudioSourceHandler>().AudioID;

            map_data.AddStep(distractor_poi, targetInfo);


            found_set.gameObject.SetActive(false);
            i++;
            found_set = transform.Find("Set" + i.ToString());

        }

        map_data.Save();

    }

    // Advance to check if the next test should ber un
    private void ContinueTest()
    {

        float distance_to_goal = (Camera.main.transform.position.x - goalAudioSourceHandler[current_set].transform.position.x) * (Camera.main.transform.position.x - goalAudioSourceHandler[current_set].transform.position.x) +
                        (Camera.main.transform.position.z - goalAudioSourceHandler[current_set].transform.position.z) * (Camera.main.transform.position.z - goalAudioSourceHandler[current_set].transform.position.z);
        //Debug.Log($"Distance to Goal:{distance_to_goal}");
        //check distance
        if (distance_to_goal < acceptance_radius * acceptance_radius)
        {
            time_in_radius += Time.deltaTime;
            //Debug.Log($"Time in Radius: {time_in_radius} ");
            if (time_in_radius > acceptance_time)
            {
                int next_set = (current_set + 1 < pointSets.Count) ? current_set + 1 : 0;
                SetStep(next_set);
                time_in_radius = 0;
            }
        }
        else
        {
            time_in_radius = 0;
        }
    }


    // Method to save the data
    private void UpdateData()
    {
        Vector3 goal_direction = Camera.main.transform.position- goalAudioSourceHandler[current_set].transform.position;
        Vector3 rel_position = goal_direction;
        rel_position.y = 0;


        Vector3 view_direction = settings.PointObject.transform.forward;
        view_direction.y = 0;


        float lr_sign = Mathf.Sign(Vector3.Dot(settings.PointObject.transform.right, goal_direction));

        float azimuth = lr_sign*Mathf.Acos(-Vector3.Dot(view_direction.normalized,rel_position)/rel_position.magnitude);
        //Debug.Log($"Azimuth: {azimuth}");
        user_data.AddStep(rel_position, azimuth);
    }

    public void ReturnToMenu()
    {
        StartMenu.LoadModeMenuScene();
    }
}
