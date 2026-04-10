using MixedReality.Toolkit.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class QuestionnaireHandler : MonoBehaviour
{
    [SerializeField]QuestionnaireSO questionnaire;
    IEnumerator<Question> enumerator;
    AnswerSheet answers;
    [SerializeField] GameObject menu;
    [SerializeField] TMP_Text QuestionTMP;
    [SerializeField] TMP_Text LowerEndTMP;
    [SerializeField] TMP_Text UpperEndTMP;
    [SerializeField] TMP_Text AnswerTextTMP;
    [SerializeField] Slider slider;
    public Action onQuestionnaireEnd;
    public static QuestionnaireHandler instance;


    public static void StartQuestionnaire(QuestionnaireSO questionnaire)
    {
        instance.questionnaire = questionnaire;
        instance.StartQuestionnaire();
    }

    public static void StartQuestionnaire(QuestionnaireSO questionnaire,Action onEnd)
    {
        instance.questionnaire = questionnaire;
        instance.StartQuestionnaire();
    }

    public void StartQuestionnaire()
    {
        menu.SetActive(true);
        enumerator = questionnaire.Questions.GetEnumerator();
        enumerator.MoveNext();
        answers = new AnswerSheet(questionnaire);
        SetupQuestion();
    }

    public void ChangeQuestionValue(SliderEventData data)
    {
        float floatAnswer = data.NewValue;
        Answer currentA = answers?.answers.Last();
        if (currentA != null)
        {
            currentA.answer = floatAnswer.ToString();
        }
        AnswerTextTMP.text = "Answer: " + floatAnswer.ToString();

}


    [ContextMenu("NextQuestion")]
    public void NextQuestion()
    {
        // go over each question
        if (enumerator.MoveNext())
        {

            SetupQuestion();
            
        }
        else
        {
            EndQuestionnaire();
        }
        return;
    }



    public void EndQuestionnaire()
    {
        answers.Save();
        menu.SetActive(false);
        onQuestionnaireEnd.SafeInvoke();
    }

    void SetupQuestion()
    {
        Question currentQuestion = enumerator.Current;
        if (currentQuestion == null)
        {
            Debug.LogError("No Questions");
        }
        Question.Type qType = currentQuestion.type;
        answers.answers.Add(new(qType));
        QuestionTMP.text = currentQuestion.question;
        if(qType == Question.Type.Likert)
        {
            LowerEndTMP.text = "Strongly \n Disagree";
            UpperEndTMP.text = "Strongly \n Agree";
            slider.MinValue = 1;
            slider.SliderStepDivisions = 6;
            slider.Value = 4;
            slider.MaxValue = 7;

        }
        if (qType == Question.Type.BeaconNumber) 
        {

            LowerEndTMP.text = "2";
            UpperEndTMP.text = "11";
            slider.MinValue = 2;
            slider.Value = 2;
            slider.SliderStepDivisions = 9;
            slider.MaxValue = 11;
        }
    }

    private void Awake() // TODO: Proper handling 
    {
        if (instance != null)
        {
            Debug.LogError($"More than one Questionnaire Instance.\n Deleting most recent {this}");
            Destroy(this.gameObject);
            return;
        }
        Debug.Log("Setup");
        instance = this;

    }
}
