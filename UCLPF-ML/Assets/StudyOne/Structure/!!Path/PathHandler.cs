using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathHandler : MonoBehaviour
{
    public static PathHandler instance;

    [SerializeField]
    private Scene PathScene;
    [SerializeField]
    private PathData pData;
    [Header("Step Handling")]
    [SerializeField] private List<StepHandler> steps = new();
    [SerializeField] private int pathIndex;

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
        pathIndex = 0;
        SetupStep(pathIndex);
    }

    void ReachedStep()
    {
        pData.Save();
        pathIndex++;
        if(pathIndex >= steps.Count)
        {
            StartPathQuestionnaire();
            return;
        }
        SetupStep(pathIndex);
    }

    void SetupStep(int index)
    {
        steps[index].StartWalk();
        pData.RegisterStepData(steps[index].stepD);
        steps[index].OnEndStep = ReachedStep;
    }

    void StartPathQuestionnaire()
    {
        // Todo: add Questioning for Users here
        EndPath(); // Place may change
    }

    


    void EndPath()
    {
        if (_onEndPath != null)
            _onEndPath();
    }
}
