using UnityEngine;

public class GunAttackState : IPlayerState
{
    private PlayerStateMachine ctx;

    private float cooldown;
    private GameObject bulletPrefab;
    private Transform firePoint;
    private Transform gunTransform;
    private float bulletLifetime;

    private bool fired = false;

    public GunAttackState(
        PlayerStateMachine ctx,
        float cooldown,
        GameObject bulletPrefab,
        Transform firePoint,
        Transform gunTransform,
        float bulletLifetime)
    {
        this.ctx = ctx;
        this.cooldown = cooldown;
        this.bulletPrefab = bulletPrefab;
        this.firePoint = firePoint;
        this.gunTransform = gunTransform;
        this.bulletLifetime = bulletLifetime;
    }

    public void Enter()
    {
        fired = false;
    }

    public void Update()
    {
        ctx.FlipToGunDirection();

        // fire ONCE per Enter(), NEVER in Update()
        if (!fired)
        {
            FireBullet();
            ctx.gunCooldownTimer = cooldown;
            fired = true;

            // switch states ONLY AFTER firing
            if (ctx.previousState != null)
            {
                ctx.SwitchState(ctx.previousState);
                return; // important!
            }
        }
    }

    public void Exit() { }

    private void FireBullet()
    {
        GameObject b = Object.Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.linearVelocity = firePoint.right * 20f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;


        Object.Destroy(b, bulletLifetime);
    }
}
