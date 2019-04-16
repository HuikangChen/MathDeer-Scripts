using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MathSettingManager : MonoBehaviour
{
    public static MathSettingManager singleton;

    public GameObject mathSettingButton;

    public bool addition = true;
    public bool subtraction = true;
    public bool multiplication = false;
    public bool division = false;
    public bool twoOp = false;
    public bool parentheses = false;

    public Image additionButton;
    public Image subtractionButton;
    public Image multiplicationButton;
    public Image divisionButton;
    public Image twoOPButton;
    public Image parenthesesButton;

    PanelTransitionManager transition;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        transition = PanelTransitionManager.singleton;

        if (PlayerPrefs.GetInt("Addition", 1) == 0)
        {
            addition = false;
            Color temp = additionButton.color;
            temp.a = 0;
            additionButton.color = temp;
            PlayerPrefs.SetInt("Addition", 0);
        }

        if (PlayerPrefs.GetInt("Subtraction", 1) == 0)
        {
            subtraction = false;
            Color temp = subtractionButton.color;
            temp.a = 0;
            subtractionButton.color = temp;
            PlayerPrefs.SetInt("Subtraction", 0);
        }

        if (PlayerPrefs.GetInt("Multiplication", 1) == 0)
        {
            multiplication = false;
            Color temp = multiplicationButton.color;
            temp.a = 0;
            multiplicationButton.color = temp;
            PlayerPrefs.SetInt("Multiplication", 0);
        }

        if (PlayerPrefs.GetInt("Division", 1) == 0)
        {
            division = false;
            Color temp = divisionButton.color;
            temp.a = 0;
            divisionButton.color = temp;
            PlayerPrefs.SetInt("Division", 0);
        }

        if (PlayerPrefs.GetInt("TwoOp", 1) == 0)
        {
            twoOp = false;
            Color temp = twoOPButton.color;
            temp.a = 0f;
            twoOPButton.color = temp;
            PlayerPrefs.SetInt("TwoOp", 0);
        }
        if (PlayerPrefs.GetInt("Parentheses", 1) == 0)
        {
            parentheses = false;
            Color temp = parenthesesButton.color;
            temp.a = 0f;
            parenthesesButton.color = temp;
            PlayerPrefs.SetInt("Parentheses", 0);
        }

        GameManager.OnGameStart += Initialize;
    }

    private void Initialize()
    {
        mathSettingButton.SetActive(false);
    }

    bool AtLeastOneOperationSelected()
    {
        int temp = 0;

        if (addition)
            temp++;
        if (subtraction)
            temp++;
        if (multiplication)
            temp++;
        if (division)
            temp++;

        if (temp >= 3)
        {
            return true;
        }
        return false;
    }

    public void SetAddition()
    {
        if (addition == false)
        {
            addition = true;
            Color temp = additionButton.color;
            temp.a = 0.25f;
            additionButton.color = temp;
            PlayerPrefs.SetInt("Addition", 1);
        }
        else
        {
            if (AtLeastOneOperationSelected())
            {
                addition = false;
                Color temp = additionButton.color;
                temp.a = 0;
                additionButton.color = temp;
                PlayerPrefs.SetInt("Addition", 0);
            }
        }
    }

    public void SetSubtraction()
    {
        if (subtraction == false)
        {
            subtraction = true;
            Color temp = subtractionButton.color;
            temp.a = 0.25f;
            subtractionButton.color = temp;
            PlayerPrefs.SetInt("Subtraction", 1);
        }
        else
        {
            if (AtLeastOneOperationSelected())
            {
                subtraction = false;
                Color temp = subtractionButton.color;
                temp.a = 0;
                subtractionButton.color = temp;
                PlayerPrefs.SetInt("Subtraction", 0);
            }
        }
    }

    public void SetMultiplication()
    {
        if (multiplication == false)
        {
            multiplication = true;
            Color temp = multiplicationButton.color;
            temp.a = 0.25f;
            multiplicationButton.color = temp;
            PlayerPrefs.SetInt("Multiplication", 1);
        }
        else
        {
            if (AtLeastOneOperationSelected())
            {
                multiplication = false;
                Color temp = multiplicationButton.color;
                temp.a = 0;
                multiplicationButton.color = temp;
                PlayerPrefs.SetInt("Multiplication", 0);
            }
        }
    }

    public void SetDivision()
    {
        if (division == false)
        {
            division = true;
            Color temp = divisionButton.color;
            temp.a = 0.25f;
            divisionButton.color = temp;
            PlayerPrefs.SetInt("Division", 1);
        }
        else
        {
            if (AtLeastOneOperationSelected())
            {
                division = false;
                Color temp = divisionButton.color;
                temp.a = 0;
                divisionButton.color = temp;
                PlayerPrefs.SetInt("Division", 0);
            }
        }
    }

    public void SetTwoOP()
    {
        twoOp = !twoOp;

        if (twoOp)
        {
            Color temp = twoOPButton.color;
            temp.a = 0.25f;
            twoOPButton.color = temp;
            PlayerPrefs.SetInt("TwoOp", 1);
        }
        else
        {
            Color temp = twoOPButton.color;
            temp.a = 0f;
            twoOPButton.color = temp;
            PlayerPrefs.SetInt("TwoOp", 0);
        }
    }

    public void SetParentheses()
    {
        parentheses = !parentheses;

        if (parentheses)
        {
            Color temp = parenthesesButton.color;
            temp.a = 0.25f;
            parenthesesButton.color = temp;
            PlayerPrefs.SetInt("Parentheses", 1);
        }
        else
        {
            Color temp = parenthesesButton.color;
            temp.a = 0f;
            parenthesesButton.color = temp;
            PlayerPrefs.SetInt("Parentheses", 0);
        }
    }

    public void OpenPanel()
    {
        transition.Slide(transition.mathSettingPanel, transition.start2, transition.end2, true);
    }

    public void ClosePanel()
    {
        transition.Slide(transition.mathSettingPanel, transition.end2, transition.start2, false);
    }

    private void OnDestroy()
    {
        GameManager.OnGameStart -= Initialize;
    }
}
