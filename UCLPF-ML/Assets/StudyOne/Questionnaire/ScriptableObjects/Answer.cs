using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Answer
{
    [SerializeField] public Question.Type type;
    [SerializeField] public string answer;

    public Answer(Question.Type type)
    {
        this.type = type;
        answer = "NA";
    }
}
