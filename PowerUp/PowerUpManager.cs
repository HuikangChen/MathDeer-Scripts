using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    private static PowerUpManager instance;
    public static PowerUpManager Instance { get { return instance; } }

    public enum PowerUp {None, Magnet, Shield };

    [HideInInspector]
    public PowerUp powerUpPickedUp;

    [SerializeField]
    [Tooltip("The magnet gameobject we are setting active when activated")]
    private GameObject magnetPowerup;

    [SerializeField]
    [Tooltip("The shield gameobject we are setting active when activated")]
    private GameObject shieldPowerup;

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

    //Called by AnswerPickUp, when player picked up the correct answer
    public void ActivatePowerUp()
    {
        switch (powerUpPickedUp)
        {
            //this is when we picked up a star
            case PowerUp.None:
                GameStatManager.Instance.AddStar(1);
                GameStatManager.Instance.AddScore(5000 * UpgradeManager.Instance.scoreMultiplier.currentLevel);
                GameManager.Instance.IncreaseDifficulty(2);
                break;

            case PowerUp.Magnet:
                magnetPowerup.SetActive(true);
                magnetPowerup.GetComponent<Magnet>().Activate();
                GameManager.Instance.IncreaseDifficulty(2);
                break;

            case PowerUp.Shield:
                shieldPowerup.SetActive(true);
                shieldPowerup.GetComponent<Shield>().Activate();
                GameManager.Instance.IncreaseDifficulty(2);
                break;
        }
    }
}
