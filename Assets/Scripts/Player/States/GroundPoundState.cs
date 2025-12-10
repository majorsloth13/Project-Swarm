using UnityEngine;

public class GroundPoundState : IPlayerState, IPlayerPhysicsState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;
    private float groundPoundSpeed = 30f;
    private bool hasLanded = false;

    // Ground pound damage settings
    private int damage = 20;
    private float knockbackForce = 10f;
    private float knockbackRadius = 2f;

    public GroundPoundState(PlayerStateMachine machine, float speed = 30f)
    {
        this.machine = machine;
        rb = machine.Rb;
        groundPoundSpeed = speed;
    }

    public void Enter()
    {
        rb.linearVelocity = new Vector2(0f, -groundPoundSpeed);
    }

    public void Update()
    {
        if (machine.IsGrounded && rb.linearVelocity.y <= 0f)
        {
            hasLanded = true;
            ApplyImpactDamage();
        }
    }

    public void FixedUpdate()
    {
        if (!hasLanded)
        {
            rb.linearVelocity = new Vector2(0f, -groundPoundSpeed);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            machine.SwitchState(new GroundedState(machine));
        }
    }

    private void ApplyImpactDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(machine.transform.position, knockbackRadius);
        foreach (Collider2D hit in hits)
        {
            //Enemy enemy = hit.GetComponent<Enemy>();
            //if (enemy != null)
            //{
            //    // Knockback direction is away from player
            //    Vector2 direction = (enemy.transform.position - machine.transform.position).normalized;
            //    enemy.TakeDamage(damage, direction * knockbackForce);
            //}
        }
    }

    public void Exit() { }
}
