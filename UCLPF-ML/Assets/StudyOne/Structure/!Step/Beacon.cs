using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Class to handle the audio Beacons
[RequireComponent(typeof(AudioSource))]
public class Beacon : MonoBehaviour
{
    static double safety_time = 0.10d;


    AudioSource audioSource;
    SonificationClip sonClip;
    AudioClip clip;
    public float Pitch = 0;
    Coroutine playing; // the coroutine for playing 

    public AudioSource AudioSource { get => audioSource; set => audioSource = value; }


    public void Setup(SonificationClip sonClip)
    {
        audioSource = GetComponent<AudioSource>();

        this.sonClip = sonClip;
        audioSource.clip = sonClip.clip;
        Pitch = sonClip.frequency;
    }

    public void StartPlayingRepeatedly(float cycletime, double offset)
    {
        playing = StartCoroutine(PlayRepeatedly(cycletime, offset));
    }

    public void StopPlaying()
    {
        if (playing != null)
        {
            StopAllCoroutines();
            audioSource.Stop();
        }
        
    }

    public void OnDisable()
    {
        StopPlaying();
    }


    IEnumerator PlayRepeatedly(float cycleTime, double offset)
    {

        
        for (double time = AudioSettings.dspTime + offset; ; time += cycleTime)
        {
            yield return new WaitUntil(() => (AudioSettings.dspTime >= time - safety_time));
            
            audioSource.PlayScheduled(time);
        }
    }
}
