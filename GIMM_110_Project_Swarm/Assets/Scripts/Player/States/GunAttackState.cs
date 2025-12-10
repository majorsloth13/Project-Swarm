using UnityEngine;

public class GunAttackState : IPlayerState
{
    private PlayerStateMachine ctx;
    private float cooldown;
    private Transform firePoint;
    private Transform gunTransform;
    private float bulletLifetime;
    private int bulletDamage;
    private float bulletSpeed;

    private bool fired = false;

    public GunAttackState(
        PlayerStateMachine ctx,
        float cooldown,
        Transform firePoint,
        Transform gunTransform,
        float bulletLifetime,
        int bulletDamage = 10,
        float bulletSpeed = 20f)
    {
        this.ctx = ctx;
        this.cooldown = cooldown;
        this.firePoint = firePoint;
        this.gunTransform = gunTransform;
        this.bulletLifetime = bulletLifetime;
        this.bulletDamage = bulletDamage;
        this.bulletSpeed = bulletSpeed;
    }

    public void Enter()
    {
        fired = false;
    }

    public void Update()
    {
        ctx.FlipToGunDirection();

        if (!fired)
        {
            FireBullet();
            ctx.gunCooldownTimer = cooldown;
            fired = true;

            if (ctx.previousState != null)
            {
                ctx.SwitchState(ctx.previousState);
                return;
            }
        }
    }

    public void Exit() { }

    private void FireBullet()
    {
        GameObject bulletGO = new GameObject("Bullet");
        bulletGO.transform.position = firePoint.position;

        Bullet bullet = bulletGO.AddComponent<Bullet>();
        bullet.Initialize(
            direction: firePoint.right,
            damage: bulletDamage,
            speed: bulletSpeed,
            lifetime: bulletLifetime
        );
    }
}
