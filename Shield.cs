using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public float baseLifeTime;
    [HideInInspector] public float lifeTime;
    public GameObject shieldEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Obstacle")
        {
            PlayerController.singleton.DisableHitBox(2f, false);
            StopCoroutine("TimerCo");
            shieldEffect.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    public void StartTimer()
    {
        lifeTime = baseLifeTime + (baseLifeTime * ((float)UpgradeManager.singleton.magnetDurationUp.currentLevel / 2));
        PlayerController.singleton.DisableHitBox(lifeTime, false);
        StopCoroutine("TimerCo");
        StartCoroutine("TimerCo");
    }

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
