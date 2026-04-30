
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


// Class that handles the overall Structure of the Study
[RequireComponent(typeof(LoggingManager))]
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
    [SerializeField] List<QuestionnaireSO> endStudies;
    IEnumerator<QuestionnaireSO> qEnumerator;
    LatinSquare<ProcessInfos.InteractionMethod> InteractionLatinSquare;
    [SerializeField] GameObject ThankMessage;
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
        InteractionLatinSquare = new();
        InteractionLatinSquare.Load();

        InitiateCombination();
        

        int count = 0;
        do
        {
            count++;
            ProcessInfos.SetNewID();
        } while (overallInfo.TakenIDs.Contains(ProcessInfos.UserID) && count<1000); // Safeguard. to prevent loop. Shouldn't happen in general but who knows.
        overallInfo.TakenIDs.Add(ProcessInfos.UserID);
        overallInfo.Save();
        studyInfo.Save();


    }
    private void Start()
    {

        CustomDebug.Log("Starting");
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
            LoggingManager lm = gameObject.GetComponent<LoggingManager>();
            if (lm != null)
                lm.Clear();
            ProcessInfos.CurrentMethod = studyInfo.MethodOrder[PathIndex];
            SceneLoading.LoadPath(studyInfo.PathOrder[PathIndex]);
            
        }
        else
        {
            if(PathHandler.instance!=null)
                PathHandler.instance.UnloadPath();
            StartQuestionnare();
        }
        
    }

    [ContextMenu("Start Questionnaire")]
    public void StartQuestionnare()
    {
        if (endStudies.Count > 0) 
        {
            qEnumerator = endStudies.GetEnumerator();
            NextQuestionnaire();
        }
    }

    [ContextMenu("Next Questionnaire")]

    public void NextQuestionnaire()
    {
        if (qEnumerator.MoveNext())
        {
            QuestionnaireHandler.StartQuestionnaire(qEnumerator.Current, NextQuestionnaire);
        }
        else
        {
            ShowEndMessage();
        }
    }

    public void InitiateCombination()
    {
        studyInfo.MethodOrder = ChooseCombination();
        // Comment: Making Same path order very time.
;
        overallInfo.AddOccurrence(studyInfo.MethodOrder);
    }

    // Method to chose a combination of Path and 
    public List<ProcessInfos.InteractionMethod> ChooseCombination()
    {

        // TODO: Insert Latin Square/Cube/Whatever here to refine method
        List<ProcessInfos.InteractionMethod> order = InteractionLatinSquare.Choice();
        InteractionLatinSquare.Save();
        return order;
    }

    // Called when the next Scene is to be Loaded
    public void StartNextMethod()
    {
        LoadNextPath();
    }

    public void ShowEndMessage()
    {
        ThankMessage.SetActive(true);
    }

    public void EndStudy()
    {
        Debug.Log("End Application");
        Application.Quit();
    }
}
