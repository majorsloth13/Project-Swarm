using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    private EnemyStateMachine enemy;
    private float attackCooldown = 1.0f;
    private float lastAttackTime;

    public EnemyAttackState(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.GetRb().linearVelocity = Vector2.zero;
    }

    public void Update()
    {
        float dist = Vector2.Distance(enemy.transform.position, enemy.Player.position);

        if (dist > enemy.attackRange)
        {
            enemy.SwitchState(new EnemyChaseState(enemy));
            return;
        }

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    public void Exit() { }

    private void PerformAttack()
    {
        Debug.Log("ENEMY ATTACKED!");
    }
}
