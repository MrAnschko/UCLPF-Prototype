using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))]
public class AudioLowpassTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Testing Calculations
        Matrix4x4 w2l = Camera.main.transform.worldToLocalMatrix;
        // Need this matrix, as the other is not inherently available.

        //Position
        float p_x = -(w2l[12] * w2l[0] + w2l[13] * w2l[1] + w2l[14] * w2l[2]);
        float p_y = -(w2l[12] * w2l[4] + w2l[13] * w2l[5] + w2l[14] * w2l[6]);
        float p_z = -(w2l[12] * w2l[8] + w2l[13] * w2l[9] + w2l[14] * w2l[10]);
        Vector3 pos = new Vector3(p_x, p_y, p_z);
        
        //direction
        float d_x = w2l[2];
        float d_y = w2l[6];
        float d_z = w2l[10];
        Vector3 dir = new Vector3(d_x, d_y, d_z);
        // Intersection
        float alpha = (d_y < -0.001f) ? (-1 - p_y) / d_y : 0;

        
        Vector3 view_pos = pos + alpha * dir;



        float cutoff_freq = 22000 * 1 / (1 + (view_pos - this.transform.position).magnitude);
        Debug.Log(cutoff_freq);

        GetComponent<AudioLowPassFilter>().cutoffFrequency = cutoff_freq;

    }
}
