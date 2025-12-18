using UnityEngine;

public class GroundPoundState : IPlayerState, IPlayerPhysicsState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;
    private Animator anim;

    private float startHeight;
    private bool isGroundPounding;

    public float slamForce = 25f;
    public float radius = 2.5f;
    public float minDamage = 1f;
    //public float damageMultiplier = 2f;
    public float knockbackForce = 10f;
    

    public GroundPoundState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
        anim = machine.GetComponent<Animator>();
    }

    public void Enter()
    {
        isGroundPounding = true;
        startHeight = machine.transform.position.y;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.down * slamForce, ForceMode2D.Impulse);

        Debug.Log("Ground pound ENTER");
    }

    public void Update()
    {

    }
    public void FixedUpdate()
    {

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isGroundPounding) return;

        if (collision.collider.CompareTag("Ground"))
        {
            Debug.Log("Ground pound IMPACT");
            isGroundPounding = false;
            Impact();
            machine.SwitchState(new IdleState(machine));
        }
    }

    private void Impact()
    {
        float fallDistance = startHeight - machine.transform.position.y;
        float damage = minDamage + fallDistance;/* * damageMultiplier;*/

        Collider2D[] enemies =
            Physics2D.OverlapCircleAll(machine.transform.position, radius, machine.enemyLayer);

        Debug.Log("Enemies hit: " + enemies.Length);

        foreach (Collider2D enemy in enemies)
        {
            enemy.GetComponent<EnemyHealth>()?.TakeDamage(damage);

            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb)
            {
                Vector2 dir =
                    (enemy.transform.position - machine.transform.position).normalized;
                enemyRb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
            }
        }

        
    }

    public void Exit()
    {
        Debug.Log("Ground pound EXIT");
        machine.clubActivated = false;
        if(machine.clubActivated == false)
        {
            Debug.Log("club activated false");
            anim.SetBool("isWalking", true);
        }
                

        
        
    }
}






















