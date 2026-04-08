using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CustomProcessing))]
public class LPController : MonoBehaviour, LPSettingsInformer
{
    [SerializeField] protected GameObject listener;

    [Header("Filter Properties")]
    [SerializeField] protected float qFactor;
    [SerializeField] protected float cutoffInitialFreq;
    [SerializeField] protected float cutoffMinimalFreq;
    [Header("Interaction Properties")]
    [SerializeField] protected float half_angle;
    [SerializeField] protected float pointDistFactor;
    [SerializeField] protected float circleDistFactor;
    [SerializeField] protected float horizonBehavior;
    [SerializeField] protected float planePosition = -1;


    public void UpdateSettings()
    {
        listener = LPGlobalSettings.PointObject;
        qFactor = LPGlobalSettings.QFactor;
        cutoffInitialFreq = LPGlobalSettings.CutoffInitialFreq;
        cutoffMinimalFreq = LPGlobalSettings.CutoffMinimalFreq;
        half_angle = LPGlobalSettings.Half_angle;
        pointDistFactor = LPGlobalSettings.PointDistFactor;
        circleDistFactor = LPGlobalSettings.CircleDistFactor;
        horizonBehavior = LPGlobalSettings.HorizonBehavior;
        planePosition = LPGlobalSettings.PlanePosition;

    }


    static float horizon_alpha(float t, float original_factor)
    {
        return (t > 1.0f) ? t * AudioSettings.outputSampleRate * 0.5f : (1.0f - t) * original_factor; //use Samplerate as max, therefore if above horizon it may dampen.
    }



    CustomProcessing filter;

    private void Start()
    {
        LPGlobalSettings.RegisterSelf(this);
        listener = LPGlobalSettings.PointObject;
        UpdateSettings();
        filter = GetComponent<CustomProcessing>();
        filter.Q = qFactor;
    }

    private void Update()
    {
        UpdateFilter();
    }

    private void OnDestroy()
    {
        LPGlobalSettings.UnregisterSelf(this);
    }

    void UpdateFilter()
    {
        // Get View
       
        
        // Get Object



        //// Gaze Direction Method
        //float angle = Mathf.Acos(Vector3.Dot(direction, obj_dir) / (direction.magnitude * obj_dir.magnitude));
        //Debug.Log($"fejnk: {Vector3.Dot(direction, obj_dir)}, {(direction.magnitude * obj_dir.magnitude)}");
        //Debug.Log($"hde: {angle}");
        //float cutoff_scale_factor = 1.0f / half_angle;

        //listener matrix:
        Matrix4x4 m = listener.transform.worldToLocalMatrix;
        // Object matrix
        Matrix4x4 s = transform.localToWorldMatrix;


        float px = s[12];
        float py = s[13];
        float pz = s[14];

        float dir_x = m[0] * px + m[4] * py + m[8] * pz + m[12];
        float dir_y = m[1] * px + m[5] * py + m[9] * pz + m[13];
        float dir_z = m[2] * px + m[6] * py + m[10] * pz + m[14];

        //Position (of the listener) Creating the last column of the inverse of m. (-> a matrix that transforms from listener to world coordinates.)
        float l_x = -(m[12] * m[0] + m[13] * m[1] + m[14] * m[2]);
        float l_y = -(m[12] * m[4] + m[13] * m[5] + m[14] * m[6]);
        float l_z = -(m[12] * m[8] + m[13] * m[9] + m[14] * m[10]);

        //direction of view 
        // a forward view vector is (0,0,1,0) in listener coordinates
        // (last one is zero to avoid translation) to transform that forward vector from listener to world the inverse of m is used.
        // the inverse of the upper left 3x3 block of m (homogenous matrix) is simply its transpose
        // accordingly multiplication results in the 3rd column vector of m.
        float d_x = m[2];
        float d_y = m[6];
        float d_z = m[10];


        // Point Factor
        // Distance Calculation
        //// Based on forward vector is (0,0,1) dot product of fwd vector and sourcedir accordingly is simply the z direction
        float angle = Mathf.Abs(Mathf.Acos(dir_z / Mathf.Sqrt(dir_x * dir_x + dir_y * dir_y + dir_z * dir_z + 0.001f))); //angle is given in Radians
        float cutoff_scale_factor = 1 / half_angle; // a scale factor for how much the distance affects the frequency


        //// Based on Point on plane
        // Intersection
        float c_dist, p_dist;

        float alpha = (d_y < -0.001f) ? (planePosition - l_y) / d_y : 0; // set the alpha to zero in case there is no (positive) intersection

        // position on the plane
        float g_x = alpha * d_x + l_x;
        float g_z = alpha * d_z + l_z;

        // distance from point: (on plane)
        p_dist = Mathf.Sqrt((g_x - px) * (g_x - px) + (g_z - pz) * (g_z - pz));

        //// Point on plane end

        // Circle:
        // distance of object to listener (along plane)
        float l_dist = Mathf.Sqrt((l_x - px) * (l_x - px) + (l_z - pz) * (l_z - pz));
        float l_p_dist = Mathf.Sqrt((g_x - l_x) * (g_x - l_x) + (g_z - l_z) * (g_z - l_z));
        c_dist = Mathf.Abs(l_dist - l_p_dist);
        
        // End Circle
        if (!(d_y < -0.001f))
        {
            p_dist = horizon_alpha(horizonBehavior, p_dist);
            c_dist = horizon_alpha(horizonBehavior, c_dist);
        }

        

        // total factor
        float goal_cutoff_frequency = cutoffInitialFreq * (1.0f / (1.0f + c_dist * circleDistFactor + p_dist * pointDistFactor + cutoff_scale_factor * angle));

        Debug.Log(goal_cutoff_frequency);
        filter.Freq = Mathf.Max(cutoffMinimalFreq,goal_cutoff_frequency);
    }

}
