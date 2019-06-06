using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour {
    
    //The upgrade we want to set active, it is on the platform
    public GameObject powerUp;

    //Used to check if we already spawned a powerup, because we only want 1 powerup present at a time
    public static bool powerUpSpawned;

    void OnEnable()
    {
        //When the platform spawns we will try to spawn a powerup on it
        TrySpawn();
    }

    //A random chance to spawn a powerup
    void TrySpawn()
    {
        powerUp.SetActive(false);

        // 1/3 chance of spawning a powerup
        int rand = Random.Range(0, 2);

        //if we hit the 1/3 chance and if there are currently no powerup spawned
        if (rand == 0 && powerUpSpawned == false)
        {
            powerUpSpawned = true;
            powerUp.SetActive(true);
        }
    }

    //Call this when restarting the game, we want to reset the static bool we used here
    public static void Reset()
    {
        powerUpSpawned = false;
    }
}
