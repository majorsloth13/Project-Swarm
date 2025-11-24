using UnityEngine;

public class DoubleJumpState : IPlayerState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;
    private float jumpForce;

    public DoubleJumpState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
        jumpForce = machine.jumpForce; // use same jump force or separate config
    }

    public void Enter()
    {
        // Reset Y velocity and apply double-jump impulse
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // consume double jump immediately
        machine.HasDoubleJump = false;
    }

    public void Update()
    {
        // Horizontal air control
        float xInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(xInput * machine.airMoveSpeed, rb.linearVelocity.y);

        // Go to Fall once we are falling
        if (rb.linearVelocity.y <= 0f)
        {
            machine.SwitchState(new FallState(machine));
            return;
        }

        // Wall slide if touching wall and moving down
        if (machine.IsTouchingWall && rb.linearVelocity.y <= 0f)
        {
            machine.SwitchState(new WallSlideState(machine));
            return;
        }
    }

    public void Exit() { }
}
