using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [Header("Movement Speeds (units/second)")]
    public float verticalSpeed = 10f;
    public float horizontalSpeed = 10f;

    [Header("Jump")]
    public float jumpForce = 12f;
    public float airMoveSpeed = 8f;

    [Header("Movement Step Rate")]
    [Tooltip("How many movement steps per second (12 ≈ slow, visible stepping).")]
    public float movementFps = 12f;
    public float StepInterval => 1f / movementFps;

    [Header("Environment Checks")]
    public GroundCheck GroundCheck;
    public WallCheck WallCheck;

    public Rigidbody2D Rb;

    [Header("Coyote & Buffer")]
    [Tooltip("How long after leaving ground the player can still jump.")]
    public float coyoteTime = 0.12f;
    [Tooltip("How long a jump press is buffered before landing.")]
    public float jumpBufferTime = 0.12f;

    [Header("Gun Attack")]
    public GameObject bulletPrefab;
    public Transform gunFirePoint;
    public Transform gunTransform;
    //public float gunFireRate = 0.25f;
    public float bulletLifetime = 2f;

    [Header("Power-Up")]
    public bool powerUpActive = false; // set true when player picks up power


    // adjustable cooldown
    public float gunCooldown = 0.25f;     
   [HideInInspector] public float gunCooldownTimer = 0f;

    // runtime timers
    internal float coyoteTimer = 0f;
    internal float jumpBufferTimer = 0f;

    // states
    private IPlayerState currentState;
    public IPlayerState previousState;

    // public flags
    public bool HasDoubleJump = true;   // rset when landing

    // Helpers (exposed for states)
    public bool IsGrounded => GroundCheck != null && GroundCheck.IsGrounded();
    public bool IsTouchingWall => WallCheck != null && WallCheck.IsTouchingWall;
    public bool IsTouchingLeftWall => WallCheck != null && WallCheck.IsTouchingLeftWall;
    public bool IsTouchingRightWall => WallCheck != null && WallCheck.IsTouchingRightWall;

    //public IPlayerState FallState;
    //public IPlayerState WallSlideState;
    //public IPlayerState DoubleJumpState;
    //public IPlayerState GroundedState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Rb == null) Rb = GetComponent<Rigidbody2D>();
        SwitchState(new GroundedState(this)); // start inside grounded parent
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsGrounded)
        {
            coyoteTimer -= Time.deltaTime;
        }
        else
        {
            coyoteTimer = coyoteTime; // rest whenever grounded
        }

        if (jumpBufferTimer > 0f)
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        // capture jump input (buffer)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferTime;
        }

        // cooldown countdown
        if (gunCooldownTimer > 0f)
        {
            gunCooldownTimer -= Time.deltaTime;
        }

        // Power-up dash activation
        if (powerUpActive && Input.GetKeyDown(KeyCode.LeftShift))
        {
            SwitchState(new MovementSlashState(this));
            return; // Important: stop further Update() in this frame
        }

        if (Input.GetMouseButtonDown(0) && gunCooldownTimer <= 0f)
        {
            SwitchState(new GunAttackState(
                this,
                gunCooldown,
                gunFirePoint,
                gunTransform,
                bulletLifetime
            ));
            return;
        }


        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorld - gunTransform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gunTransform.rotation = Quaternion.Euler(0f, 0f, angle);

        currentState?.Update();
    }

    void FixedUpdate()
    {
        // Let the state apply physics ONLY here
        if (currentState is IPlayerPhysicsState physState)
        {
            physState.FixedUpdate();
        }
    }


    /*private void HandleInput()
    {
        // 1 = Idle, 2 = Vertical, 3 = Horizontal
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchState(new IdleState(this));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchState(new VerticalState(this));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchState(new HorizontalState(this));
        }
    }*/

    public void SwitchState(IPlayerState newState)
    {
        if (newState == null)
        {
            Debug.LogWarning("SwitchState called with null!");
            return;
        }

        currentState?.Exit();
        previousState = currentState; // << assign previous
        currentState = newState;
        currentState.Enter();
    }


    // Called by states to check & consume buffered jump
    public bool TryConsumeJumpBuffer()
    {
        if (jumpBufferTimer > 0f)
        {
            jumpBufferTimer = 0f;
            return true;
        }
        return false;
    }

    // Called by states to check if coyote time still allows jump
    public bool IsCoyoteAvailable()
    {
        return coyoteTimer > 0f;
    }
    public void FlipToGunDirection()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mouseWorld.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }



    //Accessors used by state objects (encapulates internal details)
    public Transform GetTransform() => transform;
    public float GetVerticalSpeed() => verticalSpeed;
    public float GetHorizontalSpeed() => horizontalSpeed;
    //public Rigidbody2D GetRb() => GetComponent<Rigidbody2D>();
    public Rigidbody2D GetRb() => Rb;
    
}