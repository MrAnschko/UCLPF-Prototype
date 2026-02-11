using MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioListener))]
public class UCLPF_Settings : MonoBehaviour
{
    public AudioMixer mixer;

    [SerializeField]
    Material visualizationMaterial;

    public InputActionReference headGazeDirectionRef;
    public InputActionReference eyeGazeDirectionRef;
    InputAction gazeDirectionIA;
    [SerializeField]
    bool useEyeGaze;

    public bool UseEyeGaze { get => useEyeGaze; set{ useEyeGaze = value; SetGazeMethod(); } }

    public InputAction GazeDirectionIA { get => gazeDirectionIA; set => gazeDirectionIA = value; }

    public enum Mode
    {
        None,
        Vector,
        Circle,
        Point
    }





    public void SetMode(Mode mode)
    {
        switch (mode)
        {
            case Mode.Circle:
                SetModeCircle();
                return;
            case Mode.Point:
                SetModePoint();
                return;
            case Mode.Vector:
                SetModeVector();
                return;
            default:
                return;
        }

    }

    public void SetModeVector()
    {
        mixer.SetFloat("HalfAngle", Mathf.PI / 180);
        mixer.SetFloat("PointSF", 0);
        mixer.SetFloat("CircleSF", 0);

        visualizationMaterial.SetFloat("_Half_Angle", 15);
        visualizationMaterial.SetFloat("_Point_SF", 0);
        mixer.SetFloat("Mix", 1);
        visualizationMaterial.SetFloat("_Circle_SF", 0);
    }

    public void SetModePoint()
    {
        mixer.SetFloat("HalfAngle", Mathf.Deg2Rad * 18000);
        mixer.SetFloat("PointSF", 5);
        mixer.SetFloat("CircleSF", 0);
        mixer.SetFloat("Mix", 1);

        visualizationMaterial.SetFloat("_Half_Angle", 18000);
        visualizationMaterial.SetFloat("_Point_SF", 5);
        visualizationMaterial.SetFloat("_Circle_SF", 0);
    }


    public void SetModeCircle()
    {
        mixer.SetFloat("HalfAngle", Mathf.Deg2Rad * 18000);
        mixer.SetFloat("PointSF", 0);
        mixer.SetFloat("CircleSF", 5);
        mixer.SetFloat("Mix", 1);

        visualizationMaterial.SetFloat("_Half_Angle", 18000);
        visualizationMaterial.SetFloat("_Point_SF", 0);
        visualizationMaterial.SetFloat("_Circle_SF", 5);
    }


    public void ChangeMix(SliderEventData data)
    {
        float mix = data.NewValue;
        mixer.SetFloat("Mix", mix / 100);
    }

    public void ChangeQFactor(SliderEventData data)
    {
        float q_factor = data.NewValue;
        mixer.SetFloat("QFactor",q_factor);
    }

    public void ChangeMaxFreq(SliderEventData data)
    {
        float maxFreq = data.NewValue;
        mixer.SetFloat("MaxFreq", maxFreq);
    }

    public void ChangeHalfAngle(SliderEventData data)
    {
        float halfAngle= data.NewValue;
        mixer.SetFloat("HalfAngle",halfAngle);
        visualizationMaterial.SetFloat("_Half_Angle", halfAngle);
    }

    public void ChangePointFactor(SliderEventData data)
    {
        
        float pointFactor = data.NewValue;
        visualizationMaterial.SetFloat("_Point_SF", pointFactor);
        mixer.SetFloat("PointSF", pointFactor);
    }

    public void ChangeCircleFactor(SliderEventData data) 
    { 
        float circleFactor = data.NewValue;
        visualizationMaterial.SetFloat("_Circle_SF", circleFactor);
        mixer.SetFloat("CircleSF", circleFactor);

    }

    public void SetHRTF(bool hrtf)
    {
        mixer.SetFloat("HRTFEnabled", (hrtf ? 1.0f : 0.0f));
    }


    private void Update()
    {
        UpdateRotation();
    }

    // Method to update the visuals to reflect the views
    private void UpdateRotation()
    {
        Quaternion viewQuaternion = GazeDirectionIA.ReadValue<Quaternion>();
        Vector3 direction =  viewQuaternion* Vector3.forward;
        visualizationMaterial.SetVector("_Listener_Position", Camera.main.transform.position);
        visualizationMaterial.SetVector("_View_Direction", direction);

        Debug.Log(direction);
        this.transform.rotation.SetLookRotation(direction);

    }

    private void Start()
    {
        SetGazeMethod();
    }

    private void SetGazeMethod()
    {
        if (useEyeGaze)
        {
            GazeDirectionIA = eyeGazeDirectionRef.action;
            return;
        }
        GazeDirectionIA = headGazeDirectionRef.action;
        return ;
    }

    public void SetGazeMethod(bool enableEyeGaze)
    {
        this.UseEyeGaze = enableEyeGaze;
        SetGazeMethod();
    }
}
