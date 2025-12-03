/*using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackCoolDown;

    [SerializeField]private float canAttack;
    public GameObject AttackBox;
    private Animator animator;
    public Transform attackPoint;
    public float attackRange;
    public LayerMask Enemy;
    float movement;
    SpriteRenderer sprite;
    private bool aceOfSpades;

    public int Damage = 1;
    public Movement_Slash BuffTimer;
    void Start()
    {
        animator = GetComponent<Animator>();
        AttackBox.SetActive(false);
        sprite = GetComponent<SpriteRenderer>();
        aceOfSpades = GetComponent<Movement_Slash>();
        BuffTimer = FindObjectOfType<Movement_Slash>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            aceOfSpades = true;
            
        }
        
            
            if (attackCoolDown <= 0 && aceOfSpades)
            {

                AttackBox.SetActive(false);
                if (Input.GetMouseButtonDown(0)) // Assuming "Fire1" is your attack input
                {
                    Debug.Log("attacked");
                    Attack();
                    AttackBox.SetActive(true);
                    attackCoolDown = canAttack;
                }

            }
            else
            {

                attackCoolDown -= Time.deltaTime;
            

        }
        if (BuffTimer.buffTimer == 15)
        {
            aceOfSpades = false;
        }




        }


    public void Attack()
    {
        

        Collider2D[] enemyHit = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, Enemy);

        foreach(Collider2D Enemy in enemyHit)
        {
            Debug.Log("enemy hit");
            Enemy.GetComponent<Enemy>().damaged(Damage);
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    

}*/
