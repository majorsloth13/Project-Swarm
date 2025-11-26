using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private IEnemyState currentState;

    [Header("Movement")]
    public float speed = 3f;

    [Header("Patrol Bounds")]
    public float leftBound;
    public float rightBound;

    [Header("Ground & Edge Detection")]
    public Transform groundCheck;
    public Transform edgeCheck;
    public float groundCheckDistance = 0.2f;
    public float edgeCheckDistance = 0.3f;
    public LayerMask groundMask;

    [Header("Combat")]
    public float attackRange = 1.5f;

    [Header("References")]
    public EnemySensor sensor;
    public Rigidbody2D rb;
    public Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //sensor = GetComponent<EnemySensor>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        SwitchState(new EnemyPatrolState(this));
    }

    void Update()
    {
        currentState?.Update();
    }

    public void SwitchState(IEnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    // Accessor for states
    public Rigidbody2D GetRb() => rb;
    public Transform Player => player;

    void OnDrawGizmosSelected()
    {
        if (edgeCheck == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            edgeCheck.position,
            edgeCheck.position + Vector3.down * edgeCheckDistance
        );
    }
}
