using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour {

    [Tooltip("The fx to spawn when a coin is picked up")]
    [SerializeField]
    private GameObject pickupFX;

    [Tooltip("The amount of coins to add when a coin is picked up")]
    [SerializeField]
    private int coinAmount = 1;

    [Tooltip("The amount of score to add when a coin is picked up")]
    [SerializeField]
    private int scoreAmount = 50;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "PlayerPickupBox")
        {
            //Add the amount of coins with the coin multiplier upgrade in the equation
            GameStatManager.Instance.AddCoin(coinAmount + (coinAmount * UpgradeManager.Instance.coinMultiplier.currentLevel));

            //Add the amount of score with the score multiplier upgrade in the equation
            GameStatManager.Instance.AddScore(scoreAmount + (scoreAmount * UpgradeManager.Instance.scoreMultiplier.currentLevel));

            //spawn fx and play fx
            GameObject obj = PoolManager.Spawn(pickupFX, transform.position, Quaternion.identity);
            obj.GetComponent<ParticleSystem>().Play();
            SoundManager.Instance.PlayAudioEffect(SoundManager.Instance.coin);
            PoolManager.Despawn(obj, 3f);
            gameObject.SetActive(false);
        }
    }
}
