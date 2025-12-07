using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class DeathState : IPlayerState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;
    private Animator anim;

    public DeathState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
        anim = machine.GetComponent<Animator>();
    }

    public void Enter()
    {
        Debug.Log("Died");

        anim?.SetTrigger("die");

        // Stop movement
        rb.linearVelocity = Vector2.zero;

        // Disable state-machine-controlled movement
        machine.enabled = false;
    }

    public void Update() { }

    public void Exit() { }
}
