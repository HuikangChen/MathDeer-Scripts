using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// Handles displaying score/stars/coins during game
/// Handles displaying the run's stats after player loses
/// </summary>

public class GameStatManager : MonoBehaviour {

    private static GameStatManager instance;
    public static GameStatManager Instance { get { return instance; } }

    /// <summary>
    /// The display gameobject that contains the score/stars/coins during the game
    /// </summary>
    [SerializeField]
    private GameObject gameStatDisplay;
    
    [Header("Score variables")]

    [Tooltip("The text display of the score during the game")]
    [SerializeField]
    private TextMeshProUGUI scoreDisplay;

    [Tooltip("Amount of score to add between each delay")]
    [SerializeField]
    private float scoreMultiplier;

    [Tooltip("the delay between each score increase")]
    [SerializeField]
    private float scoreIncreaseDelay;

    //our current score
    [HideInInspector] public float score;

    //our high score that will be retrieved from playerprefs
    [HideInInspector] public float highScore;

    [Space(10)]
    [Header("Coin variables")]

    [Tooltip("The text display of the coins during the game")]
    [SerializeField]
    private TextMeshProUGUI coinText;

    [HideInInspector] public int coin;
    [HideInInspector] public int totalCoin;

    [Space(10)]
    [Header("Star variables")]

    [Tooltip("The text display of the stars during the game")]
    [SerializeField]
    private TextMeshProUGUI starText;
    [HideInInspector] public int star;
    [HideInInspector] public int totalStar;

    [HideInInspector] public int deaths;

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

    //Subcribe to our game events
    void Start()
    {
        GameManager.OnGameStart += Initialize;
        GameManager.OnGameStart += InitializeScore;
        GameManager.OnPlayerDead += OnPlayerDead;
    }

    //Unsubscribe to our game events 
    private void OnDestroy()
    {
        GameManager.OnPlayerDead -= OnPlayerDead;
        GameManager.OnGameStart -= Initialize;
        GameManager.OnGameStart -= InitializeScore;
    }
    #endregion

    //Invoked by the gamestart event, shows our game stats and starts score increase
    public void Initialize()
    {
        gameStatDisplay.SetActive(true);
        StartCoroutine("ScoreIncreaseOverTime");
    }

    #region Score Functions

    //Initialize our variables and display it
    public void InitializeScore()
    {
        //set our score/star/coin amounts from playerprefs
        highScore = PlayerPrefs.GetFloat("HighScore", 0);
        totalStar = PlayerPrefs.GetInt("Star", 0);
        totalCoin = PlayerPrefs.GetInt("Coin", 0);

        //Display those variables on our ui
        UpdateUI(scoreDisplay, (int)score);
        UpdateUI(coinText, coin);
        UpdateUI(starText, totalStar);
    }

    //Start our score increase coroutine
    public void StartScore()
    {
        StartCoroutine("ScoreIncreaseOverTime");
    }

    //stop our score increase coroutine
    public void StopScore()
    {
        StopCoroutine("ScoreIncreaseOverTime");
    }

    //Increase the score over time, increases by score multiplier + the upgrade level in a certain delay
    IEnumerator ScoreIncreaseOverTime()
    {
        while (true)
        {
            AddScore(scoreMultiplier + (UpgradeManager.Instance.scoreMultiplier.currentLevel * scoreMultiplier));
            yield return new WaitForSeconds(scoreIncreaseDelay);
        }
    }

    //Adds a specific amount of score 
    public void AddScore(float amount)
    {
        score += amount;
        UpdateUI(scoreDisplay, (int)score);
    }

    //Checks if our score is a new high score at the end of a run
    public bool NewHighScore()
    {
        if ((int)score > (int)highScore)
        {
            highScore = score;
            PlayerPrefs.SetFloat("HighScore", highScore);
            return true;
        }
        return false;
    }
    #endregion

    #region Coin Functions
    public void AddCoin(int amount)
    {
        coin += amount;
        totalCoin += amount;
        PlayerPrefs.SetInt("Coin", totalCoin);
        UpdateUI(coinText, coin);
    }

    public bool TakeCoin(int amount)
    {
        if (totalCoin - amount >= 0)
        {
            totalCoin -= amount;
            PlayerPrefs.SetInt("Coin", totalCoin);
            UpdateUI(coinText, coin);
            return true;
        }
        return false;
    }
    #endregion

    #region Star Functions
    public void AddStar(int amount)
    {
        star += amount;
        totalStar += amount;
        PlayerPrefs.SetInt("Star", totalStar);
        UpdateUI(starText, totalStar);
    }

    public bool TakeStar(int amount)
    {
        if (totalStar - amount >= 0)
        {
            totalStar -= amount;
            PlayerPrefs.SetInt("Star", totalStar);
            UpdateUI(starText, totalStar);
            return true;
        }
        return false;
    }
    #endregion

    void OnPlayerDead()
    {
        deaths++;
        StopScore();
    }

    //Delete all our saved data
    void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
    }

    public void UpdateUI(TextMeshProUGUI text, int value)
    {
        text.text = value + "";
    }
}
