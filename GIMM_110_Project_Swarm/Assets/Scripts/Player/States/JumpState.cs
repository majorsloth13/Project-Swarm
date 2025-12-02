using UnityEngine;

public class JumpState : IPlayerState, IPlayerPhysicsState
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
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void Update()
    {
        float xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && machine.HasDoubleJump)
        {
            machine.SwitchState(new DoubleJumpState(machine));
            return;
        }

        if (rb.linearVelocity.y <= 0f)
        {
            machine.SwitchState(new FallState(machine));
            return;
        }

        if (machine.IsTouchingWall && rb.linearVelocity.y <= 0f)
        {
            machine.SwitchState(new WallSlideState(machine));
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
