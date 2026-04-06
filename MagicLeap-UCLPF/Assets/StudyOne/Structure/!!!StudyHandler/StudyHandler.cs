
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
    int PathIndex = -1;

    public static StudyHandler Instance; // is a singleton

    private void Awake()
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

    [ContextMenu("Do Something")]
    private void LoadNextPath()
    {
        if(PathIndex < studyInfo.PathOrder.Count-1)
        {
            PathIndex++;
            SceneLoading.LoadPath(studyInfo.PathOrder[PathIndex]);

        }
        else
        {
            PathHandler.instance?.UnloadPath();
        }
        
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

    // Called when the next Scene is to be Loaded
    public void StartNextMethod()
    {

    }
}
