using UnityEngine;

public class GroundedState : IPlayerState, IPlayerPhysicsState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;

    private IGroundedSubState currentSubState;

    public GroundedState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
    }

    public void Enter()
    {
        machine.HasDoubleJump = true;
        machine.coyoteTimer = machine.coyoteTime;

        Collider2D platformCollider = machine.GroundCheck.GetGroundCollider();

        if (platformCollider != null)
        {
            machine.currentEffector = platformCollider.GetComponent<PlatformEffector2D>();
            machine.currentPlatformCollider = platformCollider;
        }
        else
        {
            machine.currentEffector = null;
            machine.currentPlatformCollider = null;
        }

        SetSubState(new GroundedIdleState(machine, this));
        /*if (machine.TryConsumeJumpBuffer())
        {
            machine.SwitchState(new JumpState(machine));
            return;
        }*/
    }

    public void Update()
    {
        machine.FlipToGunDirection();

        // Drop through input check
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            && machine.IsGrounded)
        {
            machine.StartCoroutine(machine.DropRoutine());
            return;
        }


        // check if player is still grounded
        if (!machine.IsGrounded)
        {
            machine.SwitchState(new FallState(machine));
            return;
        }

        // Jump input check
        if (Input.GetKeyDown(KeyCode.Space) || machine.TryConsumeJumpBuffer())
        {
            Debug.Log("got jump from grounded");
            machine.SwitchState(new JumpState(machine));
            return;
        }

        currentSubState?.Update();
        // Movement/Idle Transition
        /*float input = Input.GetAxisRaw("Horizontal");
        machine.FlipToGunDirection();
        if (Mathf.Abs(input) > 0.01f)
        
            machine.SwitchState(new RunState(machine)); // assumes you have a RunState
        
        else
        
            machine.SwitchState(new IdleState(machine)); // assumes you have an IdleState*/
        
    }

    public void FixedUpdate()
    {
        currentSubState?.FixedUpdate();
        // float input = Input.GetAxisRaw("Horizontal");
        // rb.linearVelocity = new Vector2(input * machine.horizontalSpeed, rb.linearVelocity.y);
    }

    public void Exit()
    {
        currentSubState?.Exit();
    }

    public void SetSubState(IGroundedSubState newSubState)
    {
        currentSubState?.Exit();
        currentSubState = newSubState;
        currentSubState.Enter();
    }
}
