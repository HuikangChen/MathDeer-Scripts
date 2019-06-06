using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Activate and Contains the magnet pickup effect
/// </summary>

public class Magnet : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The speed of coins flying to the player")]
    private float speed;

    [SerializeField]
    [Tooltip("Duration of magnet at level 1")]
    private float baseLifeTime;

    //Duration of magnet based on it's current upgraded level
    private float lifeTime;

    [SerializeField]
    [Tooltip("The magnet fx to set active")]
    private GameObject magnetEffect;

    private GameObject player;

    private void Start()
    {
        player = PlayerController.Instance.gameObject;
    }

    //The magnet uses a large collider to pick up coins
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coin")
        {
            //start our coin attracting coroutine
            StartCoroutine(AttractCoin(collision.gameObject));
        }
    }

    //moves the coins towards the player's position
    IEnumerator AttractCoin(GameObject coin)
    {
        while (coin != null && Vector2.Distance(coin.transform.position, player.transform.position) > 0.1f)
        {
            coin.transform.position = Vector2.MoveTowards(coin.transform.position, player.transform.position, Time.deltaTime * speed);
            yield return null;
        }
    }

    //Activate our magnet powerup
    public void Activate()
    {
        //calculate our lifetime based on current level of this powerup
        lifeTime = baseLifeTime + (baseLifeTime * ((float)UpgradeManager.Instance.magnetDurationUp.currentLevel / 2));

        StopCoroutine("TimerCo");
        StartCoroutine("TimerCo");
    }

    //coroutine to time the duration of magnet
    IEnumerator TimerCo()
    {
        //show our magnet effect 
        magnetEffect.SetActive(true);

        //timer
        while (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
            yield return null;
        }

        //deactivate our magnet fx and gameobject
        magnetEffect.SetActive(false);
        gameObject.SetActive(false);
    }
}
