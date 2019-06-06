using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the players physics, such as gravity and it's multipliers, move speeed, ground checking
/// Also contains the movement vector of the player
/// </summary>

[System.Serializable]
class PlayerPhysics
{
    public float gravity = -9.81f;

    [HideInInspector]
    public Vector3 velocity;

    [HideInInspector]
    public bool falling;

    [Tooltip("The gravity multiplier when player is falling")]
    public float fallMultiplier = 1.25f;

    [Tooltip("The gravity multiplier when player is jumping")]
    public float jumpMultiplier = 1f;

    #region Speed variables
    [Tooltip("The side scrolling speed")]
    public float horizontalSpeed = 2.5f;

    [Tooltip("The amount of speed increase when difficulty increases")]
    public int speedIncreaseFold = 2;

    [HideInInspector]
    public float baseSpeed;
    #endregion

    #region Ground check variables
    [SerializeField]
    [Tooltip("The gameobject used for ground checking with overlapcircle, should be under the player's feet")]
    private Transform GroundCheck;

    [SerializeField]
    [Tooltip("The layer we want to detect as ground")]
    private LayerMask groundLayer;

    [SerializeField]
    [Tooltip("The size of ground check")]
    private float groundCheckRadius = .1f;

    [HideInInspector]
    public bool isGrounded;
    #endregion

    public void Init()
    {
        //store a base speed variable to use later on 
        baseSpeed = horizontalSpeed;
    }

    //We apple our gravity and multipliers and moving the transform with controller2D given our velocity
    public void ApplyPhysics(Controller2D controller)
    {
        //applying a constant gravity to the velocity vector
        velocity.y += gravity * Time.fixedDeltaTime;

        //if we are falling
        if (velocity.y < 0)
        {
            //add fallmultiplier into our velocity vector
            velocity += Vector3.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        //if we are jumping
        else if (velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            //add jumpmultiplier into our velocity vector
            velocity += Vector3.up * Physics2D.gravity.y * (jumpMultiplier - 1) * Time.fixedDeltaTime;
        }

        //finally we are moving the transform with controller2D, and throwing in our velocity vector
        controller.Move(velocity * Time.fixedDeltaTime);
    }

    //Ground checking, we want to check for ground for jumping
    public void DoGroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, groundCheckRadius, groundLayer);
    }

    //adding the horizontalspeed to our velocity vector
    public void ApplyHorizontalMovement()
    {
        velocity.x = horizontalSpeed;
    }

    //checking if our controller2D's raycast is hitting anything above or below, then we set the velocity.y to 0
    public void ApplyVerticalCollision(Controller2D controller)
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
    }
}
