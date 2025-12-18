using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Health : HealthBase
{
    [SerializeField] private RespawnManager respawnManager; // Reference to the respawn manager for handling player death
    private Animator anim;                                  // Animator reference
    private bool dead;                                      // Tracks death status
    private SpriteRenderer spriteRend;                      // For visual feedback (optional)

    [Header("Invincibility")]
    public bool isInvincible /*{ get; private set; }*/ = false;

    //public void SetInvincible(bool value)
    //{
    //    isInvincible = value;
    //}

    private PlayerStateMachine machine;

   




    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        machine = GetComponent<PlayerStateMachine>();
        dead = false;
    }

    private void Update()
    {
        
    }

    /// <summary>
    /// Call this to deal damage to the player.
    /// </summary>
    public override void TakeDamage(float dmg)
    {
        if (machine.diamondSkinActive)
        {
            machine.diamondSkinCurrentHealth -= (int)dmg;
            Debug.Log("Diamond Skin HP: " + machine.diamondSkinCurrentHealth);

            if (machine.diamondSkinCurrentHealth <= 0)
            {
                Debug.Log("the shiled has brokenafesbjsfjysdfgdgkdgyuseg");
                machine.diamondSkinActive = false;
                machine.diamondSkin.gameObject.SetActive(false); // <-- deactivate shield visually
                //machine.KingScanned = false;
            }

            return; // block normal damage
        }
        

        // Normal damage
        if (!isInvincible)
        {
            machine.SwitchState(new HurtState(machine));
            base.TakeDamage(dmg);
        }
    }




protected override void Die()
{
    if (dead)
        return;

    dead = true;
    Debug.Log("Player died (routing to DeathState)");

    if (machine != null)
    {
        machine.SwitchState(new DeathState(machine));
    }
    else
    {
        Debug.LogError("PlayerStateMachine missing on player!");
    }
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
