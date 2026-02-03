using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// Behaviour script to be attached to a source for testing.
[DefaultExecutionOrder(0)]
public class AudioSourceHandler : MonoBehaviour
{
    static double safety_time = 0.10d;


    AudioSource audioSource;
    AudioClip clip;
    Coroutine playing; // the coroutine for playing 
    
    

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        
        clip = audioSource.clip;
        SoundPlayer.instance.RegisterSource(this);
        
    }

    public void StartPlayingRepeatedly(float time,double offset)
    {
     
        playing = StartCoroutine(PlayRepeatedly(time,offset));
    }

    public void OnDisable()
    {
        if (playing != null)
        {
            
            StopAllCoroutines();
        }
    }


    IEnumerator PlayRepeatedly(float cycleTime, double offset)
    {

        //Debug.Log($"Starting Delay: {(float)(offset - safety_time)}");
        for (double time= AudioSettings.dspTime+offset;;time+=cycleTime)
        {
            yield return new WaitUntil(()=>(AudioSettings.dspTime >= time -safety_time));
            //Debug.Log($"{this} will be played {time}.\n Current Time: {AudioSettings.dspTime}");
            audioSource.PlayScheduled(time);
        }
    }
}
