using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    private EnemyStateMachine enemy;

    private float attackCooldown = 2f; // 2 seconds
    private float cooldownTimer = 0f;

    public EnemyAttackState(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.GetRb().linearVelocity = Vector2.zero;
        cooldownTimer = 0f; // ready to attack immediately if in range
    }

    public void Update()
    {
        float distance = Vector2.Distance(
            enemy.transform.position,
            enemy.Player.position
        );

        // Leave attack state if player is out of range
        if (distance > enemy.attackRange)
        {
            enemy.SwitchState(new EnemyChaseState(enemy));
            return;
        }

        // Cooldown ticking
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }

        // Attack
        PerformAttack();
        cooldownTimer = attackCooldown;
    }


    public void Exit()
    {
        enemy.Slashbox.SetActive(false);
    }

    private void PerformAttack()
    {
        enemy.ActivateSlashbox(1f);
        Debug.Log("ENEMY ATTACKED!");
    }


}
