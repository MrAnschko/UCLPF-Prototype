using MixedReality.Toolkit.SpatialManipulation;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class UserLeading : MonoBehaviour
{
    private const float ACCEPTANCE_TIME = 0.5f;


    [SerializeField] Transform leader; // Transform that is used to inform the user about the next step position
    [SerializeField] public Transform StepEnd; // the position that will end the Step. not included in the end step
    [SerializeField] List<Transform> intermediateSteps; // List of intermidiate steps
    int currentGoal = 0;
    [Header("Goal Behavior")]
    [SerializeField] float acceptanceRadius; // the position that will end the Step. not included in the end step
    float timeAtGoal;
    Action callback;
    Action updateFunction;

    Transform GoalTransform { get => (currentGoal<intermediateSteps.Count)? intermediateSteps[currentGoal]:StepEnd;}

    bool NearGoal
    {
        get
        {
            if (User.instance == null)
                return false;
            else
            {
                Vector3 goal = GoalTransform.position;
                Vector3 diff = goal- User.instance.transform.position;
                diff.y = 0; // only count difference on the xz-plane
                return diff.sqrMagnitude < acceptanceRadius * acceptanceRadius;
            }
        }
    }

    // Method to initiate the leading process
    public void StartLeading(Action callbackWhenDone)
    {
        User.instance.DirIndicator.DirectionalTarget = leader;
        User.instance.DirIndicator.gameObject.SetActive(true);
        callback = callbackWhenDone;
        currentGoal = 0;
        leader.position = intermediateSteps[currentGoal].position;
        updateFunction = LeadingFunction;
    }

    // Method that handles the leading update
    public void LeadingFunction() 
    {
        if (NearGoal)
        {
            timeAtGoal += Time.deltaTime;
            if (timeAtGoal > ACCEPTANCE_TIME)
            {
                NextIntermediateStep();
            }
        }
        else
        {
            timeAtGoal = 0;
        }
    }

    public void NextIntermediateStep()
    {
        currentGoal++;
        leader.position = GoalTransform.position;
        if(currentGoal > intermediateSteps.Count) // strictly greater means that we are one after the end step;
        {
            EndLeading();
        }
        
    }

    public void EndLeading()
    {
        User.instance.DirIndicator.gameObject.SetActive(false);

        callback.SafeInvoke();
        updateFunction = null;
            
    }


    private void Update()
    {
        updateFunction.SafeInvoke();
    }

}
