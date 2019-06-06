using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles 2D collision, replacing Unity's 2d collision
/// A more precise and reliable collision with raycasting
/// Also moves the transform of the target given a velocity
/// </summary>

[System.Serializable]
public class Controller2D { 

    [SerializeField]
    //what we are colliding with
    private LayerMask collisionMask;

    //used to make sure we give a little spacing to cast our rays from the collider
    private const float skinWidth = .015f;
    
    [SerializeField]
    [Tooltip("Amount of rays to cast horizontally, the more the more accurate it is")]
    private int horizontalRayCount = 4;

    [SerializeField]
    [Tooltip("Amount of rays to cast vertically, the more the more accurate it is")]
    private int verticalRayCount = 4;

    //calculated based on amount of horizontal rays, the spacing between each rays are the same
    private float horizontalRaySpacing;

    //calculated based on amount of vertical rays, the spacing between each rays are the same
    private float verticalRaySpacing;

    //Our box collider only serves as boundary points, we handle the collisions ourselves in this script
    private BoxCollider2D collider;

    //The transform we are moving
    private Transform targetTransform;

    //Will be calculated based on the amount of rays
    private RaycastOrigins raycastOrigins;

    [HideInInspector]
    public CollisionInfo collisions;

	// Use this for initialization
	public void Init (BoxCollider2D c, Transform t) {
        collider = c;
        targetTransform = t;
        CalculateRaySpacing();
    }

    //Translating the target transform with velocity as the vector of movement, we are simulating rb's velocity
    public void Move(Vector3 velocity)
    {
        //we are getting and updating our raycast origins from the boxcollider of the target transform/gameobject
        //The raycast origins are going to be at the corners, we will use this to calculate where we shoot our rays
        UpdateRaycastOrigins();

        //Reset the collisions as this is our normal state
        collisions.Reset();

        //if we are moving horizontally
        if (velocity.x != 0)
        {
            //do horizontal collision 
            HorizontalCollisions(ref velocity);
        }

        //if we are moving vertically (jumping, falling)
        if (velocity.y != 0)
        {
            //do vertical raycast
            VerticalCollisions(ref velocity);
        }

        //This is what moves the transform
        targetTransform.Translate(velocity);
    }

    //We will be shooting rays horizontally, and detecting if it hits anything
    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i );
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }
    }

    //we will be shooting rays vertically detecting if it hits anything
    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
    }

    //we will get the origin of raycast from the corners of the box collider and with skinwidth as spacing
    void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    //Calculate the spacing between each rays
    void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    //The 4 corners of a box collider
    struct RaycastOrigins
    {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
    }

    //contains info on where we are hitting
    public struct CollisionInfo
    {
        public bool above;
        public bool below;
        public bool left;
        public bool right;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }
}
