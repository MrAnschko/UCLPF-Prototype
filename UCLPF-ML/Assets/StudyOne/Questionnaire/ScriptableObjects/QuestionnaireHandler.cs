using MixedReality.Toolkit.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] GameObject SliderPlate;
    [SerializeField] Slider slider;
    public Action onQuestionnaireEnd;
    public static QuestionnaireHandler instance;
    public Action buttonResponse;

    public static void StartQuestionnaire(QuestionnaireSO questionnaire)
    {
        instance.questionnaire = questionnaire;
        instance.StartQuestionnaire();
    }

    public static void StartQuestionnaire(QuestionnaireSO questionnaire,Action onEnd)
    {
        instance.onQuestionnaireEnd = onEnd;
        instance.questionnaire = questionnaire;
        instance.StartQuestionnaire();
    }

    public void StartQuestionnaire()
    {
        menu.SetActive(true);
        if (questionnaire.ShuffleOrder)
            questionnaire.Questions.Shuffle();
        enumerator = questionnaire.Questions.GetEnumerator();
        enumerator.MoveNext();
        answers = new AnswerSheet(questionnaire);
        SetupQuestion();
    }

    public void ChangeQuestionValue(SliderEventData data)
    {
        float floatAnswer = data.NewValue;
        AddAnswer(floatAnswer);
        AnswerTextTMP.text = "Answer: " + floatAnswer.ToString();

    }

    void AddAnswer(float floatAnswer)
    {
        Answer currentA = answers?.answers.Last();
        if (currentA != null)
        {
            currentA.answer = floatAnswer.ToString();
        }
    }


    [ContextMenu("NextQuestion")]
    public void NextQuestion()
    {
        // go over each question
        if (enumerator.MoveNext())
        {
            AddAnswer(slider.Value);
            SetupQuestion();
            
        }
        else
        {
            EndQuestionnaire();
        }
        return;
    }


    [ContextMenu("FinishQuestionnaire")]
    public void EndQuestionnaire()
    {
        answers.Save();
        menu.SetActive(false);
        FilterVisualization.instance.gameObject.SetActive(true);
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
        FilterVisualization.instance.gameObject.SetActive(false);

        answers.answers.Add(new(qType));
        QuestionTMP.text = currentQuestion.question;
        if (qType == Question.Type.PointTask)
        {
            SetupPointTask();
        }
        if (qType == Question.Type.Likert)
        {
            SliderPlate.SetActive(true);

            LowerEndTMP.text = "Strongly \n Disagree";
            UpperEndTMP.text = "Strongly \n Agree";
            slider.MinValue = 1;
            slider.SliderStepDivisions = 6;
            slider.Value = 4;
            slider.MaxValue = 7;
            buttonResponse = NextQuestion;
        }
        if (qType == Question.Type.BeaconNumber) 
        {
            SliderPlate.SetActive(true);

            LowerEndTMP.text = "2";
            UpperEndTMP.text = "11";
            slider.MinValue = 2;
            slider.Value = 2;
            slider.SliderStepDivisions = 9;
            slider.MaxValue = 11;
            buttonResponse = NextQuestion;
        }
        if (qType == Question.Type.TLX)
            SetupNTLX();
        if(qType == Question.Type.Choice)
            SetupChoice();
    }

    public void SetupPointTask()
    {
        Question currentQuestion = enumerator.Current;
        QuestionTMP.text = $"Once you are ready press next. Look at where you heard the {currentQuestion.question} Sound, then confirm using the controller trigger button.";
        SliderPlate.SetActive(false);
        buttonResponse = StartPointTask;
    }
    public void StartPointTask()
    {
        Question currentQuestion = enumerator.Current;
        menu.SetActive(false);
        FilterVisualization.instance.gameObject.SetActive(true);
        PointTaskHandler.BeginTask(currentQuestion.question, FinishPointTask);
    }

    public void FinishPointTask(Vector2 vector)
    {
        Answer currentA = answers?.answers.Last();
        if (currentA != null)
        {
            currentA.answer = vector.Serialize().json;
        }

        menu.SetActive(true);
        SliderPlate.SetActive(true);
        NextQuestion();
    }

    public void SetupNTLX()
    {
        Question currentQuestion = enumerator.Current;
        SliderPlate.SetActive(true);

        LowerEndTMP.text = currentQuestion.SliderStartDesc;
        UpperEndTMP.text = currentQuestion.SliderEndDesc;
        slider.MinValue = 0;
        slider.MaxValue = 20;
        slider.SliderStepDivisions = 20;
        slider.Value = 10;
        buttonResponse = NextQuestion;
    }

    public void SetupChoice()
    {
        Question currentQuestion = enumerator.Current;
        SliderPlate.SetActive(true);

        LowerEndTMP.text = currentQuestion.SliderStartDesc;
        UpperEndTMP.text = currentQuestion.SliderEndDesc;
        slider.MinValue = 0;
        slider.SliderStepDivisions = 1;
        slider.Value = 0;
        slider.MaxValue = 1;
        buttonResponse = NextQuestion;
    }

    private void Awake() // TODO: Proper handling 
    {
        if (instance != null)
        {
            Debug.LogError($"More than one Questionnaire Instance.\n Deleting most recent {this}");
            Destroy(this.gameObject);
            return;
        }
        instance = this;

    }

    [ContextMenu("ButtonResponse")]

    public void CallButtonResponse()
    {
        buttonResponse.SafeInvoke();
    }
}
