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

    private void Start()
    {
        Debug.Log("Mode Set to Vector");
        SetModeVector();
    }
    public void SetMode(Mode mode)
    {
        return;
    }

    public void SetModeVector()
    {
        mixer.SetFloat("HalfAngle", Mathf.PI/360);
        mixer.SetFloat("PointSF", 0);
        mixer.SetFloat("CircleSF", 0);

        visualizationMaterial.SetFloat("_Half_Angle",15);
        visualizationMaterial.SetFloat("_Point_SF",0);
        visualizationMaterial.SetFloat("_Circle_SF",0);
    }

    public void SetModePoint()
    {
        mixer.SetFloat("HalfAngle", Mathf.Deg2Rad*18000);
        mixer.SetFloat("PointSF", 5);
        mixer.SetFloat("CircleSF", 0);

        visualizationMaterial.SetFloat("_Half_Angle", 18000);
        visualizationMaterial.SetFloat("_Point_SF", 5);
        visualizationMaterial.SetFloat("_Circle_SF", 0);
    }


    public void SetModeCircle() 
    {
        mixer.SetFloat("HalfAngle", Mathf.Deg2Rad * 18000);
        mixer.SetFloat("PointSF", 0);
        mixer.SetFloat("CircleSF", 5);

        visualizationMaterial.SetFloat("_Half_Angle", 18000);
        visualizationMaterial.SetFloat("_Point_SF", 0);
        visualizationMaterial.SetFloat("_Circle_SF", 5);
    }
}
