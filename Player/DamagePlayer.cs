using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Deals damage to player, attached onto any gameobject that has a collider and damage player
/// </summary>

public class DamagePlayer : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "PlayerHitBox")
        {
            PlayerCamera.Instance.ShakeCamera(1, .5f);
            SoundManager.Instance.PlayAudioEffect(SoundManager.Instance.impact);
            GameManager.Instance.KillPlayer();
        }
    }
}
