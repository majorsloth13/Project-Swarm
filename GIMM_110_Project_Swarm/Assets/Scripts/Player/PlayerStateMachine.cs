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

    // runtime timers
    internal float coyoteTimer = 0f;
    internal float jumpBufferTimer = 0f;

    // states
    private IPlayerState currentState;

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

        currentState?.Update();
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
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
        /*if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter();*/
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

    //Accessors used by state objects (encapulates internal details)
    public Transform GetTransform() => transform;
    public float GetVerticalSpeed() => verticalSpeed;
    public float GetHorizontalSpeed() => horizontalSpeed;
    //public Rigidbody2D GetRb() => GetComponent<Rigidbody2D>();
    public Rigidbody2D GetRb() => Rb;
    
}