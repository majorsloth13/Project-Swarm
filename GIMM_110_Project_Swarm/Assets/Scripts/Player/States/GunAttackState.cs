using UnityEngine;

public class GunAttackState : IPlayerState
{
    private PlayerStateMachine ctx;

    private float cooldown;
    private GameObject bulletPrefab;
    private Transform firePoint;
    private Transform gunTransform;
    private float bulletLifetime;

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
        FireBullet();
        ctx.gunCooldownTimer = cooldown;
    }

    public void Update()
    {
        /*Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorld - gunTransform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gunTransform.rotation = Quaternion.Euler(0f, 0f, angle);*/
    

        if (ctx.previousState != null)
        {
            ctx.SwitchState(ctx.previousState);
        }
        else
        {
            Debug.LogWarning("GunAttackState: previousState is null, cannot switch back!");
        }
    }


    public void Exit()
    {
       
    }

    private void FireBullet()
    {
        GameObject b = Object.Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
        rb.linearVelocity = firePoint.right * 20f; // adjustable later 

        Object.Destroy(b, bulletLifetime);
    }
}
