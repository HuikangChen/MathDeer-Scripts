using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSpawner : MonoBehaviour {

    public GameObject upgrade;
    public static bool upgradeSpawned;

    void OnEnable()
    {
        InitializePlatform();
    }

    void InitializePlatform()
    {
        TrySpawnStar();
    }

    void TrySpawnStar()
    {
        upgrade.SetActive(false);

        int rand = Random.Range(0, 2);

        if (rand == 0 && upgradeSpawned == false)
        {
            upgradeSpawned = true;
            upgrade.SetActive(true);
        }
    }

    //CALL THIS SHIT WHEN RESTARTING GAME!!!!!!!!!!!
    public static void Reset()
    {
        upgradeSpawned = false;
    }
}
