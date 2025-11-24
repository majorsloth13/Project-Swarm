using UnityEngine;

public class JumpState : IPlayerState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;

    private float jumpForce;

    public JumpState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
        jumpForce = machine.jumpForce;
    }

    public void Enter()
    {
        // Reset vertical velocity then apply jump impulse
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // On initial jump we still have double-jump available (set by GroundedState previously)
        // If you prefer consuming double-jump on first jump, set machine.HasDoubleJump = false here.
    }

    public void Update()
    {
        // Horizontal control in air
        float xInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(xInput * machine.airMoveSpeed, rb.linearVelocity.y);

        // If jump pressed again and double jump available -> DoubleJump
        if (Input.GetKeyDown(KeyCode.Space) && machine.HasDoubleJump)
        {
            machine.SwitchState(new DoubleJumpState(machine));
            return;
        }

        // If we start falling -> go to fall state
        if (rb.linearVelocity.y <= 0f)
        {
            machine.SwitchState(new FallState(machine));
            return;
        }

        // If touching a wall and falling or moving down -> wall slide
        if (machine.IsTouchingWall && rb.linearVelocity.y <= 0f)
        {
            machine.SwitchState(new WallSlideState(machine));
            return;
        }
    }

    public void Exit() { }
}
