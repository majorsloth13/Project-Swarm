using UnityEngine;

public class EnemyPatrolState : IEnemyState
{
    private EnemyStateMachine ctx;
    private int direction = 1;
    private Vector3 startingPosition;

    public EnemyPatrolState(EnemyStateMachine ctx)
    {
        this.ctx = ctx;
    }

    public void Enter()
    {
        startingPosition = ctx.transform.position;
    }

    public void Update()
    {
        if (ctx.sensor.playerDetected)
        {
            ctx.SwitchState(new EnemyChaseState(ctx));
            return;
        }

        bool hitGround = Physics2D.Raycast(
            ctx.edgeCheck.position,
            Vector2.down,
            ctx.edgeCheckDistance,
            ctx.groundMask
        );

        bool isEdge = !hitGround;

        MonoBehaviour.print("isEdge & hitGround: " + isEdge + " & " + hitGround);

        if (isEdge)
        {
            Flip();
        }
        else if (ctx.transform.position.x <= startingPosition.x + ctx.leftBound)
        {
            direction = 1;
            FlipToDirection(1);
        }
        else if (ctx.transform.position.x >= startingPosition.x + ctx.rightBound)
        {
            direction = -1;
            FlipToDirection(-1);
        }

        ctx.rb.linearVelocity = new Vector2(
            direction * ctx.speed,
            ctx.rb.linearVelocity.y
        );
    }

    public void Exit()
    {
        ctx.rb.linearVelocity = Vector2.zero;
    }

    private void Flip()
    {
        direction = -1;
        FlipToDirection(direction);
    }

    private void FlipToDirection(int dir)
    {
        Vector3 scale = ctx.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dir;
        ctx.transform.localScale = scale;
    }
}