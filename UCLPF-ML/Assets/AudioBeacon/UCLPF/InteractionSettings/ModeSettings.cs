using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default", menuName = "UCLPF/InteractionMode", order = 1)]
public class ModeSettings : ScriptableObject
{
    [SerializeField]      public ProcessInfos.InteractionMethod interactionMethod;
    [Range(0.0001f,20f)]  public float qFactor = 0.707f;
    [Range(20,22050)]     public float cutoffInitialFreq = 22050;
    [Range(20,22050)]     public float cutoffMinimalFreq = 20;
    [Range(0.001f,314f)]  public float half_angle = 314f;
    [Range(0,10)]         public float pointDistFactor= 0;
    [Range(0, 10)]        public float circleDistFactor=0;
    [Range(0, 2)]         public float horizonBehavior = 0;
    public float planePosition = -1;

    public void ApplySettings()
    {
        LPGlobalSettings.QFactor = qFactor;
        LPGlobalSettings.CutoffInitialFreq = cutoffInitialFreq;
        LPGlobalSettings.CutoffMinimalFreq = cutoffMinimalFreq;
        LPGlobalSettings.Half_angle = half_angle;
        LPGlobalSettings.PointDistFactor = pointDistFactor;
        LPGlobalSettings.CircleDistFactor = circleDistFactor;
        LPGlobalSettings.PlanePosition = planePosition;
    }
}
