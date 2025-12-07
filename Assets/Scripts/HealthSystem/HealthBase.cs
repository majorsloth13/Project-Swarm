using UnityEngine;

public abstract class HealthBase : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    protected abstract void Die();
}
