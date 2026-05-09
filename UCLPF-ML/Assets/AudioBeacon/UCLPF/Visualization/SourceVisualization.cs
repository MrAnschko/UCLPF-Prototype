using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceVisualization : MonoBehaviour
{
    static List<SourceVisualization> visualizations;

    static public List<SourceVisualization> Visualizations
    {
        get
        {
            if (visualizations == null) visualizations = new List<SourceVisualization>();
            return visualizations;
        }
    }

    public static void RegisterVis(SourceVisualization vis)
    {
        if (visualizations == null) visualizations = new List<SourceVisualization>();
        visualizations.Add(vis);
    }

    public static void DeRegisterVis(SourceVisualization vis)
    {
        if (visualizations == null) return;
        visualizations.Remove(vis);
    }



    [SerializeField] AnimationCurve visibilityCurve; // how visible the material is supposed to be 
    private float state = 0f; // state depending on 0 or 1
    [SerializeField] AnimationCurve angleCurve;
    [SerializeField] Material material;
    [SerializeField] Renderer visibilityRenderer;
    [SerializeField] public BeaconData beaconData;

    public bool IsActive
    {
        get
        {
            return visibilityCurve.Evaluate(state) > 0.3f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        visibilityRenderer = GetComponent<Renderer>();
        visibilityRenderer.material = new Material(material);
        
    }

    private void Update()
    {
        updateVisual();
    }

    // Update is called once per frame
    void updateVisual()
    {
        Vector3 localDir = Gaze.instance.gameObject.transform.InverseTransformPoint(transform.position); // get direction of 
        float angle = Vector3.Angle(localDir,Vector3.forward);
        float diff = angleCurve.Evaluate(angle) * Time.deltaTime;
        state = Mathf.Clamp(state+diff,0.0f,1.0f);
        Vector4 current_col = visibilityRenderer.material.color;
        visibilityRenderer.material.color = new Vector4(current_col.x , current_col.y , current_col.z, visibilityCurve.Evaluate(state));


    }

    private void OnEnable()
    {
        RegisterVis(this);
    }

    private void OnDisable()
    {
        DeRegisterVis(this);
    }
}
