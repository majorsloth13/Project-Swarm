using UnityEngine;

public class WallSlideState : IPlayerState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;
    private float slideSpeed = 1.5f;
    private float stepTimer;

    public WallSlideState(PlayerStateMachine machine, float slideSpeed = 1.5f)
    {
        this.machine = machine;
        rb = machine.Rb;
        this.slideSpeed = slideSpeed;
    }

    public void Enter()
    {
        stepTimer = 0f;
        // Optionally zero X velocity to cling
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void Update()
    {
        // If grounded -> transition to Grounded
        if (machine.IsGrounded)
        {
            machine.SwitchState(new GroundedState(machine));
            return;
        }

        // If not touching any wall -> fall
        if (!machine.IsTouchingWall)
        {
            machine.SwitchState(new FallState(machine));
            return;
        }

        // Step-based sliding (keeps consistent with StepInterval)
        stepTimer += Time.deltaTime;
        if (stepTimer >= machine.StepInterval)
        {
            stepTimer = 0f;
            // Move down by slideSpeed * StepInterval
            rb.linearVelocity = new Vector2(0f, -slideSpeed);
        }

        // Wall jump away
        if (Input.GetKeyDown(KeyCode.Space))
        {
            machine.SwitchState(new WallJumpState(machine));
            return;
        }
    }

    public void Exit() { }
}
