using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Contains a single math result
/// Used by MathResultManager to generate a list of MathResults
/// </summary>

[System.Serializable]
public class MathResult 
{
    /// <summary>
    /// The math question along with the answer the player picked up
    /// Ex: 1+2=3
    /// </summary>
    [HideInInspector]
    public string result;

    /// <summary>
    /// 
    /// </summary>
    [HideInInspector]
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

        //replace the "?" with the chosen answer
        question = question.Replace("?", chosenAnswer.ToString());
        
        result = question;
    }

}
