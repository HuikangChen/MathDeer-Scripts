using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePickup : MonoBehaviour {

    public UpgradeManager.Upgrade upgradeName;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "PlayerPickupBox")
        {
            UpgradeManager.singleton.upgradePickedUp = upgradeName;
            QuestionManager.singleton.ActivateQuestion(transform.position);
            SoundManager.singleton.PlayAudioEffect(SoundManager.singleton.pickup);
            Destroy(gameObject);
        }
    }
}
