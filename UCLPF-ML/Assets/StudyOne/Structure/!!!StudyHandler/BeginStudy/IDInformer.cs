using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IDInformer : MonoBehaviour
{
    [SerializeField] TMP_Text TextField;
    [SerializeField] GameObject Menu;

    Action DoneCallback;
    public static IDInformer instance;


    public static void StartInformation(Action DoneCallback)
    {
        instance.Menu.SetActive(true);
        instance.TextField.text = "Thank you for your participation in this study\n" +
            $"Your Personal ID is  <u><b><i>{ProcessInfos.UserID}</u></b></i>.\n" +
            "Your Data is tracked through this ID alone and we will not save any other data that directly associates you with it." +
            "However you may always request we delete your data by providing us your ID. Please take a moment to note your ID then press next.";
        instance.DoneCallback = DoneCallback;
    }

    [ContextMenu("Confirm")]
    public void Confirm()
    {
        DoneCallback.SafeInvoke();
        CloseWindow();
    }

    public void CloseWindow()
    {
        Menu.SetActive(false);
    }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log($"Tried making multiple IDINFORMER \n Deleting most recent {this.gameObject}");
            Destroy(this.gameObject);
        }
        instance = this;
    }
}
