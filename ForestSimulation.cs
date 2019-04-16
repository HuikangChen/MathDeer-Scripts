using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestSimulation : MonoBehaviour {

    public GameObject layer;
    public enum Direction { left, right}
    public Direction direction;

    public float move_duration;
    public float move_speed;
    

    void Start()
    {
        layer = gameObject;
        StartCoroutine("MoveLayer");
    }

    IEnumerator MoveLayer()
    {
        while (move_duration > 0)
        {
            move_duration -= Time.deltaTime;
            if (direction == Direction.left)
            {
                layer.transform.Translate(Vector2.left * Time.deltaTime * move_speed);
            }
            else
            {
                layer.transform.Translate(Vector2.right * Time.deltaTime * move_speed);
            }
            yield return null;
        }
    }
}
