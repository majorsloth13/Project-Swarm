using System.Collections;
using System.Threading;
using UnityEngine;

public class HurtState : IPlayerState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;
    private Animator anim;
    public Vector2 knockbackForce = new Vector2(-5f, 3f);

   
    private float hurtDuration = 0.25f;   // how long the player stays in the hurt state
    private float hurtTimer;

    [Header("Invincibility Frames")]
    [SerializeField] private float iFrameDuration = 2f;
    private bool isInvincible = false;
    private Health health;

    public HurtState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
        
        anim = machine.GetComponent<Animator>();
    }

    public void Enter()
    {
        anim.SetBool("isHurt", true);
        Debug.Log("eneterd hurt state");
        health = machine.GetComponent<Health>();
        hurtTimer = hurtDuration;

        // Play animation only if you want to (optional)
        //anim.SetTrigger("hurt");

        Debug.Log("skjhgfsufusbfusf");
        if (!health.isInvincible)
        {
            rb.linearVelocity = knockbackForce;
            Iframe();
        }
    }

    public void Update()
    {
        hurtTimer -= Time.deltaTime;

        if (hurtTimer <= 0f)
        {
            machine.SwitchState(new GroundedState(machine));
            //machine.SwitchState(new GroundedIdleState(machine));
            Debug.Log("swithc to idle");
            return;
        }
    }

    public void Exit()
    {
        // Anything to reset on exit� usually nothing needed.
        Debug.Log("exited hurt state");
        anim.SetBool("isHurt", false);
    }

    public void Iframe()
    {
        machine.StartCoroutine(IFrames());
    }

    private IEnumerator IFrames()
    {
        health.isInvincible = true;

        Debug.Log("is invincible");
        // Optional: Flash sprite
        SpriteRenderer sr = machine.GetComponent<SpriteRenderer>();

        float timer = 0f;
        while (timer < iFrameDuration)
        {
            if (sr != null)
                sr.enabled = !sr.enabled; // flicker effect

            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        if (sr != null) sr.enabled = true;

        health.isInvincible = false;
        Debug.Log("is not invincible");
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

    }

} 

