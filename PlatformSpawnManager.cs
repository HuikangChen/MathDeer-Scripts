using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawnManager : MonoBehaviour {

    public static PlatformSpawnManager singleton;

    public float viewSize; // How close should the last spawned platform be to the camera in order to spawn a new platform

    public Transform spawnPoint;
    bool gameStarted;

    [Space(10)]
    [Header("Platform Prefabs")]
    public GameObject introPlatform;
    public GameObject answerPlatform;
    public List<TierSpawn> spawns = new List<TierSpawn>();

    List<Platform> currentPlatforms = new List<Platform>();
    Camera cam;

    void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        cam = Camera.main;
        GameManager.OnGameStart += Initialize;
    }

    void Initialize()
    {
        gameStarted = true;
    }

	void Update () {
        if (NeedToSpawn())
        {
            SpawnNextPlatform();
        }

        if (NeedToDespawn())
        {
            DespawnLastPlatform();
        }
	}

    bool NeedToSpawn()
    {
        if (currentPlatforms.Count == 0 || currentPlatforms[currentPlatforms.Count - 1].transform.position.x < cam.transform.position.x + (viewSize - 6))
        {
            return true;
        }
        return false;
    }

    bool NeedToDespawn()
    {
        if (currentPlatforms.Count > 0 && currentPlatforms[0].transform.position.x < cam.transform.position.x - viewSize)
        {
            return true;
        }
        return false;
    }

    void DespawnLastPlatform()
    {
        if (currentPlatforms[0].GetComponent<UpgradeSpawner>())
        {
            if (currentPlatforms[0].GetComponent<UpgradeSpawner>().upgrade != null && currentPlatforms[0].GetComponent<UpgradeSpawner>().upgrade.activeInHierarchy)
            {
                UpgradeSpawner.upgradeSpawned = false;
            }
        }

        if (currentPlatforms[0].GetComponent<AnswerSpawner>() && currentPlatforms[0].GetComponent<AnswerSpawner>().answer.activeInHierarchy && QuestionManager.singleton.questionActivated)
        {
            QuestionManager.singleton.DeactivateQuestion();
        }

        RemovePlatform(currentPlatforms[0]);
    }

    void SpawnNextPlatform()
    {
       

        if (currentPlatforms.Count == 0)
        {
            SpawnPlatform(new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, 0));
        }
        else
        {
            SpawnPlatform(new Vector3(currentPlatforms[currentPlatforms.Count - 1].transform.position.x + currentPlatforms[currentPlatforms.Count - 1].size,
                                                       spawnPoint.transform.position.y, 
                                                       0));
        }
    }

    void SpawnPlatform(Vector3 pos)
    {
        GameObject obj;
        int tier = (Mathf.Clamp(GameManager.singleton.difficulty, 1, 15) / 3);

        if (gameStarted == false)
        {
            obj = Instantiate(introPlatform,
                  pos,
                  Quaternion.identity);

            obj.SetActive(true);
        }
        else
        {
            if (QuestionManager.singleton.platformBufferAmount > 0)
            {
                obj = Instantiate(answerPlatform, pos, Quaternion.identity);
                obj.SetActive(true);
            }
            else
            {
                obj = Instantiate(spawns[tier].platforms[Random.Range(0, spawns[tier].platforms.Count)],
                                  pos,
                                  Quaternion.identity);

                obj.SetActive(true);
            }
        }
        currentPlatforms.Add(obj.GetComponent<Platform>());
    }

    public void RemovePlatform(Platform platform)
    {
        currentPlatforms.Remove(platform);
        Destroy(platform.gameObject);
    }

    private void OnDestroy()
    {
        GameManager.OnGameStart -= Initialize;
    }

    [System.Serializable]
    public class TierSpawn
    {
        public List<GameObject> platforms = new List<GameObject>();
    }
}
