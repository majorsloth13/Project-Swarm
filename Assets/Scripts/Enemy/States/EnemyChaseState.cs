using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    private EnemyStateMachine ctx;
    private LayerMask obstacleMask;

    public EnemyChaseState(EnemyStateMachine ctx)
    {
        this.ctx = ctx;
        obstacleMask = LayerMask.GetMask("Ground", "Wall");
    }

    public void Enter() {
    }
    
    public bool HasLineOfSight(Transform enemy, Transform player)
    {
        Vector2 dir = player.position - enemy.position;
        float dist = dir.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(enemy.position, dir.normalized, dist, obstacleMask);

        // If hit something BEFORE the player → blocked
        if (hit.collider != null && !hit.collider.CompareTag("Player"))
        {
            Debug.Log("raycast did not hit player");
            return false;
            
        }
        Debug.Log("hit player raycast");
        return true;
        
    }


    public void Update()
    {
        Debug.Log("in chase state");

        if (!HasLineOfSight(ctx.transform, ctx.Player))
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

        /*ctx.transform.localScale = new Vector3(
            Mathf.Abs(ctx.transform.localScale.x) * dir,
            ctx.transform.localScale.y,
            ctx.transform.localScale.z
        );*/
    }

    public void Exit()
    {
        ctx.rb.linearVelocity = Vector2.zero;
    }
}
