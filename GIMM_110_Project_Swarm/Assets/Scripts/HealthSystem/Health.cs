using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    private PlayerStateMachine playerSM;

    [Header("Health")]
    [SerializeField] private float startingHealth = 30f;
    public float currentHealth { get; private set; }

    

    private Animator anim;
    private bool dead;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        playerSM = GetComponent<PlayerStateMachine>();
    }
    public bool isInvincible { get; private set; } = false;

    public void SetInvincible(bool value)
    {
        isInvincible = value;
    }

    public bool TakeDamage(float damage)
    {
        if (isInvincible)
        {
            Debug.Log("Player is invincible! Damage blocked.");
            return false;
        }


        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
        Debug.Log($"Player took {damage} damage. HP = {currentHealth}");

        if (currentHealth > 0)
        {
            
            // Switch to Hurt State
            
            playerSM.SwitchState(new HurtState(playerSM));

            // Start i-frames
            
        }
        else
        {
            dead = true;
            playerSM.SwitchState(new DeathState(playerSM));
        }

        return true;
    }


    




/// <summary>
/// Fully restores health and revives player.
/// </summary>
public void ResetHealth()
    {
        currentHealth = startingHealth;
        dead = false;
        Debug.Log("Player health reset to full.");
    }
}
