using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

[RequireComponent(typeof(Controller2D))]
public class PlayerMovement : MonoBehaviour {

    public static PlayerMovement singleton;
    InputDevice inputDevice;
    bool movementDiabled;

    [Header("[Physics Settings]")]
    public float gravity = -9.81f;
    public float horizontalSpeed;
    public int speedIncreaseFold;
    float baseSpeed;

    Vector3 velocity;
    Controller2D controller;
    bool falling;
    bool inputDisabled;

    [Space(10)]
    [Header("[Jump Settings]")]
    public float jumpVelocity = 2.25f;
    public float fallMultiplier;
    public float lowJumpMultiplier;
    public float jumpTime;
    public int maxJumpCount;
    int jumpCount;
    float jumpTimeCounter;
    bool isJumping;

    [Space(10)]
    [Header("[Dash Settings]")]
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldown;
    private float dashTimeStamp;
    private float currentDashDuration;
    private bool isDashing;
    public GameObject dashCircle;
    public Transform circleSpawnPos;

    [Space(10)]
    [Header("[Ground Check Settings]")]
    public Transform GroundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius;
    bool isGrounded;

    [Space(10)]
    [Header("[FX Settings]")]
    public Animator anim;
    public SpriteRenderer spriteRenderer;
    public ParticleSystem particleTrail;

    void Awake()
    {
        singleton = this;
        controller = GetComponent<Controller2D>();
        baseSpeed = horizontalSpeed;
    }

    void Start()
    {
        DisableMovement();
        GameManager.OnDifficultyChange += DifficultyChange;
        GameManager.OnGameStart += Initialize;
        jumpCount = maxJumpCount;
        DifficultyChange(GameManager.singleton.difficulty);
        DisableInput();
    }

    private void Initialize()
    {
        EnableInput();
        EnableMovement();
    }

    void Update()
    {
        if (movementDiabled)
        {
            velocity = Vector2.zero;
            return;
        }

        inputDevice = InputManager.ActiveDevice;

        if (inputDevice != InputDevice.Null && inputDevice != TouchManager.Device)
        {
            TouchManager.ControlsEnabled = false;
        }

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        HorizontalMovement();
        CheckGround();
        JumpInput();
        DashInput();


        SetAnims();
    }

    void FixedUpdate()
    {
        if (movementDiabled)
            return; 

        velocity.y += gravity * Time.fixedDeltaTime;
        if (velocity.y < 0)
        {
            velocity += Vector3.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            velocity += Vector3.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
        controller.Move(velocity * Time.fixedDeltaTime);
    }

    void LateUpdate()
    {
        if (isGrounded && falling)
        {
            jumpCount = maxJumpCount;
        }
    }

    void DifficultyChange(int value)
    {
        float extraSpeed = Mathf.Clamp(value, 1, 14) / speedIncreaseFold * .1f;
        horizontalSpeed = baseSpeed + extraSpeed;
    }

    void HorizontalMovement()
    {
        if (isDashing)
            return;
        velocity.x = horizontalSpeed;
    }

    void DashInput()
    {
        if (inputDisabled)
            return;

        if (dashTimeStamp <= Time.time && (Input.GetKeyDown(KeyCode.D) || inputDevice.Action2.WasPressed))
        {
            dashTimeStamp = Time.time + dashCooldown;
            CameraMovement.singleton.ShakeCamera(.05f, .15f, new Vector2(1, .3f));
            StartCoroutine("Dash");
            SoundManager.singleton.PlayAudioPlayer(SoundManager.singleton.dash);

            if (GameManager.singleton.dashText.activeInHierarchy)
            {
                GameManager.singleton.dashText.SetActive(false);
            }
        }
    }

    IEnumerator Dash()
    {
        currentDashDuration = dashDuration;
        particleTrail.Play();
        Instantiate(dashCircle, circleSpawnPos.position, Quaternion.identity);
        while (currentDashDuration > 0)
        {
            isDashing = true;
            currentDashDuration -= Time.deltaTime;
            gravity = 0;
            velocity = new Vector3(horizontalSpeed + dashSpeed, 0);
            yield return null;
        }
        particleTrail.Stop();
        isDashing = false;
        gravity = -9.81f;
    }

    void StopDash()
    {
        StopCoroutine("Dash");
        particleTrail.Stop();
        isDashing = false;
        gravity = -9.81f;
    }

    void JumpInput()
    {
        if (inputDisabled)
            return;

        if ((Input.GetKeyDown(KeyCode.Space) || inputDevice.Action1.WasPressed) && isGrounded == false && jumpCount > 0)
        {
            StopDash();
            isJumping = true;
            jumpTimeCounter = jumpTime;
            velocity.y = jumpVelocity;
            jumpCount--;
            SoundManager.singleton.PlayAudioPlayer(SoundManager.singleton.jump);
        }

        if ((Input.GetKeyDown(KeyCode.Space) || inputDevice.Action1.WasPressed) && isGrounded)
        {

            if (GameManager.singleton.jumpText.activeInHierarchy)
            {
                GameManager.singleton.jumpText.SetActive(false);
            }
            StopDash();
            isJumping = true;
            jumpTimeCounter = jumpTime;
            velocity.y = jumpVelocity;
            jumpCount--;
            SoundManager.singleton.PlayAudioPlayer(SoundManager.singleton.jump);
        }

        if ((Input.GetKey(KeyCode.Space) || inputDevice.Action1.HasInput) && isJumping == true)
        {
            if (jumpTimeCounter > 0)
            {
                StopDash();
                velocity.y = jumpVelocity;
                jumpTimeCounter -= Time.deltaTime;
                SoundManager.singleton.PlayAudioPlayer(SoundManager.singleton.jump);
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) || inputDevice.Action1.WasReleased)
        {
            isJumping = false;
        }
       
        
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, groundCheckRadius, groundLayer);
    }

    void SetAnims()
    {
        if (velocity.y > 0)
        {
            falling = false;
        }

        if (velocity.y < 0 && isGrounded == false)
        {
            falling = true;
        }

        anim.SetBool("grounded", isGrounded);
        anim.SetBool("falling", falling);
        anim.SetBool("dashing", isDashing);
    }

    public void DisableMovement()
    {
        movementDiabled = true;
        velocity = Vector2.zero;
        anim.speed = 0;
    }

    public void EnableMovement()
    {
        movementDiabled = false;
        anim.speed = 1;
    }

    public void DisableInput()
    {
        inputDisabled = true;
    }

    public void EnableInput()
    {
        inputDisabled = false;
    }

    public Vector2 GetVelocity()
    {
        return velocity;
        
    }

    private void OnDestroy()
    {
        GameManager.OnGameStart -= Initialize;
        GameManager.OnDifficultyChange -= DifficultyChange;
    }
}
