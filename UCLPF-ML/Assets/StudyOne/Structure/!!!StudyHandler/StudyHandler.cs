
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
    int pathIndex = -1;
    [SerializeField]
    List<ModeSettings> InteractionModes;

    public static StudyHandler Instance; // is a singleton

    public int PathIndex {
        get => pathIndex;
        set 
        {
            pathIndex = value;
            ProcessInfos.PathCount = value;
        } 
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Multiple study handlers\n Deleting most recent:{this}");
            Destroy(this);
            return;
        }
        Instance = this;
        overallInfo.Load();
        InitiateCombination();

        int count = 0;
        do
        {
            count++;
            ProcessInfos.SetNewID();
        } while (overallInfo.TakenIDs.Contains(ProcessInfos.UserID) && count<1000); // Safeguard. to prevent loop. Shouldn't happen in general but who knows.
        overallInfo.TakenIDs.Add(ProcessInfos.UserID);
        
        

    }
    private void Start()
    {
        IDInformer.StartInformation(StartNextMethod);
    }

    private void OnDestroy()
    {
        overallInfo.Save();
        studyInfo.Save();
    }


    [ContextMenu("Do Something")]
    public void LoadNextPath()
    {
        if(PathIndex < studyInfo.PathOrder.Count-1)
        {
            PathIndex++;
            
            ProcessInfos.UCLPF_MODE = InteractionModes[(int)studyInfo.MethodOrder[PathIndex]];
            ProcessInfos.UCLPF_MODE.ApplySettings();
            ProcessInfos.CurrentMethod = studyInfo.MethodOrder[PathIndex];
            SceneLoading.LoadPath(studyInfo.PathOrder[PathIndex]);

        }
        else
        {
            if(PathHandler.instance!=null)
                PathHandler.instance.UnloadPath();
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
        LoadNextPath();
    }


}
