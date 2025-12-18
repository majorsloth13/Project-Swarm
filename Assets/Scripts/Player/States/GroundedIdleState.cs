using UnityEngine;

public class GroundedIdleState : IGroundedSubState//, IPlayerPhysicsState
{
    private PlayerStateMachine machine;
    private GroundedState parent;
    private Rigidbody2D rb;
    private Animator anim;

    public GroundedIdleState(PlayerStateMachine machine, GroundedState parent)
    {
        this.machine = machine;
        this.parent = parent;
        rb = machine.Rb;
        anim = machine.GetComponent<Animator>();
    }

    public void Enter()
    {
        Debug.Log("entered grounded idle");
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        anim.SetBool("isRunning", false);
        anim.SetBool("isWalkingAway", false);
        anim.SetBool("isFalling", false);
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
