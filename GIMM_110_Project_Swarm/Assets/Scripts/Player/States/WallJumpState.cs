using UnityEngine;

public class WallJumpState : IPlayerState, IPlayerPhysicsState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;

    private float jumpVerticalForce = 10f;
    private float jumpHorizontalForce = 6f;
    private float duration = 0.18f;
    private float timer;
    private Vector2 initialVelocity;

    public WallJumpState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
    }

    public void Enter()
    {
        timer = 0f;

        // Jump away from the wall
        float dir = machine.IsTouchingLeftWall ? 1f : -1f;
        initialVelocity = new Vector2(dir * jumpHorizontalForce, jumpVerticalForce);
        rb.linearVelocity = initialVelocity;
    }

    public void Update()
    {
        timer += Time.deltaTime;

        machine.FlipToGunDirection();

        if (timer >= duration)
        {
            machine.SwitchState(new FallState(machine));
        }
    }

    public void FixedUpdate()
    {
        float xInput = Input.GetAxisRaw("Horizontal");

        // Combine wall jump push with horizontal input for smooth control
        float horizontalVelocity = initialVelocity.x + xInput * machine.airMoveSpeed;
        rb.linearVelocity = new Vector2(horizontalVelocity, rb.linearVelocity.y);
    }

    public void Exit() { }
}
