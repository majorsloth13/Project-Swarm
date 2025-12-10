using UnityEngine;

public class FallState : IPlayerState, IPlayerPhysicsState
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
            machine.SwitchState(new DoubleJumpState(machine));
            return;
        }

        // Ground pound trigger
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            machine.SwitchState(new GroundPoundState(machine));
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
