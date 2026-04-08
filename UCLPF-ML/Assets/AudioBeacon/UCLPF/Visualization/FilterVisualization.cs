using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterVisualization : MonoBehaviour, LPSettingsInformer
{
    [SerializeField] Material visualizationMaterial;
    [SerializeField] GameObject pointObject;

    public void UpdateSettings()
    {
        pointObject = LPGlobalSettings.PointObject;
        visualizationMaterial.SetFloat("_Half_Angle", LPGlobalSettings.Half_angle * 360f / Mathf.PI);

        visualizationMaterial.SetFloat("_Point_SF", LPGlobalSettings.PointDistFactor);
        visualizationMaterial.SetFloat("_Circle_SF", LPGlobalSettings.CircleDistFactor);
        visualizationMaterial.SetFloat("_Height", LPGlobalSettings.PlanePosition);
    }

    private void Awake()
    {
        LPGlobalSettings.RegisterSelf(this);
    }

    private void OnDestroy()
    {
        LPGlobalSettings.UnregisterSelf(this);
    }

    private void Update()
    {
        UpdateVisualization();
    }

    private void UpdateVisualization()
    {

        if (pointObject != null)
        {

            Vector3 direction = pointObject.transform.forward;
            visualizationMaterial.SetVector("_Listener_Position", Camera.main.transform.position);
            visualizationMaterial.SetVector("_View_Direction", direction);

            this.transform.rotation.SetLookRotation(direction);
        }
    }
}
