using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// make this is singleton for now
public class GoalVisualization : MonoBehaviour
{
    public static GoalVisualization Instance;

    public TMP_Text text_comp;
    public void Setup()
    {
        Instance = this;
    }


    public static void SetGoalText(string text)
    {
        if (Instance == null) {
            return;
        }
        Instance.text_comp.text = "Goal:\n" + text;
    }
}
