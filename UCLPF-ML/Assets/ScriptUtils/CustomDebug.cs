using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomDebug : MonoBehaviour
{
    static CustomDebug instance;
    [SerializeField] TMP_Text OutputTMP;

    [Header("Controller Stuff")]
    [SerializeField] InputActionReference controllerRotAR;
    InputAction controllerRotA;
    [SerializeField] InputActionReference controllerPosAR;
    InputAction controllerPosA;

    void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Created Two Custom Debugs");
            Destroy(this);
            return;
        }
        instance = this;
        controllerPosA = controllerPosAR.action;
        controllerRotA = controllerRotAR.action;
    }

    public static void Log(string msg)
    {
        instance.OutputTMP.text = msg;
    }

    private void Update()
    {
        transform.rotation = controllerRotA.ReadValue<Quaternion>();
        transform.position = controllerPosA.ReadValue<Vector3>();
    }
}
