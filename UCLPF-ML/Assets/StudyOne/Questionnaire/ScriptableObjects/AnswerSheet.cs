using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class AnswerSheet : DataContainer
{
    [SerializeField] public string Name;
    [SerializeField] public string userID;
    [SerializeField] public QuestionnaireSO.PosedPoint WhenAsked;
    [SerializeField] public List<Question> questions;
    [SerializeField] public List<Answer> answers;

    public void Save() 
    {
        userID = ProcessInfos.UserID;
        string filepath = ProcessInfos.UserID + "_Questionnaire";
        if ((int)WhenAsked > 0)
            filepath += "_Path" + ProcessInfos.currentPath;
        if((int)WhenAsked>1)
                filepath+= "_Step" + ProcessInfos.CurrentStep;
        filepath += "_"+Name;

        Save(filepath);

    }

    public AnswerSheet(QuestionnaireSO questionnaire)
    {
        userID= ProcessInfos.UserID;
        Name = questionnaire.Name;
        WhenAsked = questionnaire.WhenAsked;
        questions = questionnaire.Questions;
        answers = new List<Answer>();
    }

}
