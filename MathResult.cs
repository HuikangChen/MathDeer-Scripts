using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class MathResult 
{
    public string result;
    public bool correct;

    public MathResult(string question, int correctAmswer, int chosenAnswer)
    {
        if (chosenAnswer == correctAmswer)
        {
            correct = true;
        }
        else
        {
            correct = false;
        }

        question = question.Replace("?", chosenAnswer.ToString());
        result = question;
    }

}
