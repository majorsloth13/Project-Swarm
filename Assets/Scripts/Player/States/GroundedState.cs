using UnityEngine;

public class GroundedState : IPlayerState//, IPlayerPhysicsState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;

    public IGroundedSubState currentSubState;

    public GroundedState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
    }

    public void Enter()
    {
        machine.HasDoubleJump = true;
        machine.coyoteTimer = machine.coyoteTime;

        SetSubState(new GroundedIdleState(machine, this));

        if (machine.TryConsumeJumpBuffer())
        {
            machine.SwitchState(new JumpState(machine));
           // return;
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

        currentSubState?.Update();

        /*float input = Input.GetAxisRaw("Horizontal");
        machine.FlipToGunDirection();
        if (Mathf.Abs(input) > 0.01f)
        {
            machine.SwitchState(new RunState(machine)); // assumes you have a RunState
        }
        else
        {
            machine.SwitchState(new IdleState(machine)); // assumes you have an IdleState
        }*/
    }

    public void FixedUpdate()
    {
        currentSubState?.FixedUpdate();
        /*float input = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(input * machine.horizontalSpeed, rb.linearVelocity.y);*/
    }

    public void Exit() 
    {
        currentSubState?.Exit();
    }

    public void SetSubState(IGroundedSubState subState)
    {
        if (subState == null) return;

        currentSubState?.Exit();
        currentSubState = subState;
        currentSubState.Enter();
    }
    /*public void OnCollisionEnter2D(Collision2D collision)
    {

    }*/
}
