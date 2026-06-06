using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


[ExecuteInEditMode]
public class DistCalculator : MonoBehaviour
{
    [SerializeField] List<float> dists = new List<float>();

    private void OnEnable()
    {
        int childCount = transform.childCount;
        dists = Enumerable.Repeat(float.PositiveInfinity, childCount).ToList();

        for (int i = 0; i < childCount; i++)
        {
            for(int j = childCount-1; j >i; j--)
            {
                float dist = Vector3.Distance(transform.GetChild(i).transform.position, transform.GetChild(j).transform.position);
                dists[i] = (dist< dists[i])? dist: dists[i];
                dists[j] = (dist< dists[j])? dist: dists[j];
            }
        }

        Debug.Log($"Min: {dists.Min()}");
        Debug.Log($"Max: {dists.Max()}");

        float mean = 0.0f;
        foreach (var dist in dists)
        {
            mean += dist;
        }
        mean /= dists.Count;
        Debug.Log($"Mean: {mean}");

    }
}
