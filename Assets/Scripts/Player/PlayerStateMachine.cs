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

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip deathSoundClip;


    [Header("Power-Up")]
    public bool powerUpActive = true; // set true when player picks up power
    public float powerUpTimer;
    public bool hasActivated = false;
    public bool clubActivated = false;
    // adjustable cooldown
    public float gunCooldown = 0.25f;     
    [HideInInspector] public float gunCooldownTimer = 0f;
    public int maxDashCharges = 2;
    public int currentDashCharges = 2;
    public float dashRechargeTime = 3f;
    private float dashRechargeTimer = 0f;
    public BoxCollider2D slashHitbox;

    [Header("Power-Up Slots")]
    public ScannedCard[] powerUpSlots = new ScannedCard[2]
{
    ScannedCard.None,
    ScannedCard.None
};

    [Header("card tracking")]
    public ToggleOnTracking AcetoggleTracking;
    public ToggleOnTracking kingtoggleTracking;
    public ToggleOnTracking queentoggleTracking;
    public ToggleOnTracking jackleTracking;
    public ToggleOnTracking jokertoggleTracking;
   
    [Header("Diamond Skin")]
    public bool diamondSkinActive = false;
    public int diamondSkinMaxHealth = 6;
    public int diamondSkinCurrentHealth = 1;
    public GameObject diamondSkin;

    [Header("Drop Through Platform")]
    [SerializeField] private float dropDuration = 0.25f;
    private bool isDropping = false;
    //[SerializeField]private float dropForce = 10f;

    internal float coyoteTimer = 0f;
    internal float jumpBufferTimer = 0f;

    public LayerMask enemyLayer;

    // states
    private IPlayerState currentState;
    public IPlayerState previousState;

    // public flags
    public bool HasDoubleJump = true;   // rset when landing
    public bool isIdle = false;

    private GroundCheck groundCheck;

    public LayerMask dropMask;

    internal PlatformEffector2D currentEffector = null;
    internal Collider2D currentPlatformCollider = null;

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
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        SwitchState(new GroundedState(this)); // start inside grounded parent
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Current State: " + currentState?.GetType().Name);

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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActivatePowerUp(powerUpSlots[0]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActivatePowerUp(powerUpSlots[1]);
        }

        if (powerUpTimer <= 0f)
        {
            hasActivated = false;
            gun.gameObject.SetActive(true);
            //AceScanned = false;

        }

        if (clubActivated == true)
        {
            SwitchState(new GroundPoundState(this));
            clubActivated = false;
        }

        // Deactivate shield if health <= 0
        if (diamondSkinCurrentHealth <= 0)
        {
            Debug.Log("diamond skin broken");
            diamondSkinActive = false;
            diamondSkin.gameObject.SetActive(false);

        }



        // Power-up dash activation
        if (!hasActivated && powerUpTimer <= 0)
        {

            powerUpTimer = 15f;


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

            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(SlashCoroutine());
            }

            IEnumerator SlashCoroutine()
            {
                slashHitbox.enabled = true;
                yield return new WaitForSeconds(0.15f); // slash active time
                slashHitbox.enabled = false;
            }


        }

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
        gunTransform.rotation = Quaternion.Euler(0f, 0f, angle);

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
                Rb.gravityScale = 3f;
            }

            // Let the state apply physics ONLY here
            if (currentState is IPlayerPhysicsState physState)
            {
                physState.FixedUpdate();
            }

        }

    public IEnumerator DropRoutine()
    {
        if (currentPlatformCollider == null) yield break;

        // 1. CAPTURE the platform in a local variable
        Collider2D platformToFix = currentPlatformCollider;
        Collider2D playerCollider = GetComponent<Collider2D>();

        isDropping = true;

        // 2. Use the local variable to ignore
        Physics2D.IgnoreCollision(playerCollider, platformToFix, true);

        Rb.linearVelocity = new Vector2(Rb.linearVelocity.x, -5f);

        yield return new WaitForSeconds(dropDuration);

        // 3. Use the local variable to re-enable
        // This works even if machine.currentPlatformCollider was set to null!
        if (platformToFix != null)
        {
            Physics2D.IgnoreCollision(playerCollider, platformToFix, false);
        }

        isDropping = false;

        // Only clear these if they haven't been changed by a new platform
        if (currentPlatformCollider == platformToFix)
        {
            currentPlatformCollider = null;
            currentEffector = null;
        }

        // Only switch to fall if we are actually in the air
        if (!IsGrounded)
        {
            SwitchState(new FallState(this));
        }
    }

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

    public enum ScannedCard
    {
        None,
        Ace,
        King,
        Queen,
        Jack,
        Joker
    }

    public ScannedCard scannedCard = ScannedCard.None;

    public void AssignScannedCardToSlot(ScannedCard card)
    {
        // Ignore duplicates
        if (powerUpSlots[0] == card || powerUpSlots[1] == card)
            return;

        if (powerUpSlots[0] == ScannedCard.None)
        {
            powerUpSlots[0] = card;
            Debug.Log(card + " assigned to Slot 1");
        }
        else if (powerUpSlots[1] == ScannedCard.None)
        {
            powerUpSlots[1] = card;
            Debug.Log(card + " assigned to Slot 2");
        }
    }

    void ActivatePowerUp(ScannedCard card)
    {
        switch (card)
        {
            case ScannedCard.Ace:
                ActivateAce();
                break;

            case ScannedCard.King:
                ActivateKing();
                break;

            case ScannedCard.Queen:
                ActivateQueen();
                break;

            case ScannedCard.Jack:
                ActivateJack();
                break;
            
            case ScannedCard.Joker:
                ActivateJoker();
                break;
        }
    }

    void ActivateAce()
    {
        Debug.Log("Ace power activated");
        hasActivated = true;
        gun.gameObject.SetActive(false);
        ConsumePowerUp(ScannedCard.Ace);
    }

    void ActivateKing()
    {
        Debug.Log("King power activated");
        diamondSkinCurrentHealth = diamondSkinMaxHealth;
        diamondSkinActive = true;
        diamondSkin.SetActive(true);
        ConsumePowerUp(ScannedCard.King);
    }

    void ActivateQueen()
    {

    }

    void ActivateJack()
    {
        

        Debug.Log("Jack power activated (one-time)");

        clubActivated = true;

        ConsumePowerUp(ScannedCard.Jack);
    }


    void ActivateJoker()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState is IPlayerPhysicsState physState)
        {
            physState.OnCollisionEnter2D(collision);
        }
    }

    // Update your existing OnCollisionEnter2D or add these:
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Check if what we are touching is on the "Drop" layer
        // (Ensure your platforms are on the layer you assigned to dropMask)
        if (((1 << collision.gameObject.layer) & dropMask) != 0)
        {
            currentPlatformCollider = collision.collider;
            currentEffector = collision.gameObject.GetComponent<PlatformEffector2D>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Clear the references when we leave the platform
        if (collision.collider == currentPlatformCollider)
        {
            currentPlatformCollider = null;
            currentEffector = null;
        }
    }

    void ConsumePowerUp(ScannedCard card)
    {
        if (powerUpSlots[0] == card)
            powerUpSlots[0] = ScannedCard.None;
        else if (powerUpSlots[1] == card)
            powerUpSlots[1] = ScannedCard.None;
    }

}

