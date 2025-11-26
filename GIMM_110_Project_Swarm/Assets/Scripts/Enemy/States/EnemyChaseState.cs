using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    private EnemyStateMachine ctx;

    public EnemyChaseState(EnemyStateMachine ctx)
    {
        this.ctx = ctx;
    }

    public void Enter() { }

    public void Update()
    {
        if (!ctx.sensor.playerDetected)
        {
            ctx.SwitchState(new EnemyPatrolState(ctx));
            return;
        }

        float dist = Vector2.Distance(ctx.transform.position, ctx.Player.position);

        if (dist <= ctx.attackRange)
        {
            ctx.SwitchState(new EnemyAttackState(ctx));
            return;
        }

        float dir = Mathf.Sign(ctx.Player.position.x - ctx.transform.position.x);

        ctx.rb.linearVelocity = new Vector2(dir * ctx.speed, ctx.rb.linearVelocity.y);
    }

    public void Exit()
    {
        ctx.rb.linearVelocity = Vector2.zero;
    }
}
