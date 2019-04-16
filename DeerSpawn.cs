using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerSpawn : MonoBehaviour {

    public List<GameObject> deer_prefabs = new List<GameObject>();
    public Transform spawn_pos;

    public float min_cooldown;
    public float max_cooldown;

	// Use this for initialization
	void Start () {
        StartCoroutine("SpawnRoutine");
        GameManager.OnGameStart += Initialize;
	}

    private void Initialize()
    {
        StopCoroutine("SpawnRoutine");
    }

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
