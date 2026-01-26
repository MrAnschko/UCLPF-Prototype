using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// Behaviour script to be attached to a source for testing.
[DefaultExecutionOrder(0)]
public class AudioSourceHandler : MonoBehaviour
{

    AudioSource audioSource;
    AudioClip clip;
    Coroutine playing; // the coroutine for playing 
    
    

    private void Awake()
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
        for(double time=offset; ; time+=cycleTime)
        {
            Debug.Log($"{this} will be played {time}");
            audioSource.PlayScheduled(time);
            yield return new WaitForSeconds(cycleTime);
        }
    }
}
