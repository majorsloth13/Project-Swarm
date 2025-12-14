using UnityEngine;

public class GroundedIdleState : IGroundedSubState
{
    private PlayerStateMachine machine;
    private GroundedState parent;
    private Rigidbody2D rb;

    public GroundedIdleState(PlayerStateMachine machine, GroundedState parent)
    {
        this.machine = machine;
        this.parent = parent;
        rb = machine.Rb;
    }

    public void Enter()
    {
        Debug.Log("entered grounded idle");
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void Update()
    {
        float input = Input.GetAxisRaw("Horizontal");
        machine.FlipToGunDirection();

        // Switch to RUN (substate, not main state!)
        if (Mathf.Abs(input) > 0.01f)
        {
            parent.SetSubState(new GroundedRunState(machine, parent));
        }
    }

    public void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void Exit() { }
}
