using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerMovement : MonoBehaviour {

    public float min_speed;
    public float max_speed;

    public float min_size;
    public float max_size;

    float speed;
    float size;

    Rigidbody2D rb;
    Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        speed = Random.Range(min_speed, max_speed);
        anim.speed = speed / 3f;
        size = Random.Range(min_size, max_size);
        transform.localScale = transform.localScale * size;
    }

    void Start()
    {
        Destroy(gameObject, 20f);
    }

    void FixedUpdate()
    {
        rb.velocity = Vector2.right * speed;
    }
}
