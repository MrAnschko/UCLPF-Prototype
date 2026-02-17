using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class SoundPlayer : MonoBehaviour
{
    [SerializeField]
    float cycleTime;
    [SerializeField]
    int simultaneous_sources; // the number of simultaneous sources which are played.
    [SerializeField]
    float startingOffset; //offset of when the first sound is supposed to be played. Recommended to 
    List<AudioSourceHandler> audioSources;
    [SerializeField]
    SonificationHandler sonificationMethod;

    public static SoundPlayer instance;

    Coroutine playAll;

    static public void SetupAudioSource(AudioSourceHandler ASH)
    {
        if (instance == null || instance.sonificationMethod == null)
        {
            Debug.LogError("Soundplayer not Setup correctly");
            return;
        }

        ASH.AudioSource.clip = instance.sonificationMethod.GetClipFromID(ASH.AudioID);
    }
    

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


    // function with which the sources register themselves.
    public static void RegisterSource(AudioSourceHandler source)
    {
        instance.audioSources.Add(source);
        Debug.Log($"added {source}");
    }

    public void ClearAudioSources()
    {
        audioSources.Clear();
    }


    public void StartPlaying()
    {
        double offset = startingOffset;
        for (int i = 0; i < audioSources.Count;)
        {
            Debug.Log($"Item {i} will be played with offset {offset}");
            for(int j = 0; j<simultaneous_sources & i <audioSources.Count;j++){
                audioSources[i].StartPlayingRepeatedly(cycleTime, offset);
                i++;
            }
            offset += cycleTime / (float)audioSources.Count;
        }
    }


}
