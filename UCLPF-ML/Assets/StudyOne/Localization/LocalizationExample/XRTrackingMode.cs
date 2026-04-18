using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class XRTrackingMode : MonoBehaviour
{
    public InputAction ToggleAction;

    private XRInputSubsystem _inputSubsystem;

    private int _flagIndex = 0;

    private readonly TrackingOriginModeFlags[] _supportedFlags =
        { TrackingOriginModeFlags.Device, TrackingOriginModeFlags.Floor, TrackingOriginModeFlags.Unbounded };

    IEnumerator Start()
    {
        yield return new WaitUntil(() => XRGeneralSettings.Instance != null &&
        XRGeneralSettings.Instance.Manager != null &&
        XRGeneralSettings.Instance.Manager.activeLoader != null &&
        XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>() != null);

        _inputSubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>();
        Debug.Log("Subsystem ready.");

        while (enabled)
        {
            if (ToggleAction.triggered)
            {
                _flagIndex = _flagIndex > 2 ? 0 : _flagIndex++;
                SetSpace(_supportedFlags[_flagIndex]);
            }

            yield return null;
        }
    }

    public void SetSpace(TrackingOriginModeFlags flag)
    {
        if (_inputSubsystem.TrySetTrackingOriginMode(flag))
        {
            Debug.Log("Current Tracking Mode" + _inputSubsystem.GetTrackingOriginMode());
            _inputSubsystem.TryRecenter();
        }
        else
        {
            Debug.LogError("SetSpace failed to set Tracking Mode Origin to " + flag);
        }
    }
}


