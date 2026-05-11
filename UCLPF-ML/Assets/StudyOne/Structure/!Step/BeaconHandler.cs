using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to handle beacons WITHIN A STEP
public class BeaconHandler : MonoBehaviour
{

    [Header("Sonification")]
    [SerializeField] SonificationHandler sonification;


    [Header("SceneInformation")]
    [SerializeField] double playDuration;
    [SerializeField] List<Beacon> beaconList;
    [SerializeField] public List<Beacon> activeBeacons;
    [SerializeField] public Beacon highestBeacon;
    [SerializeField] AzimuthTracking highTracking;
    [SerializeField] public Beacon lowestBeacon;
    [SerializeField] AzimuthTracking lowTracking;

    // State management
    Action fUpdate;

    public List<BeaconData> ActiveBeaconsData 
    {
        get
        {
            List<BeaconData> beaconList = new();
            foreach (var beacon in activeBeacons) 
            {
                beaconList.Add(beacon.beaconData);
            }
            return beaconList;
        }
    }

    public SonificationHandler Sonification { get => sonification; set => sonification = value; }

    public void SelectBeacons(int nBeacons)
    {
        // TODO: Make Function to select subset of beacons
        List<SonificationClip> clips = sonification.AudioClips;
        clips.Shuffle();
        clips.RemoveRange(0, clips.Count-nBeacons);

        // Selects a random subselection of positions and assigns random ints to them
        beaconList.Shuffle();
        lowestBeacon = beaconList[0]; // set first as lowest & highest
        highestBeacon = beaconList[0];
        for (int i = 0; i < beaconList.Count; i++) 
        { 

            if (i < nBeacons)
            {
                activeBeacons.Add(beaconList[i]);
                beaconList[i].Setup(clips[i]);
                lowestBeacon = (lowestBeacon.sonClip < beaconList[i].sonClip)? lowestBeacon: beaconList[i];
                highestBeacon = (highestBeacon.sonClip > beaconList[i].sonClip)? highestBeacon : beaconList[i];
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
        float cycle = ProcessInfos.START_DIFF* activeBeacons.Count;
        for (int i = 0; i < activeBeacons.Count; i++, offset += ProcessInfos.START_DIFF)
        {
            activeBeacons[i].StartPlayingRepeatedly(cycle,offset, playDuration);
        }
        StartDataLogging();
    }

    public void StartDataLogging()
    {
        highTracking = new AzimuthTracking(highestBeacon.gameObject,"high");
        lowTracking = new AzimuthTracking(lowestBeacon.gameObject,"low");
    }



    public void StopAudio()
    {
        //Function to STOP all (active) audio Beacons
        highTracking.Save();
        LoggingManager.DeRegisterLogger(highTracking);
        highTracking = null;
        lowTracking.Save();
        LoggingManager.DeRegisterLogger(lowTracking);
        lowTracking = null;

        foreach (var beacon in activeBeacons) { beacon.StopPlaying(); }
        
    }

    private void FixedUpdate()
    {
        fUpdate.SafeInvoke();
    }

}
