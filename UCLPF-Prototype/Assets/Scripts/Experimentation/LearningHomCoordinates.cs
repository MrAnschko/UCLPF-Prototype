using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearningHomCoordinates : MonoBehaviour
{
    public GameObject gaze_point_indicator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Matrix4x4 l2w = Camera.main.transform.localToWorldMatrix;
        Vector4 testing = l2w.MultiplyPoint(Vector3.zero); // w2l -> l2w -> origin in local to origin in world?
        
        Matrix4x4 w2l = transform.worldToLocalMatrix;
        Matrix4x4 l2w_d = transform.localToWorldMatrix;
        Debug.Log($"w2l: \n {w2l}");
        //Debug.Log($"w2l_t calc: \n {w2l_t.MultiplyPoint(Vector3.zero)}");
        Debug.Log($"l2w direct: \n {l2w_d}");
        
        
        //Vector4 local_pos = w2l_t.GetRow(3);
        //local_pos[3] = 0;
        //Debug.Log($"last row calculation: \n {-w2l_t.MultiplyVector(local_pos)}");
        ////Debug.Log($"l2w_d calc: \n {l2w_d.MultiplyPoint(Vector3.zero)}");
        

    }

    private void OnDrawGizmos()
    {
        //Testing Calculations
        Matrix4x4 w2l = Camera.main.transform.worldToLocalMatrix;
        // Need this matrix, as the other is not inherently available.

        //Position
        float p_x = -(w2l[12] * w2l[0] + w2l[13] * w2l[1]+w2l[14] *w2l[2]);
        float p_y = -(w2l[12] * w2l[4] + w2l[13] * w2l[5]+w2l[14] *w2l[6]);
        float p_z = -(w2l[12] * w2l[8] + w2l[13] * w2l[9]+w2l[14] *w2l[10]);
        Vector3 pos = new Vector3(p_x, p_y, p_z);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pos, 0.3f);

        //direction
        float d_x = w2l[2];
        float d_y = w2l[6];
        float d_z = w2l[10];
        Vector3 dir = new Vector3(d_x, d_y, d_z);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(pos, dir*10);

        // Intersection
        float alpha = (d_y <-0.001f)? (-1-p_y)/d_y:0;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(pos+alpha*dir, 0.3f);
        gaze_point_indicator.transform.position = pos + alpha * dir;

    }

    private void FixedUpdate()
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

        gaze_point_indicator.transform.position = pos + alpha * dir;
    }
}
