using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class Recenterer : MonoBehaviour
{

    public InputAction ToggleAction;

    private XRInputSubsystem _inputSubsystem;

    bool _initialized = false;

    static Recenterer instance;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => XRGeneralSettings.Instance != null &&
        XRGeneralSettings.Instance.Manager != null &&
        XRGeneralSettings.Instance.Manager.activeLoader != null &&
        XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>() != null);

        _inputSubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>();
        Debug.Log("Subsystem ready.");
        _initialized = true;
        if(instance!= null)
        {
            Debug.LogError("Tried Making multiple Recenterer.");
            Destroy(this);
            yield break;
        }
        instance = this;
        yield break;
    }

    public static bool TryRecenter()
    {
        return instance._inputSubsystem.TryRecenter();
    }

    public void FixedUpdate()
    {
        TryRecenter();
    }
}
