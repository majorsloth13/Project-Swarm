using UnityEngine;

public class MovementSlashState : IPlayerState, IPlayerPhysicsState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;
    private float dashSpeed = 40f;
    private float dashTime = 0.5f;
    private float timer = 0f;

    public MovementSlashState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
    }

    public void Enter()
    {
        timer = 0f;
        rb.gravityScale = 0f; // float during dash
    }

    public void Update()
    {
        timer += Time.deltaTime;

        // Player can still move horizontally while dashing
        float xInput = Input.GetAxisRaw("Horizontal");
        machine.FlipToGunDirection();
        rb.linearVelocity = new Vector2(xInput * dashSpeed, 0f);

        // Fire bullets during dash if desired
        if (Input.GetMouseButtonDown(0) && machine.gunCooldownTimer <= 0f)
        {
            GameObject b = Object.Instantiate(machine.bulletPrefab, machine.gunFirePoint.position, machine.gunTransform.rotation);
            Rigidbody2D rbBullet = b.GetComponent<Rigidbody2D>();
            rbBullet.bodyType = RigidbodyType2D.Kinematic;
            rbBullet.linearVelocity = machine.gunTransform.right * 20f;
            Object.Destroy(b, machine.bulletLifetime);
            machine.gunCooldownTimer = machine.gunCooldown;
        }

        // End dash
        if (timer >= dashTime)
        {
            machine.SwitchState(machine.previousState);
        }
    }

    public void FixedUpdate() { }

    public void Exit()
    {
        rb.gravityScale = 1f; // restore gravity
    }
}
