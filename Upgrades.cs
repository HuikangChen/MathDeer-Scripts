using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Upgrades : MonoBehaviour
{
    public UpgradeManager.Upgrade upgradeName;

    [HideInInspector] public int currentLevel;
    [HideInInspector] public int maxLevel = 5;
    public int baseCost;
    public int costMultiplier = 1;
    public int costIncreasePerLevel = 0;
    [HideInInspector]public int finalCost;

    public TextMeshProUGUI upgradeNameText;
    public TextMeshProUGUI costText;
    public List<GameObject> levelBars = new List<GameObject>();


    private void OnEnable()
    {
        SetUpgradeName();
        GetCurrentLevel();
        SetLevelBars();
        SetCost();
    }

    public void SetUpgradeName()
    {
        string name = upgradeName.ToString().Replace("_", " ");
        upgradeNameText.text = name;
    }

    public void GetCurrentLevel()
    {
        currentLevel = PlayerPrefs.GetInt(upgradeNameText.text, 0);
    }

    public void SetLevelBars()
    {
        for (int i = 0; i < maxLevel; i++)
        {
            if (currentLevel > i)
            {
                levelBars[i].gameObject.SetActive(true);
            }
            else
            {
                levelBars[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetCost()
    {
        if (currentLevel == maxLevel)
        {
            costText.text = "MAXED";
        }
        else
        {
            costText.text = CalculateCost().ToString();
        }
    }

    public void UpgradeButton()
    {
        if (currentLevel == maxLevel)
            return;

        finalCost = CalculateCost();
        if (GameStatManager.singleton.TakeCoin(finalCost))
        {
            currentLevel++;
            PlayerPrefs.SetInt(upgradeNameText.text, currentLevel);
            SetCost();
            SetLevelBars();
            UpgradeManager.singleton.UpdateTotalCoins();
        }
    }

    int CalculateCost()
    {
        return baseCost + ((baseCost * costMultiplier * currentLevel) + (costIncreasePerLevel * currentLevel));
    }
}
