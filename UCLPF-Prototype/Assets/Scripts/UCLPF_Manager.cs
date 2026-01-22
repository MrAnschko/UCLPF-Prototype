using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class UCLPF_Manager : MonoBehaviour
{
    public AudioMixer mixer;

    [SerializeField]
    Material visualizationMaterial;

    public enum Mode
    {
        None,
        Vector,
        Circle,
        Point
    }

    public void SetMode(Mode mode)
    {
        return;
    }

    public void SetModeVector()
    {
        mixer.SetFloat("HalfAngle", Mathf.PI/4);
        mixer.SetFloat("PointSF", 0);
        mixer.SetFloat("CircleSF", 0);

        visualizationMaterial.SetFloat("_Half_Angle",45);
        visualizationMaterial.SetFloat("_Point_SF",0);
        visualizationMaterial.SetFloat("_Circle_SF",0);
    }

    public void SetModePoint()
    {
        mixer.SetFloat("HalfAngle", Mathf.Deg2Rad*1800);
        mixer.SetFloat("PointSF", 1);
        mixer.SetFloat("CircleSF", 0);

        visualizationMaterial.SetFloat("_Half_Angle", 1800);
        visualizationMaterial.SetFloat("_Point_SF", 1);
        visualizationMaterial.SetFloat("_Circle_SF", 0);
    }


    public void SetModeCircle() 
    {
        mixer.SetFloat("HalfAngle", Mathf.Deg2Rad * 1800);
        mixer.SetFloat("PointSF", 0);
        mixer.SetFloat("CircleSF", 1);

        visualizationMaterial.SetFloat("_Half_Angle", 1800);
        visualizationMaterial.SetFloat("_Point_SF", 0);
        visualizationMaterial.SetFloat("_Circle_SF", 1);
    }
}
