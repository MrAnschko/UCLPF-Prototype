using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BeaconHandler))]
// Handler to start a new step
public class StepHandler : MonoBehaviour
{
    public static float ACCEPTANCE_TIME = 0.5f;
    [Header("Data")]
    // Data to save
    public StepData stepD = new();
    public PositionTracking posTr;
    public QuestionnaireSO EndPathQuestionnaire;

    [Header("Goal Behavior")]
    public Transform Goal;
    public float acceptanceRadius;
    float _timeInGoal;

    [Header("Beacons")]
    public BeaconHandler beaconHandler;



    private void Awake()
    {
        beaconHandler = GetComponent<BeaconHandler>();
    }

    bool NearGoal {
        get 
        {
            if( User.instance == null)
                return false;
            else
            {
                Vector3 diff = Goal.position - User.instance.transform.position;
                diff.y = 0; // only count difference on the xz-plane
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



    // ----------------------------------------------------- START state handling ------------------------------------------------------------

    // Function to start the Step
    public void StartWalk(int numberSources, SonificationHandler sonification)
    {
        ProcessInfos.timeAtStartStep = Time.time;
        this.gameObject.SetActive(true);
        beaconHandler.Sonification = sonification;
        beaconHandler.SelectBeacons(numberSources);
        beaconHandler.StartAudio();
        _onUpdate = DuringWalking;
        // Data Collection
        stepD.StepEnd = Goal.position;
        stepD.Beacons = beaconHandler.ActiveBeaconsData;

        stepD.Save();
        posTr = new PositionTracking(Goal.position);

    }

    // Function to be called while the 
    public void DuringWalking()
    {
        posTr.AddData();
        if (NearGoal)
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
        // STOP sounds;
        beaconHandler.StopAudio();
        // go to Questionnaire
        StartStepQuestionnaire();

        posTr.Save();
    }

    public void StartPointTask()
    {
        
    }


    public void EndPointTask()
    {

    }

    public void StartStepQuestionnaire()
    {
        QuestionnaireHandler.StartQuestionnaire(EndPathQuestionnaire, EndStepQuestionnaire);
        //_onUpdate += DuringStepQuestionnaire;
    }

    public void DuringStepQuestionnaire()
    {

    }

    public void EndStepQuestionnaire()
    {
        _onUpdate -= DuringStepQuestionnaire;
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

    // ----------------------------------------------------- END state handling ------------------------------------------------------------
}
