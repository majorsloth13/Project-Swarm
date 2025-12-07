using System.Collections;
using UnityEngine;

public class MovementSlashState : IPlayerState, IPlayerPhysicsState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;
    private float dashSpeed = 50f;
    private float dashTime = 0.2f;
    private float timer = 0f;
    private bool isDashing = false;
    public int maxDashCharges = 2;
    public int currentDashCharges = 2;
    public float dashRechargeTime = 3f;
    private float dashRechargeTimer = 0f;

    public MovementSlashState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
    }

    public void Enter()
    {
        
         // float during dash
        Debug.Log("movementslash enetered");
        
        if (machine.hasActivated)
        {
             
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
                    machine.StartCoroutine(Dash());
                }
                // Calculate real mouse velocity
            }

        }
    }

    public void Update()
    {
        timer += Time.deltaTime;

        // Player can still move horizontally while dashing
        //float xInput = Input.GetAxisRaw("Horizontal");
        //machine.FlipToGunDirection();
        //rb.linearVelocity = new Vector2(xInput * dashSpeed, 0f);


        if (!isDashing)
        {
            machine.SwitchState(new IdleState(machine));
            return;
        }




        // End dash

    }

    public void FixedUpdate() { }

    public void Exit()
    {
         // restore gravity
        Debug.Log($"Exit {rb.gravityScale}");
       
    }

    IEnumerator Dash()
    {
        isDashing = true;

        // Get world position of mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = machine.transform.position.z;

        // Direction toward click
        Vector3 direction = (mousePos - machine.transform.position).normalized;

        float timer = 0f;
        while (timer < dashTime)
        {
            rb.gravityScale = 0f;
            machine.transform.position += direction * dashSpeed * Time.deltaTime;
            //playerAttack.Attack();
            
            timer += Time.deltaTime;
            
            yield return null;
            
        }
        rb.gravityScale = 1f;
        //while (floatTime < 1f)
        //{
        //    rb.gravityScale = 0f;
        //    floatTime += Time.deltaTime;

        //}

        //    floatTime = 0f;
        //    rb.gravityScale = gravity;




        isDashing = false;
        
        Debug.Log("enetered idel fromdash");
    }
}
