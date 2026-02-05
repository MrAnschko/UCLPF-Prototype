using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class UCLPF_Manager : MonoBehaviour
{
    public AudioMixer mixer;
    public InputActionReference gazeDirectionRef;


    InputAction gazeDirectionIA;

    [SerializeField]
    Material visualizationMaterial;

    // Bookkeeping for running the Test
    [SerializeField]
    Mode mode;
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

    // DATA
    public DataContainer data;


    public enum Mode
    {
        None,
        Vector,
        Circle,
        Point
    }

    private void Start()
    {

        Debug.Log($"Mode Set to {mode}");

        SetMode(mode);
        gazeDirectionIA = gazeDirectionRef.action;

        SetupPaths();
        SetStep(0);

       
        data.Save();

    }
    public void SetMode(Mode mode)
    {
        switch (mode){
            case Mode.Circle: 
                SetModeCircle();
                return;
             case Mode.Point:
                SetModePoint();
                return;
             case Mode.Vector:
                SetModeVector();
                return;
             default:
                return;
        }

    }



    public void SetModeVector()
    {
        mixer.SetFloat("HalfAngle", Mathf.PI/180);
        mixer.SetFloat("PointSF", 0);
        mixer.SetFloat("CircleSF", 0);

        visualizationMaterial.SetFloat("_Half_Angle",15);
        visualizationMaterial.SetFloat("_Point_SF",0);
        mixer.SetFloat("Mix", 1);
        visualizationMaterial.SetFloat("_Circle_SF",0);
    }

    public void SetModePoint()
    {
        mixer.SetFloat("HalfAngle", Mathf.Deg2Rad*18000);
        mixer.SetFloat("PointSF", 5);
        mixer.SetFloat("CircleSF", 0);
        mixer.SetFloat("Mix", 1);

        visualizationMaterial.SetFloat("_Half_Angle", 18000);
        visualizationMaterial.SetFloat("_Point_SF", 5);
        visualizationMaterial.SetFloat("_Circle_SF", 0);
    }


    public void SetModeCircle() 
    {
        mixer.SetFloat("HalfAngle", Mathf.Deg2Rad * 18000);
        mixer.SetFloat("PointSF", 0);
        mixer.SetFloat("CircleSF", 5);
        mixer.SetFloat("Mix", 1);

        visualizationMaterial.SetFloat("_Half_Angle", 18000);
        visualizationMaterial.SetFloat("_Point_SF", 0);
        visualizationMaterial.SetFloat("_Circle_SF", 5);
    }

    public void SetStep(int i)
    {
        SoundPlayer.instance.ClearAudioSources();
        pointSets[current_set].gameObject.SetActive(false);
        current_set = i;
        pointSets[i].gameObject.SetActive(true);
        SoundPlayer.instance.StartPlaying();
        data.Save();
        data = new DataContainer(probant_identifier, map_identifier, current_set.ToString());
    }
    

    private void Update()
    {
        
        UpdateVisuals();
        ContinueTest();
        

    }


    private void FixedUpdate()
    {
        UpdateData();
    }


    // Method to update the visuals to reflect the views
    private void UpdateVisuals()
    {
        Vector3 direction = gazeDirectionIA.ReadValue<Quaternion>() * Vector3.forward;
        visualizationMaterial.SetVector("_Listener_Position", Camera.main.transform.position);
        visualizationMaterial.SetVector("_View_Direction", direction);

    }


    // Method to 
    private void SetupPaths()
    {
        pointSets.Clear();

        
        int i = 0;

        Transform found_set = transform.Find("Set" + i.ToString());
        while (found_set !=null)
        {
            goalAudioSourceHandler.Add(found_set.Find("TargetAudioSource").gameObject.GetComponent<AudioSourceHandler>());
            pointSets.Add(found_set.gameObject);
            
            found_set.gameObject.SetActive(false);
            i++;
            found_set = transform.Find("Set" + i.ToString());
        }

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


        Vector3 view_direction = gazeDirectionIA.ReadValue<Quaternion>() * Vector3.forward;
        
        float azimuth = Mathf.Atan2(goal_direction.x, goal_direction.z);
        data.AddStep(rel_position, azimuth);
    }
}
