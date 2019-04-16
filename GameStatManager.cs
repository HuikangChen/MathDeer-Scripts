using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStatManager : MonoBehaviour {

    public static GameStatManager singleton;

    public GameObject gameStat;
    
    [Header("Score Stuff")]
    public TextMeshProUGUI scoreText;
    public float scoreMultiplier;
    public float scoreIncreaseDelay;
    [HideInInspector] public float score;
    [HideInInspector] public float highScore;

    [Space(10)]
    [Header("Coin Stuff")]
    public TextMeshProUGUI coinText;
    [HideInInspector] public int coin;
    [HideInInspector] public int totalCoin;

    [Space(10)]
    [Header("Star Stuff")]
    public TextMeshProUGUI starText;
    [HideInInspector] public int star;
    [HideInInspector] public int totalStar;

    [HideInInspector] public int deaths;


    void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        GameManager.OnGameStart += Initialize;
        GameManager.OnGameStart += InitializeScore;
        GameManager.OnPlayerDead += OnPlayerDead;
    }

    void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
    }

    IEnumerator ScoreIncreaseOverTime()
    {
        while (true)
        {
            AddScore(scoreMultiplier + (UpgradeManager.singleton.scoreMultiplier.currentLevel * scoreMultiplier));
            yield return new WaitForSeconds(scoreIncreaseDelay);
        }
    }

    public void Initialize()
    {
        gameStat.SetActive(true);
        StartCoroutine("ScoreIncreaseOverTime");
    }

    public void InitializeScore()
    {
        highScore = PlayerPrefs.GetFloat("HighScore", 0);
        totalStar = PlayerPrefs.GetInt("Star", 0);
        totalCoin = PlayerPrefs.GetInt("Coin", 0);

        UpdateUI(scoreText, (int)score);
        UpdateUI(coinText, coin);
        UpdateUI(starText, totalStar);
    }

    public void UpdateUI(TextMeshProUGUI text, int value)
    {
        text.text = value + "";
    }

    public void AddScore(float amount)
    {
        score += amount;
        UpdateUI(scoreText, (int)score);
    }

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

    public void StartScore()
    {
        StartCoroutine("ScoreIncreaseOverTime");
    }

    public void StopScore()
    {
        StopCoroutine("ScoreIncreaseOverTime");
    }

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

    void OnPlayerDead()
    {
        deaths++;
        StopScore();
    }

    private void OnDestroy()
    {
        GameManager.OnPlayerDead -= OnPlayerDead;
        GameManager.OnGameStart -= Initialize;
        GameManager.OnGameStart -= InitializeScore;
    }
}
