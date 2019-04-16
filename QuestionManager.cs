using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour {

    public static QuestionManager singleton;
    PanelTransitionManager transition;

    #region UI STUFF
    [Header("UI Stuff")]
    public GameObject questionGO;
    public Image questionBorder;
    public TextMeshProUGUI questionText;
    public GameObject pickupImage;
    public Sprite starSprite;
    public Sprite magnetSprite;
    public Sprite shieldSprite;
    public float pickupImageMoveSpeed;
    public Transform pickupEndPos;
    Vector2 pickupBeginPos;
    Camera cam;
    #endregion

    #region QUESTION SETTINGS
    [Space(10)] [Header("Question Settings")]
    [HideInInspector] public int platformBufferAmount;
    [HideInInspector] public bool questionActivated;
    public List<int> tiers = new List<int>();
    #endregion

    #region QUESTION STUFF (THE GOOD STUFF)
    [HideInInspector] public enum operators {none, addition, subtraction, multiplication, division };
    [HideInInspector] public operators op1, op2;
    [HideInInspector] public int x, y, z; //operands
    [HideInInspector] public int answer; //the correct answer
    [HideInInspector] public List<int> answerBank = new List<int>(); //the first 3 indices contains the correct answer with 2 random wong answer generated based on the correct answer
    [HideInInspector] public string question;
    bool parentheses;
    int parenthesesPos;
    #endregion

    void Awake()
    {
        singleton = this;
        questionGO.SetActive(false);
        //ActivateQuestion();
    }

    void Start()
    {
        cam = Camera.main;
        transition = PanelTransitionManager.singleton;
    }

    public void ActivateQuestion(Vector2 startPos)
    {

        pickupBeginPos = cam.WorldToScreenPoint(startPos);
        StartCoroutine("ActivateQuestionCo");
        StartCoroutine("GenerateQuestionAndAnswer");
    }

    IEnumerator ActivateQuestionCo()
    {
        switch (UpgradeManager.singleton.upgradePickedUp)
        {
            case UpgradeManager.Upgrade.None:
                pickupImage.GetComponent<Image>().sprite = starSprite;
                break;

            case UpgradeManager.Upgrade.Magnet_Duration_Up:
                pickupImage.GetComponent<Image>().sprite = magnetSprite;
                break;

            case UpgradeManager.Upgrade.Shield_Duration_Up:
                pickupImage.GetComponent<Image>().sprite = shieldSprite;
                break;
        }

        pickupImage.transform.position = pickupBeginPos;
        pickupImage.SetActive(true);

        PanelTransitionManager.singleton.Slide(transition.questionPanel, transition.start1, transition.end1, true);
        Color temp = questionBorder.color;
        temp.a = 0;
        questionBorder.color = temp;

        while (Vector2.Distance(pickupImage.transform.position, pickupEndPos.position) > 10f)
        {
            pickupImage.transform.position = Vector2.Lerp(pickupImage.transform.position, pickupEndPos.position, Time.deltaTime * pickupImageMoveSpeed);
            yield return null;
        }

        
        platformBufferAmount = (Mathf.Clamp(GameManager.singleton.difficulty, 1, 20) / 10) + 1;
        questionActivated = true;
    }

    public void DeactivateQuestion()
    {
        PanelTransitionManager.singleton.Slide(transition.questionPanel, transition.end1, transition.start1, false);
        questionActivated = false;
        UpgradeSpawner.upgradeSpawned = false;
    }

    public bool AnswerCheck(int answer)
    {
        AddResultToList(this.answer, answer);
        StartCoroutine(AnswerCheckCo(answer.ToString()));
        if (answer == this.answer)
        {
            return true;
        }
        return false;
    }

    void AddResultToList(int correctAnswer, int chosenAnswer)
    {
        MathResultManager.singleton.Add(new MathResult(question, correctAnswer, chosenAnswer));
    }

    IEnumerator AnswerCheckCo(string answer)
    {
        question = question.Remove(question.Length - 1);
        //question = question + answer;
        question = "WRONG"; // change this later
        questionText.text = question;

        if (answer == this.answer.ToString())
        {

            pickupImage.transform.position = pickupEndPos.position;
            pickupImage.SetActive(true);
            DeactivateQuestion();
            while (Vector2.Distance(pickupImage.transform.position, cam.WorldToScreenPoint(PlayerMovement.singleton.transform.position)) > 40f)
            {
                pickupImage.transform.position = Vector2.MoveTowards(pickupImage.transform.position, cam.WorldToScreenPoint(PlayerMovement.singleton.transform.position), Time.deltaTime * pickupImageMoveSpeed * 100);
                yield return null;
            }
            pickupImage.SetActive(false);
        }
        else
        {
            questionBorder.color = Color.red;

            Vector3 origin = questionGO.transform.position;
            Vector3 shakeVector = new Vector3();
            float duration = .5f;         
            while (duration > 0)
            {
                shakeVector = new Vector3(Random.value * 8f, Random.value * 8f);
                questionGO.transform.position = origin + shakeVector;
                duration -= Time.deltaTime;
                yield return null;
            }
            questionGO.transform.position = origin;
            yield return new WaitForSeconds(1);
            DeactivateQuestion();
            pickupImage.SetActive(false);
        }
        
    }

    IEnumerator GenerateQuestionAndAnswer()
    {
        yield return StartCoroutine("GenerateQuestion");
        yield return StartCoroutine("GenerateAnswer");
    }

    #region QUESTION GENERATION STUFF (fck this thing)
    IEnumerator GenerateQuestion()
    {
        yield return StartCoroutine("GenerateOperations");
        yield return StartCoroutine("GenerateOperands");
        yield return StartCoroutine("SetQuestionUI");
    }

    IEnumerator GenerateOperations()
    {
        op1 = operators.none;
        op2 = operators.none;
        //Generate Parantheses///
        parentheses = false;

        if (MathSettingManager.singleton.parentheses)
        {
            parentheses = true;
            parenthesesPos = Random.Range(0, 2);
        }
        ////////////////////////

        //Generates random op indexes, and also use temp array to make sure multiple op dont repeat///
        List<int> temp = new List<int>();

        if (MathSettingManager.singleton.addition)
        {
            temp.Add(0);
        }

        if (MathSettingManager.singleton.subtraction)
        {
            temp.Add(1);
        }

        if (MathSettingManager.singleton.multiplication)
        {
            temp.Add(2);
        }

        if (MathSettingManager.singleton.division)
        {
            temp.Add(3);
        }

        for (int i = 0; i < temp.Count; i++)
        {
            int n = temp[i];
            int rand = Random.Range(0, temp.Count);
            temp[i] = temp[rand];
            temp[rand] = n;
        }
        ////////////////////////

        //Generate the 2 op////
        if (MathSettingManager.singleton.twoOp)
        {
            op2 = SetOp(temp[0]);
        }

        op1 = SetOp(temp[1]);
        ////////////////////////
        yield return null;
    }

    IEnumerator GenerateOperands()
    {
        x = Random.Range(1, 10);
        y = Random.Range(1, 10);
        z = Random.Range(1, 10);

        if (parentheses && ((op1 == operators.division && parenthesesPos == 1) || (op2 == operators.division && parenthesesPos == 0)))
        {
            if (op1 == operators.division && parenthesesPos == 1)
            {
                if (DoSomeMath(op2, y, z) == 0)
                {
                    if (y == 9)
                    { y = 1; }
                    else
                    { y = y + 1; }
                }
                x = x * DoSomeMath(op2, y, z);
            }

            if (op2 == operators.division && parenthesesPos == 0)
            {
                z = GetRandomDivisor(DoSomeMath(op1, x, y));
            }
        }
        else
        {
            if (op1 == operators.multiplication && op2 == operators.division)
            {
                z = GetRandomDivisor(DoSomeMath(op1, x, y));
            }
            else
            {
                if (op1 == operators.division)
                {
                    x = x * y;
                }

                if (op2 == operators.division)
                {
                    y = y * z;
                }
            }
        }

        yield return null;
    }

    IEnumerator SetQuestionUI()
    {
        question = "";

        if (parentheses)
        {
            if (parenthesesPos == 0)
            {
                question = "(" + x + OpToString(op1) + y + ")" + OpToString(op2) + z + "=?";
            }
            else
            {
                question = x + OpToString(op1) + "(" + y + OpToString(op2) + z + ")" + "=?";
            }
        }
        else
        {
            if (op2 == operators.none)
            {
                question = x + OpToString(op1) + y + "=?";
            }
            else
            {
                question = x + OpToString(op1) + y + OpToString(op2) + z + "=?";
            }
        }

        questionText.text = question;

        yield return null;
    }

    operators SetOp(int index)
    {
        if (index == 0)
        {
            return operators.addition;
        }
        else if (index == 1)
        {
            return operators.subtraction;
        }
        else if (index == 2)
        {
            return operators.multiplication;
        }

        return operators.division;
    }

    int GetRandomDivisor(int num)
    {
        List<int> temp = new List<int>();

        for (int i = Mathf.Abs(num); i > 0; i--)
        {
            if (num % i == 0)
            {
                temp.Add(i);
            }
        }

        return temp[Random.Range(0, temp.Count)];
    }

    string OpToString(operators op)
    {
        string result = "";

        switch (op)
        {
            case operators.addition:
                result = "+";
                break;
            case operators.subtraction:
                result = "-";
                break;
            case operators.multiplication:
                result = "*";
                break;
            case operators.division:
                result = "/";
                break;
        }
        return result;
    }
    #endregion

    #region ANSWER GENERATION STUFF (fck this thing also)
    IEnumerator GenerateAnswer()
    {
        yield return StartCoroutine("GenerateCorrectAnswer");
        yield return StartCoroutine("GenerateAnswerBank");
    }

    IEnumerator GenerateCorrectAnswer()
    {
        if (parentheses)
        {
            if (parenthesesPos == 0)
            {
                answer = DoSomeMath(op2, DoSomeMath(op1, x, y), z);
            }
            else
            {
                answer = DoSomeMath(op1, x, DoSomeMath(op2, y, z));
            }
        }
        else
        {
            if (op2 == operators.none)
            {
                answer = DoSomeMath(op1, x, y);
            }
            else
            {
                if ((op1 == operators.addition || op1 == operators.subtraction) && (op2 == operators.multiplication || op2 == operators.division))
                {
                    answer = DoSomeMath(op1, x, DoSomeMath(op2, y, z));
                }
                else
                {
                    answer = DoSomeMath(op2, DoSomeMath(op1, x, y), z);
                }
            }
        }
        yield return null;
    }

    IEnumerator GenerateAnswerBank()
    {
        answerBank.Clear();

        answerBank.Add(answer);

        for (int i = -5; i < 0; i++)
        {
            answerBank.Add(answerBank[0] + i);
        }

        for (int i = 1; i < 5; i++)
        {
            answerBank.Add(answerBank[0] + i);
        }

        yield return StartCoroutine(ShuffleAnswerBank(1, answerBank.Count));
        yield return StartCoroutine(ShuffleAnswerBank(0, 3));

    }

    IEnumerator ShuffleAnswerBank(int startIndex, int count)
    {
        for (int i = startIndex; i < count; i++)
        {
            int temp = answerBank[i];
            int rand = Random.Range(startIndex, count);
            answerBank[i] = answerBank[rand];
            answerBank[rand] = temp;
            yield return null;
        }
    }
    #endregion

    int DoSomeMath(operators op, int a, int b)
    {
        int result = 0;

        switch (op)
        {
            case operators.addition:
                result = a + b;
                break;
            case operators.subtraction:
                result = a - b;
                break;
            case operators.multiplication:
                result = a * b;
                break;
            case operators.division:
                result = a / b;
                break;
        }
        return result;
    }
}
