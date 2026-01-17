using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearningHomCoordinates : MonoBehaviour
{
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
}
