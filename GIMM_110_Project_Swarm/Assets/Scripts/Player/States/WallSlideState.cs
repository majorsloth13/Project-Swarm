using UnityEngine;

public class WallSlideState : IPlayerState, IPlayerPhysicsState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;
    private float slideSpeed = 4f; // Faster slide
    private bool hasWallJumped;

    public WallSlideState(PlayerStateMachine machine, float slideSpeed = 4f)
    {
        this.machine = machine;
        rb = machine.Rb;
        this.slideSpeed = slideSpeed;
    }

    public void Enter()
    {
        hasWallJumped = false;
    }

    public void Update()
    {
        machine.FlipToGunDirection();

        if (machine.IsGrounded)
        {
            machine.SwitchState(new GroundedState(machine));
            return;
        }

        if (!machine.IsTouchingWall)
        {
            machine.SwitchState(new FallState(machine));
            return;
        }

        // Wall jump input
        if (Input.GetKeyDown(KeyCode.Space) && !hasWallJumped)
        {
            machine.SwitchState(new WallJumpState(machine));
            hasWallJumped = true;
        }
    }

    public void FixedUpdate()
    {
        // Smooth downward slide (faster and capped)
        float newYVelocity = Mathf.Max(rb.linearVelocity.y, -slideSpeed);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, newYVelocity);
    }

    public void Exit() { }
}
