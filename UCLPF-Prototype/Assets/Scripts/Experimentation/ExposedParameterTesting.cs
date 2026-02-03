using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ExposedParameterTesting : MonoBehaviour
{

    [SerializeField] AudioMixer audioMixer;
    // Start is called before the first frame update
    
    // Update is called once per frame
    void FixedUpdate()
    {
        float val = 0.0f;
        audioMixer.SetFloat("MixParameter",Mathf.Abs(Mathf.Sin(Time.timeSinceLevelLoad)));
        audioMixer.GetFloat("MixParameter",out val);
        Debug.Log(val);
    }
}
