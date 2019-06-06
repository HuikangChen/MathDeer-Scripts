using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles spawning platforms as the player runs
/// Contains different tiers of difficulty platforms to spawn
/// </summary>

public class PlatformSpawnManager : MonoBehaviour {

    private static PlatformSpawnManager instance;
    public static PlatformSpawnManager Instance { get { return instance; } }

    //Used for calculating how close should the last spawned platform be to the camera in order to spawn a new platform
    [Tooltip("Camera's view size")]
    [SerializeField]
    private float viewSize;

    [Tooltip("Plaform's spawn point")]
    [SerializeField]
    private Transform spawnPoint;
    private bool gameStarted;

    [Space(10)]
    [Header("Platform Prefabs")]
    [SerializeField] private GameObject introPlatform; //Beginning platforms
    [SerializeField] private GameObject answerPlatform; //Platforms with the answers to the questions
    [SerializeField] private List<TierSpawn> spawns = new List<TierSpawn>(); //Our main platforms

    private List<Platform> currentPlatforms = new List<Platform>();
    private Camera cam;

    void Awake()
    {
        //Our singleton pattern
        if (instance != null && instance != this)
        {
            // destroy the gameobject if an instance of this exist already
            Destroy(gameObject);
        }
        else
        {
            //Set our instance to this object/instance
            instance = this;
        }
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

        //Check if we need to spawn new platforms
        if (NeedToSpawn())
        {
            //Spawn our next platform
            SpawnNextPlatform();
        }

        //check if we need to despawn the last platform 
        if (NeedToDespawn())
        {
            //despawn our last platform
            DespawnLastPlatform();
        }
	}

    //Checking when we need to spawn a new platform to the front most position based on camera's position
    bool NeedToSpawn()
    {
        //if we dont have any platforms yet, or when our front most platform's x position is less than the camera's position
        if (currentPlatforms.Count == 0 || 
            currentPlatforms[currentPlatforms.Count - 1].transform.position.x < cam.transform.position.x + (viewSize - 6))
        {
            return true;
        }
        return false;
    }

    //Checking when we need to despawn our last platform
    bool NeedToDespawn()
    {
        if (currentPlatforms.Count > 0 && currentPlatforms[0].transform.position.x < cam.transform.position.x - viewSize)
        {
            return true;
        }
        return false;
    }

    //despawning our last platform while making sure we despawn it's power ups and questions
    void DespawnLastPlatform()
    {
        //if the platform that we wanna delete has a powerup spawner on it
        if (currentPlatforms[0].GetComponent<PowerUpSpawner>())
        {
            //if our power up was spawned in the platform we want to destroy
            if (currentPlatforms[0].GetComponent<PowerUpSpawner>().powerUp != null && 
                currentPlatforms[0].GetComponent<PowerUpSpawner>().powerUp.activeInHierarchy)
            {
                PowerUpSpawner.powerUpSpawned = false;
            }
        }

        //if we have a question activated
        if (currentPlatforms[0].GetComponent<AnswerSpawner>() && 
            currentPlatforms[0].GetComponent<AnswerSpawner>().answer.activeInHierarchy &&
            QuestionManager.Instance.questionActivated)
        {
            QuestionManager.Instance.DeactivateQuestion();
        }

        RemovePlatform(currentPlatforms[0]);
    }

    //spawn our next platform when we are getting near the end
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
        int tier = (Mathf.Clamp(GameManager.Instance.difficulty, 1, 15) / 3);

        if (gameStarted == false)
        {
            obj = Instantiate(introPlatform,
                  pos,
                  Quaternion.identity);

            obj.SetActive(true);
        }
        else
        {
            if (QuestionManager.Instance.platformBufferAmount > 0)
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
