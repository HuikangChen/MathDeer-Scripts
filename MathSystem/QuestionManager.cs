using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Handles math question activation and calls math question generation
/// Handles Checking the answer the player picks up
/// </summary>

[RequireComponent(typeof(MathGenerator))]
public class QuestionManager : MonoBehaviour {

    private static QuestionManager instance;
    public static QuestionManager Instance { get { return instance; } }

    //used for transitioning/moving the question display panel
    private PanelTransitionManager transition;

    #region UI/Display References
    [Header("[UI/Display References]")]

    //Our gameobject reference to the whole question display
    [SerializeField]
    private GameObject questionGO;

    //The border that surrounds the question display, will be changed to green/red to indicate correct/wrong answers
    [SerializeField]
    private Image questionBorder;

    //Our question display text
    public TextMeshProUGUI questionText;

    //The image of the powerup we picked up, will be set based on pickup
    [SerializeField]
    private GameObject pickupImage;

    //The star pickup sprite
    [SerializeField]
    private Sprite starSprite;

    //magnet pickup sprite
    [SerializeField]
    private Sprite magnetSprite;

    //shield pickup sprite
    [SerializeField]
    private Sprite shieldSprite;

    //the move speed of the pick up image moving to the question display
    [SerializeField]
    private float pickupImageMoveSpeed;

    //The position of the pickup image on the question display
    [SerializeField]
    private Transform pickupEndPos;

    //the position of the pick up in screen space
    private Vector2 pickupBeginPos;
    private Camera cam; //main cam will be used to convert world to screen space 
    #endregion

    //the amount of emepty platforms to spawn after the question is displayed to the player before we spawn the answer platforms
    //This give the player time to think about the answer
    [HideInInspector]
    public int platformBufferAmount;

    //PlatformSpawnManager uses this bool to check if question has been activated so it can spawn empty platforms
    [HideInInspector]
    public bool questionActivated;

    //Question Generator component
    [HideInInspector]
    public MathGenerator generator;

    void Awake()
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

        generator = GetComponent<MathGenerator>();
        questionGO.SetActive(false);
    }

    void Start()
    {
        cam = Camera.main;
        transition = PanelTransitionManager.Instance;
    }

    //Activate our question by generating the question and moving the pickup to the display
    public void ActivateQuestion(Vector2 startPos)
    {
        //get our begin position to move the pickup
        pickupBeginPos = cam.WorldToScreenPoint(startPos);

        //Question activation that handles the UI/display
        StartCoroutine("ActivateQuestionCo");
        
        //Generate the question and answers and displays it
        StartCoroutine(generator.GenerateQuestionAndAnswer());
    }

    //Sets up the question display and initializes the question
    IEnumerator ActivateQuestionCo()
    {
        //checks which power up we picked up, then we set the pickup image to the corresponding pickup
        switch (PowerUpManager.Instance.powerUpPickedUp)
        {
            case PowerUpManager.PowerUp.None:
                pickupImage.GetComponent<Image>().sprite = starSprite;
                break;

            case PowerUpManager.PowerUp.Magnet:
                pickupImage.GetComponent<Image>().sprite = magnetSprite;
                break;

            case PowerUpManager.PowerUp.Shield:
                pickupImage.GetComponent<Image>().sprite = shieldSprite;
                break;
        }

        //We set the pickup image's position to the position at where the player picked it up
        pickupImage.transform.position = pickupBeginPos;
        pickupImage.SetActive(true);

        //Display the question display/panel by sliding it down from the top
        PanelTransitionManager.Instance.Slide(transition.questionPanel, transition.start1, transition.end1, true);
        Color temp = questionBorder.color;
        temp.a = 0;
        questionBorder.color = temp;

        //Move the pickup image to the question display
        while (Vector2.Distance(pickupImage.transform.position, pickupEndPos.position) > 10f)
        {
            pickupImage.transform.position = Vector2.Lerp(pickupImage.transform.position, pickupEndPos.position, Time.deltaTime * pickupImageMoveSpeed);
            yield return null;
        }

        //calculate the the amount of empty platforms to spawn based on difficulty
        platformBufferAmount = (Mathf.Clamp(GameManager.Instance.difficulty, 1, 20) / 10) + 1;

        questionActivated = true;
    }

    //called after the answer is checked
    public void DeactivateQuestion()
    {
        //Slide our question display back up
        PanelTransitionManager.Instance.Slide(transition.questionPanel, transition.end1, transition.start1, false);

        //Set our bool back to false
        questionActivated = false;
        PowerUpSpawner.powerUpSpawned = false;
    }

    //called when the player picks up the answer
    public bool AnswerCheck(int answer)
    {
        //Adds the answer the player picked up to the MathResults, so we can display it at the end of the run
        AddResultToList(generator.answer, answer);

        //Display the effect of the answer that the player picked up
        //If its wrong, the border will turn red and shake
        //If its right, the border will turn green
        StartCoroutine(DisplayAnswerCheck(answer.ToString()));

        //If the answer the player picked up is the correct one we will return true
        if (answer == generator.answer)
        {
            return true;
        }
        return false;
    }

    //Adds the corrent answer and the answer the player chose to the math result list
    void AddResultToList(int correctAnswer, int chosenAnswer)
    {
        MathResultManager.Instance.Add(new MathResult(generator.question, correctAnswer, chosenAnswer));
    }

    //Display the effect of the answer that the player picked up
    //If its wrong, the border will turn red and shake
    //If its right, the border will turn green
    IEnumerator DisplayAnswerCheck(string answer)
    {
        generator.question = generator.question.Remove(generator.question.Length - 1);
        //question = question + answer;
        generator.question = "WRONG"; // change this later
        questionText.text = generator.question;

        if (answer == generator.answer.ToString())
        {

            pickupImage.transform.position = pickupEndPos.position;
            pickupImage.SetActive(true);
            DeactivateQuestion();
            while (Vector2.Distance(pickupImage.transform.position, cam.WorldToScreenPoint(PlayerController.Instance.transform.position)) > 40f)
            {
                pickupImage.transform.position = Vector2.MoveTowards(pickupImage.transform.position, cam.WorldToScreenPoint(PlayerController.Instance.transform.position), Time.deltaTime * pickupImageMoveSpeed * 100);
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

}
