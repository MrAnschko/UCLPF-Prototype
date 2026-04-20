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
    [SerializeField] private QuestionnaireSO nasaTLX;
    [Header("Step Handling")]
    [SerializeField] private List<StepHandler> steps = new();
    [SerializeField] private int pathIndex = 0;
    [Header("Audio")]
    [SerializeField] List<ProcessInfos.BeaconClass> OrderAudioSourceClass;
    [SerializeField] SonificationHandler sonificationHandler;
    
    public static Beacon high
    {
        get
        {
            return instance.steps[instance.pathIndex].beaconHandler.highestBeacon;
        }
    }

    public static Beacon low
    {
        get
        {
            return instance.steps[instance.pathIndex].beaconHandler.lowestBeacon;
        }
    }

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
        if (instance != null)
        {
            Debug.LogError($"Tried Making Multiple Path Handler! \n Deleting most recent: {this}");
            Destroy(this);
        }
        instance = this;
        PathScene = gameObject.scene;
        //GetStartMenu();
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
    void GetStartMenu()
    {
        GetStartPoint.StartInformation(StartPath);
    }

    void StartPath()
    {

        OnEndPath = StudyHandler.Instance.LoadNextPath; // Make it so that once the path ends the next one is loaded
        PathIndex = 0;


        OrderAudioSourceClass.Shuffle();
        OrderAudioSourceClass.Insert(0, ProcessInfos.BeaconClass.Many);
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

        int numberBeacons = ChooseSourceNumber(OrderAudioSourceClass[index]);
        steps[index].StartWalk(numberBeacons, sonificationHandler);
        
        pData.RegisterStepData(steps[index].stepD);
        steps[index].OnEndStep = ReachedStep;
    }

    [ContextMenu("StartQuestionnaire")]
    void StartPathQuestionnaire()
    {
        
        QuestionnaireHandler.StartQuestionnaire(endPathQuestionnaire, StartNTLX);
    }

    void StartNTLX()
    {
        if (nasaTLX != null)
        {
            QuestionnaireHandler.StartQuestionnaire(nasaTLX, EndPath);
        }
        else
        {
            EndPath();
        }
    }


    // choose number of beacons. 
    // 2-4 for few beacons 6-8 for many
    int ChooseSourceNumber(ProcessInfos.BeaconClass bClass)
    {
        ProcessInfos.currentBeaconClass = bClass;
        int number = UnityEngine.Random.Range(0, 2);
        number += 2 + (int)bClass * 4; // min total 2 + 0 (low number) or 4 (many ) -> 2-4 (low) or 6-8 (many)
        return number;
    }

    void EndPath()
    {
        
            _onEndPath?.Invoke();
    }


}
