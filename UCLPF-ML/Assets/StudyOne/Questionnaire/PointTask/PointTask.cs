using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


// Class that handles a Point Task
public class PointTaskHandler : MonoBehaviour
{
    static PointTaskHandler instance;

    // Input Stuff
    [SerializeField]InputActionReference ConfirmationReference;
    InputAction ConfirmationAction;

    // Data Saving
    PointTaskDC pointData;

    // Internal State
    bool isActive;
    Action<Vector2> taskCompleted; // callback to signal that a task has been completed

    private void Awake()
    {
        Debug.Log(Vector2.left.ToString());
        if( instance != null)
        {
            Debug.LogError($"Tried making Multiple Point Task Handlers\n Deleting most Recent {this}");
        }
        instance = this;
        ConfirmationAction = ConfirmationReference.action;
        ConfirmationAction.performed += ConfirmPoint;
    }

    public static void BeginTask(string desc, Action<Vector2> CompletedCallback)
    {
        
        instance.pointData = new(desc);
        instance.isActive = true;
        instance.taskCompleted = CompletedCallback;
    }

    [ContextMenu("ConfirmPoint")]
    void EndTask()
    {
        if (isActive)
        {
            Vector2 response = Gaze.GetGroundPlaneIntersection();
            isActive = !isActive;
            pointData.point = response;
            pointData?.Save();
            taskCompleted.SafeInvoke(response);
        }
    }

    
    public void ConfirmPoint(InputAction.CallbackContext context)
    {
        EndTask();
    }

    
}
