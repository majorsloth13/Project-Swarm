using UnityEngine;

public class RunState : IPlayerState, IPlayerPhysicsState
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

    public void Enter() {  }

    public void Update()
    {
        
        if (!machine.IsGrounded)
        {
            machine.SwitchState(new FallState(machine));
            return;
        }

        float input = Input.GetAxisRaw("Horizontal");
        machine.FlipToGunDirection();

        if (Mathf.Abs(input) < 0.01f)
        {
            machine.SwitchState(new IdleState(machine));
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || machine.TryConsumeJumpBuffer())
        {
            Debug.Log("got jump from run");
            machine.SwitchState(new JumpState(machine));
            return;
        }

        // Double jump should never trigger on ground, but we still check consistently
        //if (Input.GetKeyDown(KeyCode.Space) && machine.HasDoubleJump)
        //{
        //    machine.SwitchState(new DoubleJumpState(machine));
        //    return;
        //}
    }


    public void FixedUpdate()
    {
        float input = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(input * speed, rb.linearVelocity.y);
        
    }

    public void Exit() { }
}
