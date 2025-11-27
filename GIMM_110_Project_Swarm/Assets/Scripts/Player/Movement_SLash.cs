using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Movement_Slash : MonoBehaviour
{

    public float dashSpeed = 40f;
    public float dashTime = 1f;
    private bool isDashing = false;
    public float gravity;
    public float fallMultiplier = 2.5f;
    public float floatTime = 0;
    public int maxDashCharges = 2;
    public int currentDashCharges = 2;
    public float dashRechargeTime = 3f;
    private float dashRechargeTimer = 0f;

    public PlayerAttack playerAttack;
    public SpriteRenderer playerSpriteRenderer;

    private bool aceOfScpades;


    [SerializeField] private float speed;
    private Rigidbody2D rb;
    SpriteRenderer sprite;
    float movement;

    public bool isGrounded;
    [SerializeField] float jumpForce = 18f;
    public bool moveable;
    [SerializeField] public float buffTimer;
    public GameObject gun;
    
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // Optional: prevent tipping over
        moveable = true;
        
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && buffTimer == 15)
        {
            aceOfScpades = true;
            gun.gameObject.SetActive(false);
            
        }
        else if (buffTimer <= 0f)
        {
            aceOfScpades = false;
            gun.gameObject.SetActive(true);
            
        }
        if (!aceOfScpades && buffTimer <= 0f)
        {
            buffTimer = 15f;
            Debug.Log("buff timer is 15");
        }

            if (aceOfScpades)
        {
            buffTimer -= Time.deltaTime;
            if (currentDashCharges < maxDashCharges)
            {
                dashRechargeTimer += Time.deltaTime;

                if (dashRechargeTimer >= dashRechargeTime)
                {
                    currentDashCharges++;
                    dashRechargeTimer = 0f;
                }
            }
            {

                if (Input.GetMouseButtonDown(1) && !isDashing && currentDashCharges > 0)
                {
                    currentDashCharges--;
                    StartCoroutine(Dash());
                }
                // Calculate real mouse velocity
            }

        }
        
        

        if (moveable)
        {

            movement = Input.GetAxis("Horizontal");
            UpdateSpriteDirection();
            rb.linearVelocity = new Vector2(movement * speed, rb.linearVelocity.y);

        }

        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
        
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }



    }

        
        

    

    IEnumerator Dash()
    {
        isDashing = true;

        // Get world position of mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;

        // Direction toward click
        Vector3 direction = (mousePos - transform.position).normalized;

        float timer = 0f;
        while (timer < dashTime)
        {
            rb.gravityScale = 0f;
            transform.position += direction * dashSpeed * Time.deltaTime;
            playerAttack.Attack();
            
            timer += Time.deltaTime;
            
            yield return null;
            
        }
        
        while (floatTime < 1f)
        {
            rb.gravityScale = 0f;
            floatTime += Time.deltaTime;
            
        }
        
            floatTime = 0f;
            rb.gravityScale = gravity;
            
        
        

        isDashing = false;
    }



    private void Jump()
    {



        // If the player is not grounded, return out of method 
        if (!isGrounded)
        {
            
            return;
        }
        else
        { rb.gravityScale = gravity; }

            // If the player is grounded and space is pressed, set the y velocity of the player to the jumpforce
            // Sets the y velocity of the player to the jumpforce. Preserves the x velocity.
            //Debug.Log("Player Jumped");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        Debug.Log("grounded");


    }

    private void UpdateSpriteDirection()
    {
        // Flips the sprite based on the direction the player is moving
        if (movement > 0f)
        {

            
            sprite.flipX = false;
            
        }
        else if (movement < 0f)
        {

            
            sprite.flipX = false;
        }
    }

}








