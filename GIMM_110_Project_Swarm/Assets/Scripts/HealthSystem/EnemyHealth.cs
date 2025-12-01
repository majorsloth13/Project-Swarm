using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 50f;
    public float currentHealth { get; private set; }
    private bool dead;

    private EnemyStateMachine stateMachine;
    private Animator anim;

    private void Awake()
    {
        currentHealth = maxHealth;
        stateMachine = GetComponent<EnemyStateMachine>();
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        if (dead) return;

        currentHealth -= damage;

        // Optional: play hurt animation
        anim?.SetTrigger("hurt");

        if (currentHealth <= 0)
        {
            dead = true;
            stateMachine.SwitchState(new EnemyDeathState(stateMachine));
        }
    }
}
