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

        // (Optional) Play death animation - won't be seen if object is destroyed instantly
        Animator anim = enemy.GetComponent<Animator>();
        if (anim) anim.SetTrigger("die");

        // Increment kill counter
        KillCounter.Instance?.AddKill();

        // Destroy enemy immediately
        GameObject.Destroy(enemy.gameObject);
    }

    public void Update() { }

    public void Exit() { }
}
