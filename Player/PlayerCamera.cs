using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera that follows the player with shake functions
/// </summary>

public class PlayerCamera : MonoBehaviour {

    private static PlayerCamera instance;
    public static PlayerCamera Instance { get { return instance; } }

    //reference to the player
    [SerializeField]
    private GameObject player;

    //calculated at start based on the startRunningPos
    private float x_offset;

    [Tooltip("The y offset for following playing")]
    [SerializeField]
    private float y_offset;

    //The position that the player will start running from, used to calculate x offset
    [SerializeField]
    private Transform startFollowingPos;

    //Paralax uses this bool to check if player is running with the camera
    [HideInInspector]
    public bool playerRunningWithCamera;

    private bool isRunning = false; //Is the coroutine running right now?

    private float shakeDuration;

    //X component of a shake vector to add onto the movement vector of the camera to produce a shaking effect
    private float shake_offset_x;

    //Y component of a shake vector to add onto the movement vector of the camera to produce a shaking effect
    private float shake_offset_y;

    #region Unity Events
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

    private void Start()
    {
        //calculate our x offset so the player is not centered at the screen
        x_offset = transform.position.x - startFollowingPos.position.x;
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        //We only want to follow the player if its x position greater than the camera follow point
        if (player.transform.position.x < startFollowingPos.position.x)
        {
            playerRunningWithCamera = false;
            return;
        }

        playerRunningWithCamera = true;
        MoveCamera(ShakeOffset());
    }
    #endregion

    //move our camera given the shake vector
    void MoveCamera(Vector3 shake_offset)
    {
        Vector3 target = new Vector3(player.transform.position.x + x_offset, y_offset, -10) + shake_offset;
        transform.position = target;
    }

    #region Camera Shake Functions

    //Shake with amount and duration in random direction
    public void ShakeCamera(float amount, float duration)
    {
        //start our duration
        shakeDuration = duration;

        //Call our shake coroutine with the shake amount and duration
        StartCoroutine(Shake(amount, duration));
    }

    //Shake with amount and duration in a specified direction
    public void ShakeCamera(float amount, float duration, Vector2 dir)
    {
        //check if our shake coroutine is already runing
        if (isRunning)
            return;

        StartCoroutine(ShakeInDir(amount, duration, dir));
    }

    //Given the shake amount, duration, dir, we will set the shake vector multiplied by the amount for the duration
    IEnumerator ShakeInDir(float amount, float duration, Vector2 dir)
    {
        float time = duration;

        while (time > 0)
        {
            shake_offset_x = dir.normalized.x * Random.value * amount;
            shake_offset_y = dir.normalized.y * Random.value * amount;
            time -= Time.deltaTime;
            yield return null;
        }

        shake_offset_x = 0;
        shake_offset_y = 0;
    }
    
    public IEnumerator Shake(float amount, float duration)
    {
        isRunning = true;

        float time = duration;
        while (time > 0)
        {
            shake_offset_x = (Random.value / 20f) * amount;
            shake_offset_y = (Random.value / 20f) * amount;
            time -= Time.deltaTime;
            yield return null;
        }

        shake_offset_x = 0;
        shake_offset_y = 0;

        isRunning = false;
    }
    
    public IEnumerator Shake()
    {
        isRunning = true;

        float time = shakeDuration;
        while (time > 0)
        {
            shake_offset_x = Random.value / 20f;
            shake_offset_y = Random.value / 20f;
            time -= Time.deltaTime;
            yield return null;
        }

        shake_offset_x = 0;
        shake_offset_y = 0;

        isRunning = false;
    }

    //This is the shakeVector that's added to the move vector of the camera
    Vector3 ShakeOffset()
    {
        Vector3 offset = new Vector3(shake_offset_x, shake_offset_y, 0);
        return offset;
    }
    #endregion
}
