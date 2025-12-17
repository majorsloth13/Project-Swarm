using UnityEngine;

public class DeathState : IPlayerState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;
    private Animator anim;
    private RespawnManager respawn;

    public DeathState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
        anim = machine.GetComponent<Animator>();
        respawn = Object.FindAnyObjectByType<RespawnManager>(); // find manager once
    }

    public void Enter()
    {
        anim.SetBool("isHurt", true);
        Debug.Log("Died");

        //anim?.SetTrigger("die");

        // Play the death sound using the static helper
        AudioClip deathClip = machine.DeathSoundClip;

        if (deathClip != null)
        {
            // PlayClipAtPoint creates a one-shot AudioSource at the player's position
            AudioSource.PlayClipAtPoint(deathClip, machine.transform.position);
        }

        // Stop movement
        rb.linearVelocity = Vector2.zero;

        // Disable movement system
        machine.enabled = false;

        // Call Respawn
        respawn?.RespawnPlayer();
    }

    public void Update() { }

    public void Exit() { anim.SetBool("isHurt", false); }
}
