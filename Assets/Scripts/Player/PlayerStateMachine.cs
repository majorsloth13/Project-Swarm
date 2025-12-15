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
    public GameObject gun;

    [Header("Power-Up")]
    public bool powerUpActive = true; // set true when player picks up power
    public float powerUpTimer = 3f;
    public bool hasActivated = false;
    // adjustable cooldown
    public float gunCooldown = 0.25f;     
   [HideInInspector] public float gunCooldownTimer = 0f;
    public int maxDashCharges = 2;
    public int currentDashCharges = 2;
    public float dashRechargeTime = 3f;
    private float dashRechargeTimer = 0f;

    [Header("Drop Through Platform")]
    [SerializeField] private float dropDuration = 0.25f;
    private bool isDropping = false;
    //[SerializeField]private float dropForce = 10f;

    [Header("Audio")]
    [Tooltip("The sound effect plays when player dies")]
    public AudioClip DeathSoundClip;

    // runtime timers
    internal float coyoteTimer = 0f;
    internal float jumpBufferTimer = 0f;

    // states
    private IPlayerState currentState;
    public IPlayerState previousState;

    // public flags
    public bool HasDoubleJump = true;   // rset when landing

    private GroundCheck groundCheck;

    public LayerMask dropMask;

    internal PlatformEffector2D currentEffector = null;
    internal Collider2D currentPlatformCollider = null;


    // Helpers (exposed for states)
    public bool IsGrounded => 
        !isDropping && GroundCheck != null && GroundCheck.IsGrounded();
    public bool IsTouchingWall => WallCheck != null && WallCheck.IsTouchingWall;
    public bool IsTouchingLeftWall => WallCheck != null && WallCheck.IsTouchingLeftWall;
    public bool IsTouchingRightWall => WallCheck != null && WallCheck.IsTouchingRightWall;

    //public IPlayerState FallState;
    //public IPlayerState WallSlideState;
    //public IPlayerState DoubleJumpState;
    //public IPlayerState GroundedState;

    private void Awake()
    {
        groundCheck = GetComponentInChildren<GroundCheck>();
        Rb = GetComponent<Rigidbody2D>();
    }

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
            if (Input.GetKeyDown(KeyCode.LeftShift) && powerUpTimer == 3)
            {
                hasActivated = true;
                gun.gameObject.SetActive(false);

            }
            else if (powerUpTimer <= 0f)
            {
                hasActivated = false;
                gun.gameObject.SetActive(true);
            
        }
            // Power-up dash activation
            if (!hasActivated && powerUpTimer <= 0)
            {

                powerUpTimer = 3f;


            }
        if (hasActivated)
        {
            powerUpTimer -= Time.deltaTime;
            if (currentDashCharges < maxDashCharges)
            {
                dashRechargeTimer += Time.deltaTime;

                if (dashRechargeTimer >= dashRechargeTime)
                {
                    currentDashCharges++;
                    dashRechargeTimer = 0f;
                }
                
            }
            if (Input.GetMouseButtonDown(1) && currentDashCharges > 0)
            {
                Debug.Log("slashed");
                currentDashCharges--;
                SwitchState(new MovementSlashState(this));
                
                return;
            }
         
        }
        



        //if (Input.GetMouseButtonDown(0) && gunCooldownTimer <= 0f)
        //{
        //    SwitchState(new GunAttackState(
        //        this,
        //        //gunFireRate,
        //        gunCooldown,
        //        bulletPrefab,
        //        gunFirePoint,
        //        gunTransform,
        //        bulletLifetime
        //    ));
        //    return;
        //}

        if (Input.GetMouseButtonDown(0) && gunCooldownTimer <= 0f && !hasActivated)
        {
            Debug.Log("fired");
            FireBullet(); // no state switch
            return;
        }

        FlipToGunDirection();

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mouseWorld - gunTransform.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Check if the player is flipped (facing left)
        if (transform.localScale.x < 0)
        {
            angle = 180f - angle;
            angle = -angle;
        }

        gunTransform.rotation = Quaternion.Euler(0f, 0f, angle);

            currentState?.Update();
        }

        // FixedUpdate method
        void FixedUpdate()
        {
           if (IsGrounded)
           {
                Rb.gravityScale = 1f;
           }

            // Let the state apply physics ONLY here
            if (currentState is IPlayerPhysicsState physState)
            {
                physState.FixedUpdate();
            }

        }

    public IEnumerator DropRoutine()
    {
        if (currentEffector == null || currentPlatformCollider == null)
        {
            yield break;
        }

        isDropping = true;

        Collider2D playerCollider = GetComponent<Collider2D>();

        Physics2D.IgnoreCollision(playerCollider, currentPlatformCollider, true);

        Rb.linearVelocity = new Vector2(Rb.linearVelocity.x, -5f);

        yield return new WaitForSeconds(dropDuration);

        Physics2D.IgnoreCollision(playerCollider, currentPlatformCollider, false);

        isDropping = false;
        currentEffector = null;
        currentPlatformCollider = null;

        SwitchState(new FallState(this));
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

        // Determine the player's new scale based on  mouse position
        float newPlayerScaleX = (mouseWorld.x > transform.position.x) ? 2f : -2f;

        // Check if the player is actually flipping this frame
        if (transform.localScale.x != newPlayerScaleX)
        {
            // Apply the flip to the player
            transform.localScale = new Vector3(newPlayerScaleX, 2f, 1f);

            // Counter flip the Gun's local scale to keep it upright
            if (gunTransform != null)
            {
                float gunLocalScaleX;

                if (newPlayerScaleX > 0)
                {
                    // Player faces right: Gun faces right (default sprite direction)
                    gunLocalScaleX = 1f;
                }
                else
                {

                    gunLocalScaleX = 1f;
                }


                gunLocalScaleX = 1f;

                gunTransform.localScale = new Vector3(
                    gunLocalScaleX,
                    gunTransform.localScale.y,
                    gunTransform.localScale.z
                );

            }
        }

        /* if (mouseWorld.x > transform.position.x)
             transform.localScale = new Vector3(2, 2, 1);
         else
             transform.localScale = new Vector3(-2, 2, 1);*/
    }



    //Accessors used by state objects (encapulates internal details)
    public Transform GetTransform() => transform;
    public float GetVerticalSpeed() => verticalSpeed;
    public float GetHorizontalSpeed() => horizontalSpeed;
    //public Rigidbody2D GetRb() => GetComponent<Rigidbody2D>();
    public Rigidbody2D GetRb() => Rb;

    private void FireBullet()
    {
        GameObject b = Object.Instantiate(bulletPrefab, gunFirePoint.position, gunFirePoint.rotation);

        Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.linearVelocity = gunFirePoint.right * 20f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;


        Object.Destroy(b, bulletLifetime);
    }

}