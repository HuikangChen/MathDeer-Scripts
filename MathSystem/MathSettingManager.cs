using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains the Math Setting panel functions, allowing player to take out/add in certain math operations
/// </summary>

public class MathSettingManager : MonoBehaviour
{
    private static MathSettingManager instance;
    public static MathSettingManager Instance { get { return instance; } }

    [SerializeField]
    [Tooltip("The button that displays the math settings")]
    private GameObject mathSettingButton;

    /// <summary>
    /// Does math question have addition?
    /// </summary>
    [HideInInspector]
    public bool addition = true;

    /// <summary>
    /// Does math question have subtraction?
    /// </summary>
    [HideInInspector]
    public bool subtraction = true;

    /// <summary>
    /// Does math question have multiplication?
    /// </summary>
    [HideInInspector]
    public bool multiplication = false;

    /// <summary>
    /// Does math question have division?
    /// </summary>
    [HideInInspector]
    public bool division = false;

    /// <summary>
    /// Does math question have two operations?
    /// </summary>
    [HideInInspector]
    public bool twoOp = false;

    /// <summary>
    /// Does math question have parentheses?
    /// </summary>
    [HideInInspector]
    public bool parentheses = false;

    [Header("[Button References]")]
    [SerializeField] private Image additionButton;
    [SerializeField] private Image subtractionButton;
    [SerializeField] private Image multiplicationButton;
    [SerializeField] private Image divisionButton;
    [SerializeField] private Image twoOPButton;
    [SerializeField] private Image parenthesesButton;

    private PanelTransitionManager transition;

    private void Awake()
    {
        //Our singleton pattern
        if (instance != null && instance != this)
        {
            // destroy the gameobject if an instance of this exist already
            Destroy(gameObject);
        }
        else
        {
            //Set our instance to this object/instance
            instance = this;
        }
    }

    private void Start()
    {
        transition = PanelTransitionManager.Instance;

        #region Math settings initialization 

        //Below checks if each math setting has been selected or not, then it sets the button color accordingly
        
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
        #endregion

        GameManager.OnGameStart += Initialize;
    }

    private void Initialize()
    {
        mathSettingButton.SetActive(false);
    }

    #region Math setting button callback functions

    //When player clicks the "Addition" button, we will either take it out or add it in the math question generation
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
    #endregion

    //we want the math setting to have at least 1 operation selected in order to generate math questions
    //we use this to prevent the player from unselecting all the math settings
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
