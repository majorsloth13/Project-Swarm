using UnityEngine;

public class IdleState : IPlayerState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;

    public IdleState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
    }

    public void Enter()
    {
        // Stop horizontal velocity on enter (optional)
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void Update()
    {
        // If no longer grounded, go to Fall
        if (!machine.IsGrounded)
        {
            machine.SwitchState(new FallState(machine));
            return;
        }

        float input = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(input) > 0.01f)
        {
            machine.SwitchState(new RunState(machine));
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || machine.TryConsumeJumpBuffer())
        {
            machine.SwitchState(new JumpState(machine));
            return;
        }
    }

    public void Exit() { }
}
