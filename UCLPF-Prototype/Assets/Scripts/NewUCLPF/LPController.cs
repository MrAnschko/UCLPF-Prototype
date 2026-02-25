using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CustomProcessing))]
public class LPController : MonoBehaviour
{
    [SerializeField] public InputActionReference headGazeDirectionRef;
    [SerializeField] public InputActionReference eyeGazeDirectionRef;
    [SerializeField] bool useEyeGaze;
    [SerializeField] AudioListener listener;

    [Header("FilterProperties")] 
    [SerializeField] float half_angle;
    [SerializeField] float cutoff_initial_freq;
    [SerializeField] float qFactor;

    public bool UseEyeGaze { get => useEyeGaze; set { useEyeGaze = value; SetGazeMethod(); } }



    InputAction gazeDirectionIA;

    CustomProcessing filter;

    private void Start()
    {
        SetGazeMethod();
        
        filter = GetComponent<CustomProcessing>();
        filter.Q = qFactor;
    }

    private void Update()
    {
        UpdateFilter();
    }

    void UpdateFilter()
    {
        // Get View
        Quaternion viewQuaternion = gazeDirectionIA.ReadValue<Quaternion>();
        Vector3 direction = viewQuaternion * Vector3.forward;

        Vector3 listenerPos = listener.transform.position;
        Vector3 obj_pos = transform.position;
        Vector3 obj_dir = obj_pos-listenerPos;

        Debug.Log(listenerPos);

        // Get Object



        // Gaze Direction Method
        float angle = Mathf.Acos(Vector3.Dot(direction, obj_dir) / (direction.magnitude * obj_dir.magnitude));
        Debug.Log($"fejnk: {Vector3.Dot(direction, obj_dir)}, {(direction.magnitude * obj_dir.magnitude)}");
        Debug.Log($"hde: {angle}");
        float cutoff_scale_factor = 1.0f / half_angle;

        // Point Factor


        // Circle Factor


        // total factor
        float goal_cutoff_frequency = cutoff_initial_freq * (1.0f / (1.0f + cutoff_scale_factor * angle));
        Debug.Log(goal_cutoff_frequency);
        filter.Freq = Mathf.Max(11.0f,goal_cutoff_frequency);
    }


    private void SetGazeMethod()
    {
        if (useEyeGaze)
        {
            gazeDirectionIA = eyeGazeDirectionRef.action;
            return;
        }
        gazeDirectionIA = headGazeDirectionRef.action;
        return;
    }
}
