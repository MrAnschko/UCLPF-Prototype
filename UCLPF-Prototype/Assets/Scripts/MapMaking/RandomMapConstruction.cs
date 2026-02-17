using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class RandomMapConstruction : MonoBehaviour
{
    static float SAMPLEFACTOR = 2;

    [Header("Settings")]
    [SerializeField]
    int steps;
    [SerializeField]
    List<int> possibleIDs = new List<int>();
    [SerializeField]
    List<int> numberOfPointsPerStep = new List<int>();
    [SerializeField]
    float intraStepDist;
    [SerializeField]
    float minPOIDist;

    [SerializeField]
    float interStepDistMin;
    [SerializeField]
    float interStepDistMax;

    [Space(20)]
    [Header("Data")]

    [SerializeField]
    MapData data;
    [SerializeField]
    SonificationHandler sonificationMethod;


    [ContextMenu("Random Map Data")]
    void MakeRandomMapData()
    {
        Vector2 center = Vector2.zero;
        List<StepInfo> all_step = new List<StepInfo>();
        List<int> possibleIDsShuffled = possibleIDs;


        // for each step
        for (int step = 0; step < steps; step++)
        {
            StepInfo stepInfo = new StepInfo();


            //find new center of a sphere. Technically not uniform but for now this will be okay
            float angle = Random.Range(0, 2 * Mathf.PI);
            float dist = Random.Range(interStepDistMin, interStepDistMax);
            center = center + new Vector2(Mathf.Cos(angle) * dist, Mathf.Sin(angle) * dist);

            // generate a number of points
            List<Vector2> pts = new List<Vector2>();
            // Random Point away from previous center, or zero

            for (int pt = 0; pt < numberOfPointsPerStep[step];)
            {
                Vector2 new_pt = Random.insideUnitCircle * intraStepDist + center;
                if (!pts.Any((Vector2 v) => (v - new_pt).magnitude < minPOIDist))
                {
                    pts.Add(new_pt);
                    pt++;
                }
            }

            possibleIDsShuffled.Shuffle();



            //chose one to be the target.
            POI_Info targetInfo = new POI_Info();
            targetInfo.id = possibleIDsShuffled[0];
            targetInfo.pos = new Vector3(pts[0].x,-1, pts[0].y);
            stepInfo.target = targetInfo;

            List<POI_Info> distractors = new List<POI_Info>();
            for (int i = 1; i < pts.Count; i++)
            {
                POI_Info distractor_point = new();
                distractor_point.id = possibleIDsShuffled[i];
                distractor_point.pos = new Vector3(pts[i].x, -1, pts[i].y);

                distractors.Add(distractor_point);

            }
            stepInfo.distractor_poi = distractors;
            data.steps.Add(stepInfo);
        }

    }


    [ContextMenu("Save Map Data")]
    void SaveMapData()
    {
        data.Save();

    }
}
