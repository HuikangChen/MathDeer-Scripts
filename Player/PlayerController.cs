using System.Collections;
using UnityEngine;
using InControl;

/// <summary>
/// This script controls all of the player's components and funtionalities. 
/// Also controlling other components such as hitboxes and particletrails
/// </summary>

public class PlayerController : MonoBehaviour {

    private static PlayerController instance;
    public static PlayerController Instance { get { return instance; } }

    #region Inspector Variables, makes up our player's functionality
    [SerializeField]
    private Controller2D controller; //Handles the collisions and translations of the player's transform

    [SerializeField]
    private PlayerInput input; //Handles input from keyboard/controller/touch for jump and dashing

    [SerializeField]
    private PlayerPhysics physics; //Handles the velocity, gravity, ground checking

    [SerializeField]
    private PlayerJumpSettings jumpSettings; //Contains variables for jump 

    [SerializeField]
    private PlayerDashSettings dashSettings; //Contains variables for dash
    #endregion

    private bool movementDiabled;

    [Header("[Component References]")]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ParticleSystem particleTrail;
    [SerializeField] private SpriteRenderer deerSprite;
    [SerializeField] private Collider2D hitBox;
    [SerializeField] private Collider2D pickupBox;

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

    void Start()
    {
        //Disable movement and input on start until player clicks on "playgame"
        DisableMovement();
        DisableInput();

        //Subcribe player's initialization to ongamestart
        GameManager.OnGameStart += Init; 
    }

    void Update()
    {
        //Recieves input from player through keyboard/controller/touch and fires the events
        input.GetInputs();

        //Sets our player's animations
        SetAnims();
    }

    //We apply all our player's physics in FixedUpdate
    void FixedUpdate()
    {
        //if movement is disabled, we want to set velocity to zero
        if (movementDiabled)
        {
            physics.velocity = Vector2.zero;
            return;
        }

        //apply our gravity and velocity and moving the transform with controller2D
        //We call this before vertical Collision so it doesn't set our velocity.y to 0 every frame which stops our player from jumping
        physics.ApplyPhysics(controller);

        //sets velocity.y to 0 when collision is detected in order to simulate vertical collisions
        physics.ApplyVerticalCollision(controller);

        //If the player is not dashing, we will move player horizontally
        if (!dashSettings.isDashing)
            physics.ApplyHorizontalMovement();

        physics.DoGroundCheck();

        //Resets our jump count when the player lands
        if (physics.isGrounded && physics.falling)
        {
            jumpSettings.jumpCount = jumpSettings.maxJumpCount;
        }
    }

    //Unsubscribe all our functions ondestroy
    private void OnDestroy()
    {
        GameManager.OnGameStart -= Init;
        GameManager.OnDifficultyChange -= ChangeDifficulty;

        input.dashInput.OnInputDown -= Dash;
        input.jumpInput.OnInputDown -= Jump;
        input.jumpInput.OnInput -= JumpHold;
        input.jumpInput.OnInputUp -= JumpRelease;
    }
    #endregion

    //Player's initialization
    private void Init()
    {
        physics.Init();
        jumpSettings.Init();
        controller.Init(GetComponent<BoxCollider2D>(), transform);

        hitBox.enabled = true;

        //Set the initial difficulty
        ChangeDifficulty();

        //subcribe our functions to input events
        input.dashInput.OnInputDown += Dash;
        input.jumpInput.OnInputDown += Jump;
        input.jumpInput.OnInput += JumpHold;
        input.jumpInput.OnInputUp += JumpRelease;
        GameManager.OnDifficultyChange += ChangeDifficulty;

        EnableInput();
        EnableMovement();
    }

    #region Jump Functions
    //When player pressed the jump input, subscribed to OnInputDown
    void Jump()
    {
        //If player is on ground or in air and still has jumpcount, we will jump
        if (physics.isGrounded || (physics.isGrounded == false && jumpSettings.jumpCount > 0))
        {
            //this is the jump text that appears when game starts to give player tip on jumping
            //We will set it inactive on the first time the player presses jump after the game starts
            if (GameManager.Instance.jumpText.activeInHierarchy)
            {
                GameManager.Instance.jumpText.SetActive(false);
            }

            StopDash();
            jumpSettings.isJumping = true;

            //Start our jump time counter, it's the duration for how long the player can hold onto jump to get a higher jump
            jumpSettings.jumpTimeCounter = jumpSettings.jumpTime;

            //Apply our jump velocity 
            physics.velocity.y = jumpSettings.jumpvelocity;

            //decrease jump count
            jumpSettings.jumpCount--;

            SoundManager.Instance.PlayAudioPlayer(SoundManager.Instance.jump);
        }
    }

    //When player holds the jump input, subscribed to OnInput
    void JumpHold()
    {
        if (jumpSettings.isJumping == true)
        {
            //if we still have jump hold time 
            if (jumpSettings.jumpTimeCounter > 0)
            {
                StopDash();

                //apply jump velocity to give it a bigger jump boost when holding onto jump
                physics.velocity.y = jumpSettings.jumpvelocity;
                jumpSettings.jumpTimeCounter -= Time.deltaTime;
                SoundManager.Instance.PlayAudioPlayer(SoundManager.Instance.jump);
            }
            else
            {
                jumpSettings.isJumping = false;
            }
        }
    }

    //When player releases jump input, subscribed to OnInputUp
    void JumpRelease()
    {
        jumpSettings.isJumping = false;
    }
    #endregion

    #region Dash Functions
    //When player pressed the dash input, subscribed to OnInputDown
    void Dash()
    {
        //check cooldown
        if (dashSettings.dashTimeStamp <= Time.time)
        {
            //Time stamping to simulate cooldown
            dashSettings.dashTimeStamp = Time.time + dashSettings.dashCooldown; 

            //Shake the camera horizontally a bit to simulate a nudge when dashing
            PlayerCamera.Instance.ShakeCamera(.05f, .15f, new Vector2(1, .3f));

            //Start our dash coroutine
            StartCoroutine("DashCo");
            SoundManager.Instance.PlayAudioPlayer(SoundManager.Instance.dash);

            //this is the dash text that appears when game starts to give player tip on dashing
            //We will set it inactive on the first time the player presses dash after the game starts
            if (GameManager.Instance.dashText.activeInHierarchy)
            {
                GameManager.Instance.dashText.SetActive(false);
            }
        }
    }

    IEnumerator DashCo()
    {
        //initialize our current dash duration 
        dashSettings.currentDashDuration = dashSettings.dashDuration;

        //show dash trail
        particleTrail.Play();

        //spawn dash fx
        Instantiate(dashSettings.dashFx, dashSettings.fxSpawnPos.position, Quaternion.identity);

        //while our dash duration is not over
        while (dashSettings.currentDashDuration > 0)
        {
            dashSettings.isDashing = true;
            dashSettings.currentDashDuration -= Time.deltaTime;

            //Set our gravity to 0 so player stays afloat while dashing
            physics.gravity = 0;

            //set velocity to the sum of horizontalSpeed and dashSpeed
            physics.velocity = new Vector3(physics.horizontalSpeed + dashSettings.dashSpeed, 0);
            yield return null;
        }

        //Stop dash trail
        particleTrail.Stop();
        dashSettings.isDashing = false;

        //Set our gravity back to normal
        physics.gravity = -9.81f;
    }

    //Called by the jump functions 
    void StopDash()
    {
        //stop our dash coroutine
        StopCoroutine("DashCo");

        //stop dash trail
        particleTrail.Stop();
        dashSettings.isDashing = false;

        //set gravity back to normal
        physics.gravity = -9.81f;
    }
    #endregion

    //Difficulty changes the horizontal speed of the player
    void ChangeDifficulty()
    {
        //calculate the xtra speed we will add onto our current horizontal speed
        float extraSpeed = Mathf.Clamp(GameManager.Instance.difficulty, 1, 14) / physics.speedIncreaseFold * .1f;

        //Add the speeds
        physics.horizontalSpeed = physics.baseSpeed + extraSpeed;
    }

    //Set our player's animations based on certain variables
    void SetAnims()
    {
        if (physics.velocity.y > 0)
        {
            physics.falling = false;
        }

        if (physics.velocity.y < 0 && physics.isGrounded == false)
        {
            physics.falling = true;
        }

        anim.SetBool("grounded", physics.isGrounded);
        anim.SetBool("falling", physics.falling);
        anim.SetBool("dashing", dashSettings.isDashing);
    }

    public void DisableMovement()
    {
        movementDiabled = true;
        physics.velocity = Vector2.zero;
        anim.speed = 0;
    }

    public void EnableMovement()
    {
        movementDiabled = false;
        anim.speed = 1;
    }

    public void DisableInput()
    {
        input.disabled = true;
    }

    public void EnableInput()
    {
        input.disabled = false;
    }

    /// <summary>
    /// Disables hitbox and lowers the alpha of player, called when player revives
    /// </summary>
    /// <param name="duration">duration of hitbox disabled</param>
    /// <param name="changeAlpha">the alpha of the sprite to change into</param>
    public void DisableHitBox(float duration, bool changeAlpha)
    {
        StopAllCoroutines();
        StartCoroutine(DisableHitBoxCo(duration, changeAlpha));
    }

    IEnumerator DisableHitBoxCo(float duration, bool changeAlpha)
    {
        hitBox.enabled = false;
        Color color = deerSprite.color;
        if (changeAlpha)
        {
            color.a = .5f;
            deerSprite.color = color;
        }

        while (duration > 0.01)
        {
            duration -= Time.deltaTime;
            yield return null;
        }

        hitBox.enabled = true;

        if (changeAlpha)
        {
            color.a = 1f;
            deerSprite.color = color;
        }
    }

    public Vector2 GetVelocity()
    {
        return physics.velocity;       
    }
}
