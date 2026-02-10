using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using MixedReality.Toolkit.UX;

public class UCLPF_Settings : MonoBehaviour
{
    public AudioMixer mixer;

    [SerializeField]
    Material visualizationMaterial;

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
        mixer.SetFloat("HRTFEnabled", (hrtf ? 1 : 0));
    }
}
