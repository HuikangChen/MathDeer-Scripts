using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Activates and contains the shield pickup effect
/// </summary>

public class Shield : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The duration of shield at level 1")]
    private float baseLifeTime;

    //Duration of shield based on it's current upgraded level
    private float lifeTime;

    [SerializeField]
    [Tooltip("The shield fx to set active")]
    private GameObject shieldEffect;

    //protecting our player when it hits an obstacle
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if it hits obstacle
        if (collision.tag == "Obstacle")
        {
            //disable our player's hitbox preventing it from getting hit for 2 seconds
            PlayerController.Instance.DisableHitBox(2f, false);

            //stop the duration of the shield since we hit an obstacle already
            StopCoroutine("TimerCo");

            //deactive the shield
            shieldEffect.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    //activate the shield powerup
    public void Activate()
    {
        //calculate the lifetime of shield based on current level of this powerup
        lifeTime = baseLifeTime + (baseLifeTime * ((float)UpgradeManager.Instance.magnetDurationUp.currentLevel / 2));

        //disabling the player's hitbox so the obstacle can't hit the player
        PlayerController.Instance.DisableHitBox(lifeTime, false);

        //starting the timer for shield
        StopCoroutine("TimerCo");
        StartCoroutine("TimerCo");
    }

    //timer for shield
    IEnumerator TimerCo()
    {
        shieldEffect.SetActive(true);
        while (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
            yield return null;
        }
        shieldEffect.SetActive(false);
        gameObject.SetActive(false);
    }
}
