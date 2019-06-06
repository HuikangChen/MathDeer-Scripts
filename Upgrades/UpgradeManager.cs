using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    private static UpgradeManager instance;
    public static UpgradeManager Instance { get { return instance; } }

    public Upgrades scoreMultiplier;
    public Upgrades coinMultiplier;
    public Upgrades magnetDurationUp;
    public Upgrades shieldDurationUp;

    [SerializeField]
    //The display of the total coins we currently have
    private TextMeshProUGUI totalCoinText; 

    private PanelTransitionManager transition;

    private void Awake()
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

    private void Start()
    {
        //transition holds all our panels for transitioning
        transition = PanelTransitionManager.Instance;

        //set our upgrade panel inactive when game starts
        transition.upgradePanel.SetActive(false);
    }

    //Gets called when panel is opened by GameManger to show our total coins and current upgrade levels
    public void Initialize()
    {
        UpdateTotalCoins();
        GetUpgradeLevels();
    }

    //Gets all the current level of each upgrades and displays them
    public void GetUpgradeLevels()
    {
        scoreMultiplier.GetCurrentLevel();
        coinMultiplier.GetCurrentLevel();
        magnetDurationUp.GetCurrentLevel();
        shieldDurationUp.GetCurrentLevel();
    }

    //gets saved coins from our playerpref
    public void UpdateTotalCoins()
    {
        totalCoinText.text = PlayerPrefs.GetInt("Coin", 0) + "";
    }

    public void CloseUpgradePanel()
    {
        transition.Slide(transition.upgradePanel, transition.end2, transition.start2, false);
    }
}
