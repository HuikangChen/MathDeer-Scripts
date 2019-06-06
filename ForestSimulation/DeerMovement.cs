using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for moving the deers in the main menu scene, will generate a random speed and size for the deer
/// Part of simulating a forest for the animations in the main menu
/// </summary>

public class DeerMovement : MonoBehaviour {

    [Tooltip("The minimum speed of the deer's movement, will generate a random speed between min and max")]
    [SerializeField]
    private float min_speed;

    [Tooltip("The maximum speed of the deer's movement, will generate a random speed between min and max")]
    [SerializeField]
    private float max_speed;

    [Tooltip("The minimum size of the deer's movement, will generate a random size between min and max")]
    [SerializeField]
    private float min_size;

    [Tooltip("The maximum size of the deer's movement, will generate a random size between min and max")]
    [SerializeField]
    private float max_size;

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
