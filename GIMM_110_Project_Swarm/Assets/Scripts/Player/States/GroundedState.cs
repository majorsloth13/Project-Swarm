using UnityEngine;

public class GroundedState : IPlayerState
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
        // Reset double-jump when we land
        machine.HasDoubleJump = true;
        // reset coyote (handled in machine.Update too, but this is explicit on Enter)
        machine.coyoteTimer = machine.coyoteTime;

        // If the player buffered a jump while falling right before landing, perform jump
        if (machine.TryConsumeJumpBuffer())
        {
            machine.SwitchState(new JumpState(machine));
            return;
        }
    }

    public void Update()
    {
        // If we are no longer grounded, go to Fall
        if (!machine.IsGrounded)
        {
            machine.SwitchState(new FallState(machine));
            return;
        }

        // Jump input (considers coyote time/time on ground)
        if (Input.GetKeyDown(KeyCode.Space) || machine.TryConsumeJumpBuffer())
        {
            // If grounded, jump immediately
            machine.SwitchState(new JumpState(machine));
            return;
        }

        // Movement: choose Idle vs Run
        float input = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(input) > 0.01f)
        {
            machine.SwitchState(new RunState(machine));
        }
        else
        {
            machine.SwitchState(new IdleState(machine));
        }
    }

    public void Exit() { }
}
