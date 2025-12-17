using System.Collections;
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
        respawn = Object.FindAnyObjectByType<RespawnManager>();
    }

    public void Enter()
    {
        Debug.Log("Died");

        if (anim != null)
        {
            anim.SetBool("isHurt", true);
        }

        // Stop movement
        rb.linearVelocity = Vector2.zero;

        // Play death sound
        if (machine.DeathSoundClip != null && machine.audioSource != null)
        {
            machine.audioSource.Stop();
            machine.audioSource.clip = machine.DeathSoundClip;
            machine.audioSource.Play();
        }

        // Start respawn delay coroutine 
        machine.StartCoroutine(DeathRoutine());
    }

    public void Update()
    {
        // Intentionally empty (player is dead)
    }

    public void Exit()
    {
        if (anim != null)
        {
            anim.SetBool("isHurt", false);
        }
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        machine.enabled = false;

        if (respawn != null)
        {
            respawn.RespawnPlayer();
        }
    }
}
