using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for spawning deers in the main menu
/// </summary>

public class DeerSpawn : MonoBehaviour {

    //Deer prefabs to spawn at the main menu scene
    [Tooltip("the deer prefabs to be spawned in the beginning menu scene")]
    [SerializeField]
    private List<GameObject> deer_prefabs = new List<GameObject>();

    //Spawn position for the deers
    [Tooltip("Spawn position of the deer_prefabs")]
    [SerializeField]
    private Transform spawn_pos;

    //Min and max cooldown between spawns
    [Tooltip("The minimum cooldown of the deer spawn, will generate a random size between min and max")]
    [SerializeField]
    private float min_cooldown;

    [Tooltip("The maximum cooldown of the deer spawn, will generate a random size between min and max")]
    [SerializeField]
    private float max_cooldown;

	// Use this for initialization
	void Start () {
        StartCoroutine("SpawnRoutine");
        GameManager.OnGameStart += Initialize;
	}

    private void Initialize()
    {
        StopCoroutine("SpawnRoutine");
    }

    //Our coroutine to spawn the spawns
    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            int count = Random.Range(0, deer_prefabs.Count + 1);

            for (int i = 0; i < count; i++)
            {
                Instantiate(deer_prefabs[i], new Vector3(spawn_pos.position.x, spawn_pos.position.y, 0), Quaternion.identity);
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(min_cooldown, max_cooldown));
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStart -= Initialize;
    }
}
