using UnityEngine;

public class GroundedState : IPlayerState, IPlayerPhysicsState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;

    public GroundedState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
    }

    public void Enter()
    {
        machine.HasDoubleJump = true;
        machine.coyoteTimer = machine.coyoteTime;

        if (machine.TryConsumeJumpBuffer())
        {
            machine.SwitchState(new JumpState(machine));
            return;
        }
    }

    public void Update()
    {
        if (!machine.IsGrounded)
        {
            machine.SwitchState(new FallState(machine));
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || machine.TryConsumeJumpBuffer())
        {
            Debug.Log("got jump from grounded");
            machine.SwitchState(new JumpState(machine));
            return;
        }

        float input = Input.GetAxisRaw("Horizontal");
        machine.FlipToGunDirection();
        if (Mathf.Abs(input) > 0.01f)
        {
            machine.SwitchState(new RunState(machine)); // assumes you have a RunState
        }
        else
        {
            machine.SwitchState(new IdleState(machine)); // assumes you have an IdleState
        }
    }

    public void FixedUpdate()
    {
        float input = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(input * machine.horizontalSpeed, rb.linearVelocity.y);
    }

    public void Exit() { }
}
