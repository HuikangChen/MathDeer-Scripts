using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// The game manager that handles the game's core loop and displays the result/math results panels
/// </summary>

public class GameManager : MonoBehaviour {

    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    #region Events
    public delegate void GameStateHandler();
    public static event GameStateHandler OnDifficultyChange;
    public static event GameStateHandler OnPlayerDead;
    public static event GameStateHandler OnGameStart;
    #endregion

    #region Start Game variables
    [SerializeField] private GameObject startButton; 
    [SerializeField] private GameObject title;

    public GameObject jumpText;
    public GameObject dashText;
    #endregion

    #region Panels variables
    [Space(10)]
    [Header("Continue Panel Variables")]
    [SerializeField] private GameObject continueGame;
    [SerializeField] private Image continueBar;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private float continueTime = 3;

    
    [Space(10)]
    [Header("Results Variables")]
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private TextMeshProUGUI scoreResult;
    [SerializeField] private TextMeshProUGUI coinResult;
    [SerializeField] private TextMeshProUGUI starResult;
    [SerializeField] private TextMeshProUGUI scoreName;
    [SerializeField] private TextMeshProUGUI highScoreResult;

    [Space(10)]
    [Header("Math Results Variables")]
    [SerializeField] private GameObject mathResultsPanel;
    #endregion

    [Header("Difficulty Setting")]
    public int difficulty;

    private PanelTransitionManager transition;
    public GameObject blackPanel;

    #region Unity Events
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
    }

    void Start()
    {
        //set the fps of platform
        if (Application.platform == RuntimePlatform.Android)
        {
            Application.targetFrameRate = 60;
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
        transition = PanelTransitionManager.Instance;
        SoundManager.Instance.PlayMainAudio(SoundManager.Instance.mainBG);
    }
    #endregion

    #region Game Loop
    //Called by some button
    public void StartGame()
    {
        if (OnGameStart != null)
        {
            //Show the hint text for jumping and dashing
            jumpText.SetActive(true);
            dashText.SetActive(true);

            //deactivate our menu buttons
            startButton.SetActive(false);
            title.SetActive(false);
            OnGameStart();
        }
    }

    //Kills the player and starts the continue sequence
    public void KillPlayer()
    {
        if (OnPlayerDead != null)
        {
            SoundManager.Instance.mainSource.Stop();
            OnPlayerDead();
        }
        StartCoroutine("ContinueSequence");
    }

    //Shows the continue box to let player continue game by using stars
    IEnumerator ContinueSequence()
    {
        //Set the black panel to dim the background
        blackPanel.SetActive(true);

        //Disable movement and input
        PlayerController.Instance.DisableMovement();
        PlayerController.Instance.DisableInput();

        //moves the continue panel down to the center of the screen
        transition.Slide(transition.continuePanel, transition.start1, transition.end2, true);

        //Shows the amount of stars they need inorder to continue the game
        amountText.text = "x" + GameStatManager.Instance.deaths;

        //Start the countdown of the timer they have to decide
        yield return StartCoroutine("StartContinueProgressBar");

        //Slide the continue panel back up
        yield return StartCoroutine(transition.SlideCo(transition.continuePanel, transition.end2, transition.start1, false));

        //Display the result panel if the player cant continue
        DisplayResultsPanel();
    }

    //Start the countdown of the timer for the player to continue the game
    IEnumerator StartContinueProgressBar()
    {
        continueBar.fillAmount = 1;

        while (continueBar.fillAmount > 0.01)
        {
            continueBar.fillAmount -= Time.deltaTime / continueTime;
            yield return null;
        }

        continueBar.fillAmount = 0;
    }

    //Checks if the player has enough star to continue the game
    public bool ContinueGame()
    {
        //if player has enough stars
        if (GameStatManager.Instance.TakeStar(GameStatManager.Instance.deaths))
        {
            //Stops the continue sequence so we dont end up displaying the result panel
            StopCoroutine("ContinueSequence");
            SoundManager.Instance.PlayMainAudio(SoundManager.Instance.mainBG);

            //Enalbe the movement and input again
            PlayerController.Instance.EnableMovement();
            PlayerController.Instance.EnableInput();

            //Disable the hitbox so player has some time to recover
            PlayerController.Instance.DisableHitBox(2f, true);

            //start the score increase again
            GameStatManager.Instance.StartScore();
            return true;
        }
        return false;
    }

    //The function that's caled when the "continue" button is pressed
    public void TryContinueGame()
    {
        //checks if we can contine with enough stars
        if (continueGame.activeInHierarchy && ContinueGame())
        {
            transition.Slide(transition.continuePanel, transition.end2, transition.start1, false);
            blackPanel.SetActive(false);
        }
    }

    //Restarts our game if player cannot continue
    public void RestartGame()
    {
        PowerUpSpawner.Reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion

    #region Displaying Panels functions

    //Display our results panel at the end of the game
    public void DisplayResultsPanel()
    {
        //moves our panel
        transition.Slide(transition.resultPanel, transition.start2, transition.end2, true);

        //initialize and display our results on the panel
        GameStatManager.Instance.InitializeScore();
        scoreResult.text = GameStatManager.Instance.score + "";
        coinResult.text = GameStatManager.Instance.coin + "";
        starResult.text = GameStatManager.Instance.star + "";

        //if we have a new highscore
        if (GameStatManager.Instance.NewHighScore())
        {
            scoreName.text = "NEW HIGH SCORE";
        }
        else
        {
            scoreName.text = "SCORE";
        }

        //update our highscore 
        GameStatManager.Instance.UpdateUI(highScoreResult, (int)GameStatManager.Instance.highScore);
    }

    public void DisplayMathResultsPanel()
    {
        transition.Slide(transition.mathResultPanel, transition.start2, transition.end2, true);
        MathResultManager.Instance.Initialize();
    }

    public void DisplayUpgradesPanel()
    {
        transition.Slide(transition.upgradePanel, transition.start2, transition.end2, true);
        UpgradeManager.Instance.Initialize();
    }
    #endregion

    public void IncreaseDifficulty(int amount)
    {
        difficulty += amount;
        if (OnDifficultyChange != null)
        {
            OnDifficultyChange();
        }
    }
}
