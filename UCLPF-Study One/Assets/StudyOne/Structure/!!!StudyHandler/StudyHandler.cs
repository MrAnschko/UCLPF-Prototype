using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


// Class that handles the overall Structure of the Study

public class StudyHandler : MonoBehaviour
{
    [SerializeField]
    OveralDC overallInfo;
    [SerializeField]
    StudyInfo studyInfo = new();

    [SerializeField]
    int testOrder = 0;

    public static StudyHandler Instance; // is a singleton

    private void Start()
    {
        if (Instance != null)
        {
            Debug.LogError($"Multiple study handlers\n Deleting most recent:{this}");
            Destroy(this);
            return;
        }
        overallInfo.Load();
        InitiateCombination();



    }

    private void OnDestroy()
    {
        overallInfo.Save();
        studyInfo.Save();
    }
    // A method to set the Participant id
    public void SetParticipantID(string ID)
    {
        studyInfo.UserIdentifier = ID;
        
    }

    public void InitiateCombination()
    {
        studyInfo.MethodOrder = ChooseCombination();
        overallInfo.AddOccurrence(studyInfo.MethodOrder);
    }

    // Method to chose a combination of Path and 
    public List<ProcessInfos.InteractionMethod> ChooseCombination()
    {
        // Get all mins
        List<int> mins = overallInfo.MinimumMethodOrdersIndex();
        //
        int choice = Random.Range(0,mins.Count);

        return ProcessInfos.IntToMethodOrder(mins[choice]);
    }

    // Called when the next 
    public void StartNextMethod()
    {

    }
}
