using UnityEngine;

public class WallJumpState : IPlayerState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;

    private float jumpVerticalForce = 10f;
    private float jumpHorizontalForce = 6f;
    private float duration = 0.18f;
    private float timer;
    private Vector2 initialVelocity;

    public WallJumpState(PlayerStateMachine machine, float verticalForce = 10f, float horizontalForce = 6f, float duration = 0.18f)
    {
        this.machine = machine;
        rb = machine.Rb;
        this.jumpVerticalForce = verticalForce;
        this.jumpHorizontalForce = horizontalForce;
        this.duration = duration;
    }

    public void Enter()
    {
        timer = 0f;

        // Choose direction away from the wall
        float dir = machine.IsTouchingLeftWall ? 1f : -1f;
        // Set immediate velocity away and up
        initialVelocity = new Vector2(dir * jumpHorizontalForce, jumpVerticalForce);
        rb.linearVelocity = initialVelocity;
    }

    public void Update()
    {
        timer += Time.deltaTime;
        // Give player control during the short wall-jump window
        float input = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(input * machine.airMoveSpeed, rb.linearVelocity.y);

        if (timer >= duration)
        {
            // After wall jump we go into fall (or jump depending on velocity)
            machine.SwitchState(new FallState(machine));
            return;
        }
    }

    public void Exit() { }
}
