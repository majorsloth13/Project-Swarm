using UnityEngine;

public class EnemyDeathState : IEnemyState
{
    private EnemyStateMachine enemy;

    public EnemyDeathState(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        // Stop movement
        enemy.rb.linearVelocity = Vector2.zero;

        // Disable collider
        Collider2D col = enemy.GetComponent<Collider2D>();
        if (col) col.enabled = false;

        // Play death animation
        Animator anim = enemy.GetComponent<Animator>();
        if (anim) anim.SetTrigger("die");

        // Increment kill counter
        KillCounter.Instance?.AddKill();

        // Destroy enemy after animation
        enemy.StartCoroutine(DestroyAfterDelay());
    }

    public void Update() { }

    public void Exit() { }

    private System.Collections.IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(1.0f); // adjust to match animation length
        GameObject.Destroy(enemy.gameObject);
    }
}
