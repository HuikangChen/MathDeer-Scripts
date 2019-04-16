using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public float speed;
    public float baseLifeTime;
    [HideInInspector]public float lifeTime;
    GameObject player;
    public GameObject magnetEffect;

    private void Start()
    {
        player = PlayerController.singleton.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coin")
        {
            StartCoroutine(AttractCoin(collision.gameObject));
        }
    }

    IEnumerator AttractCoin(GameObject coin)
    {
        while (coin != null && Vector2.Distance(coin.transform.position, player.transform.position) > 0.1f)
        {
            coin.transform.position = Vector2.MoveTowards(coin.transform.position, player.transform.position, Time.deltaTime * speed);
            yield return null;
        }
    }

    public void StartTimer()
    {
        lifeTime = baseLifeTime + (baseLifeTime * ((float)UpgradeManager.singleton.magnetDurationUp.currentLevel / 2));
        StopCoroutine("TimerCo");
        StartCoroutine("TimerCo");
    }

    IEnumerator TimerCo()
    {
        magnetEffect.SetActive(true);

        while (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
            yield return null;
        }
        magnetEffect.SetActive(false);
        gameObject.SetActive(false);
    }
}
