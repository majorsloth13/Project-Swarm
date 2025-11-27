using UnityEngine;

public class FallState : IPlayerState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;

    public FallState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
    }

    public void Enter() { }

    public void Update()
    {
        // Horizontal mid-air control
        float input = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(input * machine.airMoveSpeed, rb.linearVelocity.y);

        // Landing detection -> switch to GroundedState
        if (machine.IsGrounded)
        {
            machine.SwitchState(new GroundedState(machine));
            return;
        }

        // Wall slide
        if (machine.IsTouchingWall && rb.linearVelocity.y <= 0f)
        {
            machine.SwitchState(new WallSlideState(machine));
            return;
        }

        // If player presses Space and has double jump available -> double jump
        if (Input.GetKeyDown(KeyCode.Space) && machine.HasDoubleJump)
        {
            machine.SwitchState(new DoubleJumpState(machine));
            return;
        }
    }

    public void Exit() { }
}
