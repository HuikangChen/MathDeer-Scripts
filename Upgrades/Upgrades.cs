using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Upgrades : MonoBehaviour
{
    public enum Upgrade { None, Score_Multiplier, Coin_Multiplier, Magnet_Duration_Up, Shield_Duration_Up };
    public Upgrade upgradeName;

    [HideInInspector]
    public int currentLevel;

    [HideInInspector]
    public int maxLevel = 5;

    [SerializeField]
    //Used to calculate finalCost
    private int baseCost;

    [SerializeField]
    //Used to calculate finalCost
    private int costMultiplier = 1; 

    [SerializeField]
    //Used to calculate finalCost
    private int costIncreasePerLevel = 0; 

    [HideInInspector]
    //The final cost of our upgrade at current level
    public int finalCost; 

    [SerializeField]
    //display of the upgrade name
    private TextMeshProUGUI upgradeNameText;

    [SerializeField]
    //display of the cost of the upgrade
    private TextMeshProUGUI costText;

    [SerializeField]
    [Tooltip("The list of cells/bars gameobjects")]
    //The progress cells/bars to show current level of the upgrade
    private List<GameObject> levelBars = new List<GameObject>();


    //when the upgradepanel opens, we initialize our upgrades
    private void OnEnable()
    {
        SetUpgradeName();
        GetCurrentLevel();
        SetLevelBars();
        SetCost();
    }

    #region For initialization when panel opens
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
    #endregion

    //The upgrade function called when we click upgrade
    public void UpgradeButton()
    {
        if (currentLevel == maxLevel)
            return;

        //Calculate our final cost based on current level
        finalCost = CalculateCost();

        //check if we have enough coins to take
        if (GameStatManager.Instance.TakeCoin(finalCost))
        {
            currentLevel++; 

            //save current level into player prefs
            PlayerPrefs.SetInt(upgradeNameText.text, currentLevel);

            //Update cost, level display, totalcoins
            SetCost();
            SetLevelBars();
            UpgradeManager.Instance.UpdateTotalCoins();
        }
    }

    int CalculateCost()
    {
        return baseCost + ((baseCost * costMultiplier * currentLevel) + (costIncreasePerLevel * currentLevel));
    }
}
