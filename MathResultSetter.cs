using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MathResultSetter : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image mark;

    public Sprite checkMark;
    public Sprite crossMark;

    public void Set(string result, bool correct)
    {
        text.text = result;
        if (correct)
        {
            mark.sprite = checkMark;
            mark.color = Color.green;
        }
        else
        {
            mark.sprite = crossMark;
            mark.color = Color.red;
        }
    }
}
