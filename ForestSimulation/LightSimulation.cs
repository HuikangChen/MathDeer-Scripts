using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to simulate fading lights in the main menuscene
/// </summary>

public class LightSimulation : MonoBehaviour {

    [SerializeField] private float min_fade_speed = .05f;
    [SerializeField] private float max_fade_speed = .1f;
    [SerializeField] private float min_target_alpha = .2f;
    [SerializeField] private float max_target_alpha = .4f;
    [SerializeField] private float min_size = .5f;
    [SerializeField] private float max_size = 1f;

    private SpriteRenderer sprite;
    private Color color;
   
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        color = sprite.color;
    }

	// Use this for initialization
	void Start () {
        StartSimulation();
	}
  
    void StartSimulation()
    {
        StartCoroutine("Simulate");
    }

    //Slowly increases the alpha of the light to 1 then it waits a couple seconds to fade it back
    IEnumerator Simulate()
    {
        while (true)
        {
            if(Random.Range(0, 2) == 0)
            {
                yield return new WaitForSeconds(Random.Range(.5f, 1f));
            }

            yield return new WaitForSeconds(Random.Range(.5f, 3f));

            transform.localScale = new Vector3(Random.Range(min_size, max_size), transform.localScale.y, 1);

            float target_alpha = Random.Range(min_target_alpha, max_target_alpha);

            float fade_speed = Random.Range(min_fade_speed, max_fade_speed);

            while (sprite.color.a < target_alpha)
            {
                color = sprite.color;
                color.a += Time.deltaTime * fade_speed;
                sprite.color = color;
                yield return null;
            }
            

            yield return new WaitForSeconds(Random.Range(.5f, 2f));

            while (sprite.color.a > 0)
            {
                color = sprite.color;
                color.a -= Time.deltaTime * fade_speed;
                sprite.color = color;
                yield return null;
            }
            
        }
    }

}
