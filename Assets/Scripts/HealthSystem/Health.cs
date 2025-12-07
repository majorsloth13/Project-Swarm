using System.Collections;
using UnityEngine;

public class Health : HealthBase
{
    [SerializeField] private RespawnManager respawnManager; // Reference to the respawn manager for handling player death
    private Animator anim;                                  // Animator reference
    private bool dead;                                      // Tracks death status
    private SpriteRenderer spriteRend;                      // For visual feedback (optional)

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        dead = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(1);
        }
    }

    /// <summary>
    /// Call this to deal damage to the player.
    /// </summary>
    public override void TakeDamage(float dmg)
    {
        Debug.Log(name + " took " + dmg + " damage!");
        if (dead)
        {
            Debug.LogWarning("TakeDamage called while already dead!");
        }

        base.TakeDamage(dmg);

        if (!dead && currentHealth > 0)
        {
            if (anim != null)
            // Hurt feedback if still alive
            anim.SetTrigger("hurt");
        }
    }
    protected override void Die()
    {
        // Death sequence
        if (dead)
            return;

        dead = true;
        Debug.Log("Player died.");

        if (anim != null)
            anim.SetTrigger("die");

        // Disable movement temporarily
        PlayerStateMachine move = GetComponent<PlayerStateMachine>();
        if (move != null)
            move.enabled = false;

            // Respawn after delay
            if (respawnManager != null)
                respawnManager.RespawnPlayer();
    }
    

    /// <summary>
    /// Fully restores health and revives player.
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        dead = false;
        Debug.Log("Player health reset to full.");
    }
}
