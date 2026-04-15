using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EmptyQuestionnaire", menuName = "UCLPF/Questionnaire", order = 1)]

public class QuestionnaireSO : ScriptableObject
{
    public enum PosedPoint
    {
        AfterEnd,
        AfterPath,
        AfterStep
    }

    [SerializeField] public bool ShuffleOrder;

    [SerializeField] public string Name;
    [SerializeField] public PosedPoint WhenAsked;
    [SerializeField] public List<Question> Questions;
    
}
