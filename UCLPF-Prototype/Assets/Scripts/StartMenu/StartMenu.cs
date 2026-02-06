using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MixedReality.Toolkit.UX;
using System;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    DialogPool dialogPool = new DialogPool();
    public GameObject DialogPrefab;
    // Start is called before the first frame update
    void Start()
    {
        dialogPool.DialogPrefab = DialogPrefab;
        Dialog dialog = (Dialog) dialogPool.Get(DialogPool.Policy.DismissExisting);
        dialog.SetHeader("Interaction Method");
        dialog.SetBody("Which interaction mode would you like to test?");
        dialog.SetNegative("Point", (DialogButtonEventArgs args ) => UsePointMode(UCLPF_Manager.Mode.Point, args));
        dialog.SetNeutral("View", (DialogButtonEventArgs args ) => UsePointMode(UCLPF_Manager.Mode.Vector, args));
        dialog.SetPositive("Circle", (DialogButtonEventArgs args ) => UsePointMode(UCLPF_Manager.Mode.Circle, args));
        dialog.Show();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static void UsePointMode (UCLPF_Manager.Mode mode ,DialogButtonEventArgs args)
    {
        PersistentData.Mode = mode;
        Debug.Log(PersistentData.Mode);
        LoadTestScene();
        return;
    }

    private static void LoadTestScene()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
