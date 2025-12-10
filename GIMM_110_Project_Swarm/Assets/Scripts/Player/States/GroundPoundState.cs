using UnityEngine;
using System.Collections;

public class GroundPoundState : IPlayerState, IPlayerPhysicsState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;

    private bool hasLanded = false;

    // Damage settings
    private int damage = 20;
    private float knockbackRadius = 2f;

    // Ground pound speed and collision
    private float groundPoundSpeed = 30f;
    private float collisionIgnoreDuration = 0.15f;

    private Collider2D playerCollider;

    public GroundPoundState(PlayerStateMachine machine, float speed = 30f)
    {
        this.machine = machine;
        rb = machine.Rb;
        groundPoundSpeed = speed;

        playerCollider = machine.GetComponent<Collider2D>();
    }

    public void Enter()
    {
        rb.linearVelocity = new Vector2(0f, -groundPoundSpeed);

        // Ignore collisions with all enemies while falling
        Collider2D[] enemies = Physics2D.OverlapCircleAll(machine.transform.position, 5f, LayerMask.GetMask("Enemy"));
        foreach (Collider2D enemy in enemies)
        {
            if (enemy != null)
                Physics2D.IgnoreCollision(playerCollider, enemy, true);
        }
    }

    public void Update()
    {
        DetectImpact();
    }

    public void FixedUpdate()
    {
        if (!hasLanded)
            rb.linearVelocity = new Vector2(0f, -groundPoundSpeed);
    }

    private void DetectImpact()
    {
        if (hasLanded) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            machine.transform.position,
            0.55f,
            LayerMask.GetMask("Ground", "Enemy")
        );

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            int layerMask = 1 << hit.gameObject.layer;

            if ((layerMask & LayerMask.GetMask("Ground")) != 0)
            {
                // Landed on ground
                hasLanded = true;
            }
            else if ((layerMask & LayerMask.GetMask("Enemy")) != 0)
            {
                // Hit enemy mid-air
                ApplyImpactEffects(new Collider2D[] { hit });
            }
        }

        if (hasLanded)
        {
            rb.linearVelocity = Vector2.zero;
            machine.StartCoroutine(SwitchToGroundedNextFrame());
        }
    }

    private IEnumerator SwitchToGroundedNextFrame()
    {
        yield return null;
        machine.SwitchState(new GroundedState(machine));
    }

    private void ApplyImpactEffects(Collider2D[] hits)
    {
        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            EnemyHealth hp = hit.GetComponent<EnemyHealth>();
            Rigidbody2D enemyRb = hit.attachedRigidbody;

            if (hp != null) hp.TakeDamage(damage);

            if (enemyRb != null)
            {
                Collider2D enemyCol = enemyRb.GetComponent<Collider2D>();

                // Temporarily ignore collision to prevent player sticking
                Physics2D.IgnoreCollision(playerCollider, enemyCol, true);

                // Move the player slightly upward to prevent overlap
                rb.position += Vector2.up * 0.1f;

                // Apply arc-based knockback
                bool fromAbove = machine.transform.position.y > enemyRb.position.y + 0.1f;
                float distance = fromAbove ? 2.5f : 5f;
                float height = fromAbove ? 2.5f : 1f;
                float duration = fromAbove ? 0.15f : 0.2f;

                machine.StartCoroutine(KnockbackEnemyArc(enemyRb, fromAbove, distance, height, duration));

                // Restore collision after short delay
                machine.StartCoroutine(RestoreCollision(playerCollider, enemyCol, collisionIgnoreDuration));
            }
        }

        // Stop player's downward velocity immediately
        rb.linearVelocity = Vector2.zero;
    }

    private IEnumerator KnockbackEnemyArc(Rigidbody2D enemyRb, bool fromAbove, float distance, float height, float duration)
    {
        if (enemyRb == null) yield break;

        Vector2 startPos = enemyRb.position;
        Vector2 dir = (enemyRb.position - (Vector2)machine.transform.position).normalized;

        float timer = 0f;
        float originalGravity = enemyRb.gravityScale;
        enemyRb.gravityScale = 0f;

        while (timer < duration)
        {
            if (enemyRb == null) yield break;

            timer += Time.deltaTime;
            float t = timer / duration;

            // Linear horizontal movement
            Vector2 pos = Vector2.Lerp(startPos, startPos + dir * distance, t);

            // Sinusoidal vertical arc
            pos.y += Mathf.Sin(t * Mathf.PI) * height;

            enemyRb.MovePosition(pos);
            yield return null;
        }

        enemyRb.gravityScale = originalGravity;
    }

    private IEnumerator RestoreCollision(Collider2D a, Collider2D b, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (a != null && b != null)
            Physics2D.IgnoreCollision(a, b, false);
    }

    public void Exit() { }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(machine.transform.position, knockbackRadius);
    }
}
