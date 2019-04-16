using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour {

    public GameObject pickupFX;
    public int coinAmount = 1;
    public int scoreAmount = 50;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "PlayerPickupBox")
        {
            GameStatManager.singleton.AddCoin(coinAmount + (coinAmount * UpgradeManager.singleton.coinMultiplier.currentLevel));
            GameStatManager.singleton.AddScore(scoreAmount + (scoreAmount * UpgradeManager.singleton.scoreMultiplier.currentLevel));
            GameObject obj = PoolManager.Spawn(pickupFX, transform.position, Quaternion.identity);
            obj.GetComponent<ParticleSystem>().Play();
            SoundManager.singleton.PlayAudioEffect(SoundManager.singleton.coin);
            PoolManager.Despawn(obj, 3f);
            gameObject.SetActive(false);
        }
    }
}
