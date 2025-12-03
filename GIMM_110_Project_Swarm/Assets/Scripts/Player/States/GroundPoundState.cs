using UnityEngine;

public class GroundPoundState : IPlayerState, IPlayerPhysicsState
{
    // References the main player state machine
    private PlayerStateMachine machine;
    private Rigidbody2D rb;

    public GroundPoundState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
    }

    public void Enter() { }

    public void Update()
    {
       
       
    }

    public void FixedUpdate()
    {
      
    }

    public void Exit() { }
}
