using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BeaconHandler)),RequireComponent(typeof(UserLeading))]
// Handler to start a new step
public class StepHandler : MonoBehaviour
{
    public const float ACCEPTANCE_TIME = 0.5f;
    [Header("Data")]
    // Data to save
    public StepData stepD = new();
    public PositionTracking posTr;
    public LookBehaviour lookBehaviour;
    [DoNotSerialize] EyeTrackingData eyeTracking;
    public QuestionnaireSO EndPathQuestionnaire;

    [Header("Leading")]
    [SerializeField] UserLeading leading;

    [Header("Beacons")]
    public BeaconHandler beaconHandler;
    



    

    Action _onFixedUpdate; // Function that is called when updating -> changable in order to change behavior until goal is reached
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
        beaconHandler = GetComponent<BeaconHandler>();
        leading = GetComponent<UserLeading>();

        ProcessInfos.timeAtStartStep = Time.time;
        this.gameObject.SetActive(true);
        beaconHandler.Sonification = sonification;
        beaconHandler.SelectBeacons(numberSources);
        beaconHandler.StartAudio();
        // Data Collection
        stepD.StepEnd = CustomWorldOrigin.PosUnityToMap(leading.StepEnd.position);
        stepD.intermediatePoints = new();
        foreach (Transform tf in leading.intermediateSteps)
            stepD.intermediatePoints.Add(CustomWorldOrigin.PosUnityToMap(tf.position));
        stepD.Beacons = beaconHandler.ActiveBeaconsData;

        stepD.Save();
        posTr = new PositionTracking(leading.StepEnd.position);
        eyeTracking = new();
        lookBehaviour = new();
        lookBehaviour.StartLogging();
        leading.StartLeading(EndWalk);

    }

    public void EndWalk()
    {

        // STOP sounds;
        beaconHandler.StopAudio();
        stepD.Save();
        posTr.Save();
        lookBehaviour.StopLogging();


        LoggingManager.DeRegisterLogger(posTr);
        posTr = null;
        eyeTracking.Save();
        LoggingManager.DeRegisterLogger(eyeTracking);
        eyeTracking = null;
        // go to Questionnaire
        StartStepQuestionnaire();


    }

    public void StartPointTask()
    {
        
    }


    public void EndPointTask()
    {

    }

    [ContextMenu("StartQuestionnaire")]
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
        //_onUpdate -= DuringStepQuestionnaire;
        
        EndStep();
    }


    // Function to end the step
    public void EndStep()
    {
        this.gameObject.SetActive(false);
        _onEndStep.SafeInvoke();
    }

    private void FixedUpdate()
    {

        _onFixedUpdate.SafeInvoke(); // As behaviour may change based on which state we are in, the update function will be changed accordingly.
    }

    // ----------------------------------------------------- END state handling ------------------------------------------------------------
}
