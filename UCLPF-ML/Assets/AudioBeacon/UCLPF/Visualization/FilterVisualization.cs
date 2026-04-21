using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterVisualization : MonoBehaviour, LPSettingsInformer
{
    public static FilterVisualization instance;
    
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
        if(instance!= null)
        {
            Debug.LogError($"Tried making multiple Filter Visualizations!\n Deleting most recent {this.gameObject}");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        LPGlobalSettings.PlanePosition = this.transform.position.y;
        LPGlobalSettings.RegisterSelf(this);
        UpdateSettings();
    }

    private void OnDestroy()
    {
        LPGlobalSettings.UnregisterSelf(this);
        if(instance == this)
            instance = null;
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
            visualizationMaterial.SetVector("_Listener_Position", pointObject.transform.position);
            visualizationMaterial.SetVector("_View_Direction", direction);

            this.transform.rotation.SetLookRotation(direction);
        }
    }
}
