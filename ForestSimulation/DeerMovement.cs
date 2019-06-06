using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for moving the deers in the main menu scene
/// </summary>

public class DeerMovement : MonoBehaviour {

    [SerializeField] private float min_speed;
    [SerializeField] private float max_speed;

    [SerializeField] private float min_size;
    [SerializeField] private float max_size;

    //The random calculated speed
    private float speed;
    
    //The random calculated size
    private float size;

    private Rigidbody2D rb;
    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        //generate a random speed
        speed = Random.Range(min_speed, max_speed);

        //set animation speed based on movespeed
        anim.speed = speed / 3f;

        //set random size
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
