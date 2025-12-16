using UnityEngine;

public class GroundedRunState : IGroundedSubState
{
    private PlayerStateMachine machine;
    private GroundedState parent;
    private Rigidbody2D rb;
    private float speed;

    public GroundedRunState(PlayerStateMachine machine, GroundedState parent)
    {
        this.machine = machine;
        this.parent = parent;
        rb = machine.Rb;
        speed = machine.GetHorizontalSpeed();
    }

    public void Enter()
    {
        Debug.Log("entered grounded run");
    }

    public void Update()
    {
        
       /* if (!machine.IsGrounded)
        {
            machine.SwitchState(new FallState(machine));
            return;
        }*/

        float input = Input.GetAxisRaw("Horizontal");
        machine.FlipToGunDirection();

        if (Mathf.Abs(input) < 0.01f)
        {
            parent.SetSubState(new GroundedIdleState(machine, parent));
            //machine.SwitchState(new IdleState(machine));
            //return;
        }

       /* if (Input.GetKeyDown(KeyCode.Space) || machine.TryConsumeJumpBuffer())
        {
            Debug.Log("got jump from run");
            machine.SwitchState(new JumpState(machine));
            return;
        }*/

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
