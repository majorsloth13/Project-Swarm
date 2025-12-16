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
    public float powerUpTimer;
    public bool hasActivated = false;
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
            transform.localScale = new Vector3(2, 2, 1);
        else
            transform.localScale = new Vector3(-2, 2, 1);
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
    }

    void ActivateKing()
    {
        Debug.Log("King power activated");
        diamondSkinCurrentHealth = diamondSkinMaxHealth;
        diamondSkinActive = true;
        diamondSkin.SetActive(true);
    }

    void ActivateQueen()
    {

    }

    void ActivateJack()
    {

    }

    void ActivateJoker()
    {

    }

}

