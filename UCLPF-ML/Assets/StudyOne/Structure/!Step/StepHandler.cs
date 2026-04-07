using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handler to start a new step
public class StepHandler : MonoBehaviour
{
    public static float ACCEPTANCE_TIME = 0.5f;
    [Header("Data")]
    // Data to save
    public StepData stepD;

    [Header("Goal Behavior")]
    public Transform Goal;
    public float acceptanceRadius;


    float _timeInGoal;

    bool near_goal {
        get 
        {
            if( User.instance == null)
                return false;
            else
            {
                Vector3 diff = Goal.position - User.instance.transform.position;
                return diff.sqrMagnitude < acceptanceRadius * acceptanceRadius;
            }
        }
    }

    

    Action _onUpdate; // Function that is called when updating -> changable in order to change behavior until goal is reached
    Action _onEndStep; // Internal Function to be called when a step has ended;

    public Action OnEndStep {
        get => _onEndStep; 
        set
        {
            if (_onEndStep == null)
                _onEndStep = value;
            else
                Debug.LogError("Only One End Step function allowed");
        }
    }

    // Function to start the Step
    public void StartWalk()
    {
        
        this.gameObject.SetActive(true);
        _onUpdate = DuringWalking;
    }

    // Function to be called while the 
    public void DuringWalking()
    {
        if (near_goal)
        {
            _timeInGoal += Time.deltaTime;
            if( _timeInGoal > ACCEPTANCE_TIME)
            {
                EndWalk();
            }
        }
        else
        {
            _timeInGoal = 0;
        }
    }

    public void EndWalk()
    {
        // for now just start the Questionnaire
        _onUpdate -= DuringWalking;
        // go to Questionnaire
        StartStepQuestionnaire();
    }

    public void StartStepQuestionnaire()
    {
        _onUpdate += DuringStepQuestionnaire;
    }

    public void DuringStepQuestionnaire()
    {
        // TODO: make questionnaire
        EndStepQuestionnaire();
    }

    public void EndStepQuestionnaire()
    {
        _onUpdate -= DuringStepQuestionnaire;
        // TODO: Saving
        EndStep();
    }


    // Function to end the step
    public void EndStep()
    {
        this.gameObject.SetActive(false);
        _onEndStep.SafeInvoke();
    }

    private void Update()
    {

        _onUpdate.SafeInvoke(); // As behaviour may change based on which state we are in, the update function will be changed accordingly.
    }
}
