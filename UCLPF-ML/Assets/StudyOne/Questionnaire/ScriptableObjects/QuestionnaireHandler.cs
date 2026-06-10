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
        AddAnswer(slider.Value);
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


    [ContextMenu("FinishQuestionnaire")]
    public void EndQuestionnaire()
    {
        answers.Save();
        menu.SetActive(false);
        if(FilterVisualization.instance !=null)
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
        if(FilterVisualization.instance != null)
            FilterVisualization.instance.gameObject.SetActive(false);

        answers.answers.Add(new(qType));
        QuestionTMP.text = currentQuestion.question;
        QuestionTMP.ForceMeshUpdate();
        
        if (qType == Question.Type.PointTask)
        {
            SetupPointTask();
        }
        if (qType == Question.Type.Likert)
        {
            SliderPlate.SetActive(true);

            LowerEndTMP.text = "Strongly \n Disagree";
            UpperEndTMP.text = "Strongly \n Agree";

            slider.MaxValue = 7f;
            slider.MinValue = 1f;
            slider.Value = 4f;
            
            slider.SliderStepDivisions = 6;

            buttonResponse = NextQuestion;
        }
        if (qType == Question.Type.BeaconNumber) 
        {
            SliderPlate.SetActive(true);

            LowerEndTMP.text = "2";
            UpperEndTMP.text = "11";

            slider.MaxValue = 11f;
            slider.MinValue = 2f;
            slider.Value = 2f;
            slider.SliderStepDivisions = 9;
            buttonResponse = NextQuestion;
        }
        if (qType == Question.Type.TLX)
            SetupNTLX();
        if(qType == Question.Type.Choice)
            SetupChoice();
        if(qType == Question.Type.Break)
            SetupBreak();
        
        StartCoroutine(ReloadWindow());
    }

    public void SetupPointTask()
    {
        menu.SetActive(false);
        Question currentQuestion = enumerator.Current;
        QuestionTMP.text = $"Once you have read the instructions press next. When this panel closes look at where you heard the Sound with the {currentQuestion.question}, then confirm using the controller trigger button.";
        
        AnswerTextTMP.text = "Next";
        SliderPlate.SetActive(false);
        menu.SetActive(true);
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

    public void SetupBreak()
    {
        Question currentQuestion = enumerator.Current;
        SliderPlate.SetActive(false);
        AnswerTextTMP.text = "Next";
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

    IEnumerator ReloadWindow()
    {
        menu.SetActive(false);
        yield return null;
        menu.SetActive(true);
        yield return null;
        menu.SetActive(false);
        yield return null;
        menu.SetActive(true);
        yield return null;
        menu.SetActive(false);
        yield return null;
        menu.SetActive(true);
        yield return null;
        Debug.Log("Updated");
    }
}
