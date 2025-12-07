using UnityEngine;

public class IdleState : IPlayerState, IPlayerPhysicsState
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
        Debug.Log("enetered idle");
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void Update()
    {
        
        if (!machine.IsGrounded)
        {

            machine.SwitchState(new FallState(machine));
            return;
        }

        float input = Input.GetAxisRaw("Horizontal");
        machine.FlipToGunDirection();
        if (Mathf.Abs(input) > 0.01f)
        {
            machine.SwitchState(new RunState(machine));
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || machine.TryConsumeJumpBuffer())
        {
            Debug.Log("got jump from idel");
            machine.SwitchState(new JumpState(machine));
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) && machine.HasDoubleJump)
        {
            Debug.Log("double jump!");
            machine.SwitchState(new DoubleJumpState(machine));
            return;
        }
    }

    public void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void Exit() { }
}
