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

        // Disable collider (optional)
        Collider2D col = enemy.GetComponent<Collider2D>();
        if (col) col.enabled = false;

        // Play animation
        Animator anim = enemy.GetComponent<Animator>();
        if (anim) anim.SetTrigger("die");

        // Destroy enemy after animation
        enemy.StartCoroutine(DestroyAfterDelay());
    }

    public void Update() { }

    public void Exit() { }

    private System.Collections.IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(1.0f);
        GameObject.Destroy(enemy.gameObject);
    }
}
