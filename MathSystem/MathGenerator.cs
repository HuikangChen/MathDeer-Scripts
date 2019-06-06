using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles generation for math question and answer bank
/// </summary>

public class MathGenerator : MonoBehaviour
{
    public enum operators { none, addition, subtraction, multiplication, division };

    /// <summary>
    /// Our 2 operators for the math question, will be generated based on settings
    /// op2 can be none if question only has 1 operator
    /// </summary>
    [HideInInspector]
    public operators op1, op2;

    /// <summary>
    /// Our operands for the math question, will be generated based on settings
    /// operand z will not be generated if we only have 1 operator
    /// </summary>
    [HideInInspector]
    public int x, y, z; 

    /// <summary>
    /// The correct answer based on our generated question
    /// </summary>
    [HideInInspector] 
    public int answer;

    /// <summary>
    /// An answer bank generated based on the question
    /// The first 3 indexes are the answers we will be displaying in the game, it contains 2 wrong and 1 correct answer
    /// </summary>
    [HideInInspector]
    public List<int> answerBank = new List<int>(); 

    /// <summary>
    /// The generated question as a string, Ex: 1+2= , 2*2+3=
    /// </summary>
    [HideInInspector]
    public string question;

    //If our question has parentheses
    private bool parentheses;

    //The position of our parentheses, can be 1 or 2
    //1 = parentheses around operand x and y
    //2 = parentheses around operand y and z
    private int parenthesesPos;

    //Called by QuestionManager 
    /// <summary>
    /// Generates a math question based on difficulty and math settings, 
    /// then generates an answer bank with 2 wrong and a correct answer
    /// We use coroutines to make sure certain steps finishes before others, since later steps requires previous steps to finish
    /// </summary>
    /// <returns></returns>
    public IEnumerator GenerateQuestionAndAnswer()
    {
        yield return StartCoroutine("GenerateQuestion");
        yield return StartCoroutine("GenerateAnswer");
    }

    #region QUESTION GENERATION COROUTINES

    //The coroutine that generates the question and displays it in 3 steps
    IEnumerator GenerateQuestion()
    {
        //First we generate the operations, it's based on the difficulty and settings
        yield return StartCoroutine("GenerateOperations");

        //Second we generate the Operands based on the Operators, difficulty and settings
        yield return StartCoroutine("GenerateOperands");

        //Third we set the display/ui based on the question
        yield return StartCoroutine("SetQuestionUI");
    }

    //Generates either 1 or 2 operations based on difficulty/settings, and our parentheses
    IEnumerator GenerateOperations()
    {
        //Reset our operations to none
        op1 = operators.none;
        op2 = operators.none;

        //Reset our parentheses
        parentheses = false;

        //if our settings allow parentheses
        if (MathSettingManager.Instance.parentheses)
        {
            parentheses = true;
            
            //Generate a random position for our parentheses
            parenthesesPos = Random.Range(0, 2);
        }

        //List of operations based on settings, we will use this to generate our operations
        List<int> opList = new List<int>();

        //Checking our settings to see what operations we want to include
        //0 = addition
        //1 = subtraction
        //2 = multiplcation
        //3 = division
        if (MathSettingManager.Instance.addition)
        {
            //add the + operation to the list to include it
            opList.Add(0);
        }

        if (MathSettingManager.Instance.subtraction)
        {
            //add the - operation to the list to include it
            opList.Add(1);
        }

        if (MathSettingManager.Instance.multiplication)
        {
            //add the * operation to the list to include it
            opList.Add(2);
        }

        if (MathSettingManager.Instance.division)
        {
            //add the / operation to the list to include it
            opList.Add(3);
        }

        //shuffle our list of operations to randomly choose operations
        for (int i = 0; i < opList.Count; i++)
        {
            int n = opList[i];
            int rand = Random.Range(0, opList.Count);
            opList[i] = opList[rand];
            opList[rand] = n;
        }     

        //if our settings allow two operations
        if (MathSettingManager.Instance.twoOp)
        {
            //Finally we set the op2 to the first index of the randomly shuffled list of operations
            op2 = SetOp(opList[0]);
        }

        //Finally we set op1 to the 2nd index of the list
        op1 = SetOp(opList[1]);
        yield return null;
    }

    //Generates either 2 or 3 operands based on difficulty/settings
    //Our main problem is when we get divisions in the question, we want to prevent 0 as demoinator and decimal answers
    IEnumerator GenerateOperands()
    {
        //first we initialize the 3 operands from 1-9 randomly
        x = Random.Range(1, 10);
        y = Random.Range(1, 10);
        z = Random.Range(1, 10);

        //if our question allows parentheses or parantheses and division
        //We need to check this before generating any operands to prevent dividing by 0 or getting a decimal as answer
        if (parentheses && ((op1 == operators.division && parenthesesPos == 1) || (op2 == operators.division && parenthesesPos == 0)))
        {
            //if we have division as op1 and it's inside a parantheses
            if (op1 == operators.division && parenthesesPos == 1)
            {
                //if we get 0 in demoninator, we cant divide by 0
                if (DoSomeMath(op2, y, z) == 0)
                {
                    //so we'll set operand 2 to 9 or ++
                    if (y == 9)
                    { y = 1; }
                    else
                    { y = y + 1; }
                }
                //then we'll set operand 1 to the product of demoinator and itself to prevent getting decimal answers
                x = x * DoSomeMath(op2, y, z);
            }

            //if our op2 is division then we'll just generate a random divisor based on the other op and operand
            if (op2 == operators.division && parenthesesPos == 0)
            {
                z = GetRandomDivisor(DoSomeMath(op1, x, y));
            }
        }
        else //if our question have no parentheses
        {
            //once again we will get a random divisor if our op2 is division, but this time without parentheses in the question
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


    //Take our operations and operands and generate a string based on that to display
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

        //display our question
        QuestionManager.Instance.questionText.text = question;

        yield return null;
    }

    //return an operation given an integer
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

    //generate a random divisor/factor of a number
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

    //return a string based on an operation
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

    #region ANSWER GENERATION COROUTINES

    IEnumerator GenerateAnswer()
    {
        //First we will generate the correct answer based on the question
        yield return StartCoroutine("GenerateCorrectAnswer");

        //Second we will generate a list of wrong answers based on the correct one and store them into the answer bank
        yield return StartCoroutine("GenerateAnswerBank");
    }

    IEnumerator GenerateCorrectAnswer()
    {
        //if we have parentheses in the question
        if (parentheses)
        {
            //if parenthese is around operand x and y
            if (parenthesesPos == 0)
            {
                //generate our correct answer
                answer = DoSomeMath(op2, DoSomeMath(op1, x, y), z);
            }
            else   //if parenthese is around operand y and z
            {
                //generate our correct answer
                answer = DoSomeMath(op1, x, DoSomeMath(op2, y, z));
            }
        }
        else
        {
            //if our question only have 1 operation
            if (op2 == operators.none)
            {
                //generate our correct answer
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

    //we will generate 9 wrong answers
    IEnumerator GenerateAnswerBank()
    {
        answerBank.Clear();

        //add the correct answer to the list first
        answerBank.Add(answer);

        for (int i = -5; i < 0; i++)
        {
            answerBank.Add(answerBank[0] + i);
        }

        for (int i = 1; i < 5; i++)
        {
            answerBank.Add(answerBank[0] + i);
        }

        //shuffle our wrong answers while not touching the correct answer, so correct answer is at index 0
        yield return StartCoroutine(ShuffleAnswerBank(1, answerBank.Count));

        //then shuffle index 0, 1, 2 so we will have the correct answer shuffled with 2 other incorrect ones
        yield return StartCoroutine(ShuffleAnswerBank(0, 3));

    }

    //shuffles the answerbank from startindex to count
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

    //A helper function that helps us do math based 2 operands and an operation
    /// <summary>
    /// returns (a op b), a applied operation on b
    /// </summary>
    /// <param name="op"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
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
