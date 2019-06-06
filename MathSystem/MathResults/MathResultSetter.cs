using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Used by MathResultManager to set the math results onto the UI
/// </summary>

public class MathResultSetter : MonoBehaviour
{
    //Display of the answer the player picked up
    [SerializeField]
    private TextMeshProUGUI text;

    //Check or cross mark to show if the result is correct or wrong
    [SerializeField]
    private Image mark;

    [SerializeField]
    private Sprite checkMark;

    [SerializeField]
    private Sprite crossMark;

    //Called by MathResultManager to set each math results
    public void Set(string result, bool correct)
    {
        //Sets the result display/text
        text.text = result;

        //if its correct it will give it a checkmark
        if (correct)
        {
            mark.sprite = checkMark;
            mark.color = Color.green;
        }
        else //if wrong it will give it a crossmark
        {
            mark.sprite = crossMark;
            mark.color = Color.red;
        }
    }
}
