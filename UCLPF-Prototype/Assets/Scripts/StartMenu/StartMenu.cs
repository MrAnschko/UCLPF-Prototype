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
        SetupStartMenu();


    }

    void SetupStartMenu()
    {
        dialogPool.DialogPrefab = DialogPrefab;
        Dialog dialog = (Dialog)dialogPool.Get(DialogPool.Policy.DismissExisting);
        dialog.SetHeader("Use Case");
        dialog.SetBody("Which mode do you wish to start?");
        dialog.SetNeutral("User Test", (DialogButtonEventArgs args) => {});
        dialog.SetNegative("Free Roam (Predefined modes)", (DialogButtonEventArgs args) => { 
            SetupModeMenu();
            dialog.Dismiss();
        });
        dialog.SetPositive("Free Roam (Custom Settings)", (DialogButtonEventArgs args) => LoadSandboxScene());
        dialog.Show();
    }


    void SetupModeMenu()
    {
        dialogPool.DialogPrefab = DialogPrefab;
        Dialog dialog = (Dialog)dialogPool.Get(DialogPool.Policy.DismissExisting);
        dialog.SetHeader("Interaction Method");
        dialog.SetBody("Which interaction mode would you like to test?");
        dialog.SetNegative("Point", (DialogButtonEventArgs args) => UsePointMode(UCLPF_Settings.Mode.Point, args));
        dialog.SetNeutral("View", (DialogButtonEventArgs args) => UsePointMode(UCLPF_Settings.Mode.Vector, args));
        dialog.SetPositive("Circle", (DialogButtonEventArgs args) => UsePointMode(UCLPF_Settings.Mode.Circle, args));
        dialog.Show();
    }
 
    private static void UsePointMode (UCLPF_Settings.Mode mode ,DialogButtonEventArgs args)
    {
        PersistentData.Mode = mode;
        Debug.Log(PersistentData.Mode);
        LoadModeTestScene();
        return;
    }


    private static void LoadSandboxScene()
    {
        SceneManager.LoadSceneAsync("SandboxScene");
    }

    private static void LoadModeTestScene()
    {
        SceneManager.LoadSceneAsync("ModeTestingScene");
    }
}
