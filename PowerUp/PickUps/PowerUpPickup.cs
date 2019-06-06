using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPickup : MonoBehaviour {

    public PowerUpManager.PowerUp pickupName;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "PlayerPickupBox")
        {
            //setting the powerup that we picked up so we can activate it later
            PowerUpManager.Instance.powerUpPickedUp = pickupName;

            //activating the math question
            QuestionManager.Instance.ActivateQuestion(transform.position);

            //play pickup sound
            SoundManager.Instance.PlayAudioEffect(SoundManager.Instance.pickup);

            Destroy(gameObject);
        }
    }
}
