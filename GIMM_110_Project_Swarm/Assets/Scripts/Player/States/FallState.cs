using UnityEngine;

public class FallState : IPlayerState, IPlayerPhysicsState
{
    private PlayerStateMachine machine;
    public Rigidbody2D rb;

    public FallState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
    }

    public void Enter() { }

    public void Update()
    {
        machine.FlipToGunDirection();

        if (machine.IsGrounded)
        {
            machine.SwitchState(new GroundedState(machine));
            return;
        }

        if (machine.IsTouchingWall && rb.linearVelocity.y <= 0f)
        {
            machine.SwitchState(new WallSlideState(machine));
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && machine.HasDoubleJump)
        {
            machine.HasDoubleJump = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, machine.jumpForce);
            machine.SwitchState(new DoubleJumpState(machine));
            return;
        }
    }

    public void FixedUpdate()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(xInput * machine.airMoveSpeed, rb.linearVelocity.y);
    }

    public void Exit() { }
}
