using UnityEngine;

public class RunState : IPlayerState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;
    private float speed;

    public RunState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
        speed = machine.GetHorizontalSpeed();
    }

    public void Enter() { }

    public void Update()
    {
        if (!machine.IsGrounded)
        {
            machine.SwitchState(new FallState(machine));
            return;
        }

        float input = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(input) < 0.01f)
        {
            machine.SwitchState(new IdleState(machine));
            return;
        }

        // Move
        rb.linearVelocity = new Vector2(input * speed, rb.linearVelocity.y);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) || machine.TryConsumeJumpBuffer())
        {
            machine.SwitchState(new JumpState(machine));
            return;
        }
    }

    public void Exit() { }
}
