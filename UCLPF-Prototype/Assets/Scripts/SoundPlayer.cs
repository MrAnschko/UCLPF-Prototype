using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class SoundPlayer : MonoBehaviour
{
    [SerializeField]
    float cycleTime;
    [SerializeField]
    float startingOffset; //offset of when the first sound is supposed to be played. Recommended to 
    List<AudioSourceHandler> audioSources;

    public static SoundPlayer instance;

    Coroutine playAll;
    
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Multiple Sound Players in Scene");
            return;
        }

        instance = this;
        audioSources = new List<AudioSourceHandler>();
        Debug.Log(this);
    }
    private void Start()
    {
        StartPlaying();
    }

    // function with which the sources register themselves.
    public void RegisterSource(AudioSourceHandler source)
    {
        audioSources.Add(source);
        Debug.Log($"added {source}");
    }

    public void StartPlaying()
    {
        double offset = startingOffset;
        for (int i = 0; i < audioSources.Count; i++)
        {
            Debug.Log($"Item {i} will be played with offset {offset}");
            audioSources[i].StartPlayingRepeatedly(cycleTime, offset);
            offset += cycleTime / (float)audioSources.Count;
        }
    }

    IEnumerator startAllAudioSources() 
    { 
        yield return null;
    }
}
