using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour {

    public static GameManager singleton;
    public delegate void DifficultyHandler(int value);
    public static event DifficultyHandler OnDifficultyChange;

    public delegate void DeathHandler();
    public static event DeathHandler OnPlayerDead;

    public delegate void GameStateHandler();
    public static event GameStateHandler OnGameStart;
    public GameObject startButton;
    public GameObject title;

    PanelTransitionManager transition;
    public GameObject blackPanel;

    public GameObject jumpText;
    public GameObject dashText;

    [Header("Difficulty Setting")]
    public int difficulty;

    [Space(10)]
    [Header("Continue Panel Stuff")]
    public GameObject continueStuff;
    public Image continueBar;
    public TextMeshProUGUI amountText;
    public float continueTime = 3;

    [Space(10)]
    [Header("Results Panel")]
    public GameObject resultsPanel;
    public TextMeshProUGUI scoreResult;
    public TextMeshProUGUI coinResult;
    public TextMeshProUGUI starResult;
    public TextMeshProUGUI scoreName;
    public TextMeshProUGUI highScoreResult;

    [Space(10)]
    [Header("Math Results Panel")]
    public GameObject mathResultsPanel;

    void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Application.targetFrameRate = 60;
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
        transition = PanelTransitionManager.singleton;
        SoundManager.singleton.PlayMainAudio(SoundManager.singleton.mainBG);
    }

    public void TryContinueGame()
    {
        if (continueStuff.activeInHierarchy && ContinueGame())
        {
            transition.Slide(transition.continuePanel, transition.end2, transition.start1, false);
            blackPanel.SetActive(false);
        }
    }

    public void PlayerDead()
    {
        if (OnPlayerDead != null)
        {
            SoundManager.singleton.mainSource.Stop();
            OnPlayerDead();
        }
        StartCoroutine("ContinueSequence");
    }

    IEnumerator ContinueSequence()
    {
        blackPanel.SetActive(true);
        PlayerMovement.singleton.DisableMovement();
        transition.Slide(transition.continuePanel, transition.start1, transition.end2, true);
        amountText.text = "x" + GameStatManager.singleton.deaths;
        yield return StartCoroutine("StartContinueProgressBar");
        yield return StartCoroutine(transition.SlideCo(transition.continuePanel, transition.end2, transition.start1, false));
        DisplayResultsPanel();
    }

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

    public bool ContinueGame()
    {
        if (GameStatManager.singleton.TakeStar(GameStatManager.singleton.deaths))
        {
            StopCoroutine("ContinueSequence");
            SoundManager.singleton.PlayMainAudio(SoundManager.singleton.mainBG);
            PlayerMovement.singleton.EnableMovement();
            PlayerController.singleton.DisableHitBox(2f, true);
            GameStatManager.singleton.StartScore();
            return true;
        }
        return false;
    }

    public void RestartGame()
    {
        UpgradeSpawner.Reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void DisplayResultsPanel()
    {
        transition.Slide(transition.resultPanel, transition.start2, transition.end2, true);
        GameStatManager.singleton.InitializeScore();
        scoreResult.text = GameStatManager.singleton.score + "";
        coinResult.text = GameStatManager.singleton.coin + "";
        starResult.text = GameStatManager.singleton.star + "";

        if (GameStatManager.singleton.NewHighScore())
        {
            scoreName.text = "NEW HIGH SCORE";
        }
        else
        {
            scoreName.text = "SCORE";
        }
        GameStatManager.singleton.UpdateUI(highScoreResult, (int)GameStatManager.singleton.highScore);

    }

    public void DisplayMathResultsPanel()
    {
        transition.Slide(transition.mathResultPanel, transition.start2, transition.end2, true);
        MathResultManager.singleton.Initialize();
    }

    public void DisplayUpgradesPanel()
    {
        transition.Slide(transition.upgradePanel, transition.start2, transition.end2, true);
        UpgradeManager.singleton.Initialize();
    }

    public void IncreaseDifficulty(int amount)
    {
        difficulty += amount;
        if (OnDifficultyChange != null)
        {
            OnDifficultyChange(difficulty);
        }
    }

    public void StartGame()
    {
        if (OnGameStart != null)
        {
            jumpText.SetActive(true);
            dashText.SetActive(true);
            startButton.SetActive(false);
            title.SetActive(false);
            OnGameStart();
        }
    }
}
