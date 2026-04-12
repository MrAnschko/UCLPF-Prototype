using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathHandler : MonoBehaviour
{
    public static PathHandler instance;
    [Header("Static, Set in Editor")]
    [SerializeField] ProcessInfos.Path path;

    
    [Header("Data")]
    [SerializeField] private Scene PathScene;
    [SerializeField] private PathData pData;
    [SerializeField] private QuestionnaireSO endPathQuestionnaire;
    [Header("Step Handling")]
    [SerializeField] private List<StepHandler> steps = new();
    [SerializeField] private int pathIndex = 0;
    [Header("Audio")]
    [SerializeField] int[] OrderAudioSourceNumber = { 2, 3, 4, 9, 10, 11 };
    [SerializeField] SonificationHandler sonificationHandler;
    

    Action _onEndPath;

    public Action OnEndPath
    {
        get => _onEndPath;
        set
        {
            if (_onEndPath == null)
                _onEndPath = value;
            else
                Debug.LogError("Only One End Step function allowed");
        }
    }

    public int PathIndex 
    { 
        get => pathIndex;
        set
        {
            pathIndex = value;
            ProcessInfos.CurrentStep = value;
        }
    }

    void Awake()
    {
        StartPath();

    }

    private void OnDestroy()
    {
        instance = null;
    }


    public void UnloadPath()
    {
        SceneManager.UnloadSceneAsync(PathScene);
        
    }

    void StartPath()
    {
        if (instance != null)
        {
            Debug.LogError($"Tried Making Multiple Path Handler! \n Deleting most recent: {this}");
            Destroy(this);
        }
        instance = this;
        PathScene = gameObject.scene;
        OnEndPath = StudyHandler.Instance.LoadNextPath; // Make it so that once the path ends the next one is loaded
        PathIndex = 0;
        OrderAudioSourceNumber.Shuffle();
        SetupStep(PathIndex);
        ProcessInfos.timeAtStartPath = Time.time;
        ProcessInfos.currentPath = this.path;
        pData.PathOrder = ProcessInfos.PathCount;
    }

    void ReachedStep()
    {
        pData.Save();
        PathIndex++;
        if(PathIndex >= steps.Count)
        {
            StartPathQuestionnaire();
            return;
        }
        SetupStep(PathIndex);
    }

    void SetupStep(int index)
    {
        steps[index].StartWalk(OrderAudioSourceNumber[index],sonificationHandler);
        
        pData.RegisterStepData(steps[index].stepD);
        steps[index].OnEndStep = ReachedStep;
    }

    void StartPathQuestionnaire()
    {
        // Todo: add Questioning for Users here
        QuestionnaireHandler.StartQuestionnaire(endPathQuestionnaire, EndPath);
    }

    


    void EndPath()
    {
        if (_onEndPath != null)
            _onEndPath();
    }
}
