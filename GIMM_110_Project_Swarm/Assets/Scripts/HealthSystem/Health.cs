using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;       // Maximum health
    public float currentHealth;          // Current health, public for OverhealthState

    [SerializeField] private RespawnManager respawnManager;

    private Animator anim;
    private bool dead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        Debug.Log($"Player starting health: {currentHealth}");
    }

    /// <summary>
    /// Deal damage to the player
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (dead) return;

        currentHealth -= damage;
        if (currentHealth < 0f) currentHealth = 0f;

        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");

        if (currentHealth == 0f)
        {
            Die();
        }
        else
        {
            anim?.SetTrigger("hurt");
        }
    }

    /// <summary>
    /// Fully restore health
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        dead = false;
        Debug.Log($"Player health reset: {currentHealth}");
    }

    /// <summary>
    /// Temporarily apply extra health (overhealth)
    /// </summary>
    public void SetTemporaryHealth(float newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0f, maxHealth + 1000f);
        Debug.Log($"Temporary health applied. Current health: {currentHealth}");
    }

    private void Die()
    {
        dead = true;
        anim?.SetTrigger("die");

        // Fix CS0131 by using a variable instead of null-conditional assignment
        PlayerStateMachine psm = GetComponent<PlayerStateMachine>();
        if (psm != null)
        {
            psm.enabled = false;
        }

        respawnManager?.RespawnPlayer();
        Debug.Log("Player has died.");
    }
}
