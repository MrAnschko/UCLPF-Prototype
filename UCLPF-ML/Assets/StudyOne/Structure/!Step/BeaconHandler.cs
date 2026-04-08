using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to handle beacons WITHIN A STEP
public class BeaconHandler : MonoBehaviour
{

    [Header("Sonification")]
    [SerializeField] SonificationHandler sonification;
    

    [Header("SceneInformation")]
    [SerializeField] List<Beacon> beaconList;
    [SerializeField] List<Beacon> activeBeacons;
    [SerializeField] Beacon highestBeacon; 
    [SerializeField] Beacon lowestBeacon;

    public SonificationHandler Sonification { get => sonification; set => sonification = value; }

    public void SelectBeacons(int nBeacons)
    {
        // TODO: Make Function to select subset of beacons
        List<SonificationClip> clips = sonification.AudioClips;
        clips.Shuffle();
        clips.RemoveRange(0, clips.Count-nBeacons);

        // Selects a random subselection of positions and assigns random ints to them
        beaconList.Shuffle();
        for (int i = 0; i < beaconList.Count; i++) 
        { 
            if (i < nBeacons)
            {
                activeBeacons.Add(beaconList[i]);
                beaconList[i].Setup(clips[i]);
            }
            else
            {
                beaconList[i].gameObject.SetActive(false);
            }
            
        }

    }

    public void StartAudio()
    {
        float offset = 0;
        float cycle = ProcessInfos.CYCLE_TIME;
        for( int i = 0; i<activeBeacons.Count;i++,offset+=cycle/activeBeacons.Count)
        {
            activeBeacons[i].StartPlayingRepeatedly(cycle,offset);
        }
    }

    public void StopAudio()
    {
        //Function to STOP all (active) audio Beacons
        foreach (var beacon in activeBeacons) { beacon.StopPlaying(); }
    }

}
