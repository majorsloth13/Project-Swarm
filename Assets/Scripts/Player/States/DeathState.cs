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
        Debug.Log("DeathState FILE: " + GetType().AssemblyQualifiedName);

        Debug.Log("DeathState Enter reached");

        if (anim != null)
        {
            anim.SetBool("isHurt", true);
        }

        // Stop movement
        rb.linearVelocity = Vector2.zero;

        Debug.Log("AudioSource null? " + (machine.audioSource == null));
        Debug.Log("Death clip null? " + (machine.DeathSoundClip == null));

        // Play death sound
        if (machine.DeathSoundClip != null && machine.audioSource != null)
        {
            //machine.audioSource.Stop();
            machine.audioSource.PlayOneShot(machine.DeathSoundClip);
            machine.audioSource.Play();

            Debug.Log("Attempted to play death sound");
        }
        else
        {
            Debug.LogWarning("Death sound NOT played due to null reference");
        }
        // Start respawn delay coroutine 
        machine.StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        machine.enabled = false;

        yield return new WaitForSeconds(0.5f);

        if (respawn != null)
        {
            respawn.RespawnPlayer();
        }
        // Re-enable immediately after respawn
        //machine.enabled = true;
        machine.SwitchState(new GroundedState(machine));
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


}
