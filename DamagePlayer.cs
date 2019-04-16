using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DamagePlayer : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "PlayerHitBox")
        {
            CameraMovement.singleton.ShakeCamera(1, .5f);
            SoundManager.singleton.PlayAudioEffect(SoundManager.singleton.impact);
            GameManager.singleton.PlayerDead();
        }
    }
}
