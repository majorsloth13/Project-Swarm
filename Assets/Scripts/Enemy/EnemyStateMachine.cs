using System.Collections;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private IEnemyState currentState;

    [Header("Movement")]
    public float speed = 3f;

    [Header("Patrol Bounds")]
    public float leftBound;
    public float rightBound;
    public LayerMask obstacleMask;

    [Header("Ground & Edge Detection")]
    public Transform groundCheck;
    public Transform edgeCheck;
    public float groundCheckDistance = 0.2f;
    public float edgeCheckDistance = 0.3f;
    public LayerMask groundMask;

    [Header("Combat")]
    public float attackRange = 1.5f;
    public BoxCollider2D slashHitbox;
    public GameObject Slashbox;

    [Header("References")]
    public EnemySensor sensor;
    public Rigidbody2D rb;
    public Transform player;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sensor = GetComponent<EnemySensor>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
        SwitchState(new EnemyPatrolState(this));
        
    }

    void Update()
    {
        currentState?.Update();
        anim.SetBool("isWalkingEnemy", true);
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

    public void ActivateSlashbox(float duration)
    {
        StartCoroutine(SlashboxRoutine(duration));
    }

    private IEnumerator SlashboxRoutine(float duration)
    {
        Slashbox.SetActive(true);
        yield return new WaitForSeconds(duration);
        Slashbox.SetActive(false);
    }

}
