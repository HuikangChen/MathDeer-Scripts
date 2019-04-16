using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager singleton;
    public enum Upgrade {None, Score_Multiplier, Coin_Multiplier, Magnet_Duration_Up, Shield_Duration_Up };
    public Upgrade upgradePickedUp;

    public Upgrades scoreMultiplier;
    public Upgrades coinMultiplier;
    public Upgrades magnetDurationUp;
    public Upgrades shieldDurationUp;

    public GameObject magnetPowerup;
    public GameObject shieldPowerup;

    public TextMeshProUGUI totalCoinText;

    PanelTransitionManager transition;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        transition = PanelTransitionManager.singleton;
        transition.upgradePanel.SetActive(false);
    }

    public void Initialize()
    {
        UpdateTotalCoins();
        GetUpgradeLevels();
    }

    public void GetUpgradeLevels()
    {
        scoreMultiplier.GetCurrentLevel();
        coinMultiplier.GetCurrentLevel();
        magnetDurationUp.GetCurrentLevel();
        shieldDurationUp.GetCurrentLevel();
    }

    public void UpdateTotalCoins()
    {
        totalCoinText.text = PlayerPrefs.GetInt("Coin", 0) + "";
    }

    public void CloseUpgradePanel()
    {
        transition.Slide(transition.upgradePanel, transition.end2, transition.start2, false);
    }

    public void ActivatePowerUp()
    {
        switch (upgradePickedUp)
        {
            case Upgrade.None:
                GameStatManager.singleton.AddStar(1);
                GameStatManager.singleton.AddScore(5000 * UpgradeManager.singleton.scoreMultiplier.currentLevel);
                GameManager.singleton.IncreaseDifficulty(2);
                break;

            case Upgrade.Magnet_Duration_Up:
                magnetPowerup.SetActive(true);
                magnetPowerup.GetComponent<Magnet>().StartTimer();
                GameManager.singleton.IncreaseDifficulty(2);
                break;

            case Upgrade.Shield_Duration_Up:
                shieldPowerup.SetActive(true);
                shieldPowerup.GetComponent<Shield>().StartTimer();
                GameManager.singleton.IncreaseDifficulty(2);
                break;
        }
    }
}
