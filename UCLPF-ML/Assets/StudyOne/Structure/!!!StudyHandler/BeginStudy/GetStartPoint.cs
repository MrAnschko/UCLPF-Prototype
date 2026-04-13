using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class GetStartPoint : MonoBehaviour
{
    [SerializeField] TMP_Text TextField;
    [SerializeField] GameObject Menu;
    [SerializeField] GameObject Test;
    [SerializeField] InputActionReference ConfirmButton;
    InputAction confirmIA;
    Action DoneCallback;
    public static GetStartPoint instance;



    public static void StartInformation(Action DoneCallback)
    {
        instance.Menu.SetActive(true);
        instance.TextField.text = "In order to start the next Step it is important that you move to a specific point. Please tell the accompanying researcher to show you that point. Click Next once you've moved there";
        instance.DoneCallback = DoneCallback;
    }

    [ContextMenu("Confirm")]
    public void Confirm()
    {


        confirmIA = ConfirmButton.action;
        confirmIA.performed += (InputAction.CallbackContext context) => SetStartingPoint();
        CloseWindow();
        
    }

    [ContextMenu("Set Starting Point")]
    private void SetStartingPoint()
    {
        XROrigin origin = Test.GetComponent<XROrigin>();
        origin.MoveCameraToWorldLocation(Vector3.zero);
        bool test = origin.MatchOriginUpCameraForward(Vector3.up, Vector3.forward);

        CloseWindow();

        DoneCallback = null;
        confirmIA.performed -= (InputAction.CallbackContext context) => SetStartingPoint();
        DoneCallback.SafeInvoke();
    }

    public void CloseWindow()
    {
        Menu.SetActive(false);
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log($"Tried making multiple StartPointInformer \n Deleting most recent {this.gameObject}");
            Destroy(this.gameObject);
        }
        instance = this;
    }

}

