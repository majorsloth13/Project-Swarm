using System.Collections;
using UnityEngine;

public class HurtState : IPlayerState, IPlayerPhysicsState
{
    private PlayerStateMachine machine;
    private Rigidbody2D rb;
    private Animator anim;
    private Health health;

    public Vector2 knockbackForce = new Vector2(-5f, 3f);

    private float hurtDuration = 0.25f;
    private float hurtTimer;

    [Header("Invincibility Frames")]
    [SerializeField] private float iFrameDuration = 2f;

    public HurtState(PlayerStateMachine machine)
    {
        this.machine = machine;
        rb = machine.Rb;
        anim = machine.GetComponent<Animator>();
        health = machine.GetComponent<Health>();
    }

    public void Enter()
    {
        if (anim != null)
            anim.SetBool("isHurt", true);

        hurtTimer = hurtDuration;

        if (health != null && !health.isInvincible)
        {
            rb.linearVelocity = knockbackForce;
            machine.StartCoroutine(IFrames());
        }
    }

    public void Update()
    {
        hurtTimer -= Time.deltaTime;

        if (hurtTimer <= 0f)
        {
            machine.SwitchState(new GroundedState(machine));
        }
    }

    public void FixedUpdate() { }

    public void Exit()
    {
        if (anim != null)
            anim.SetBool("isHurt", false);
    }

    private IEnumerator IFrames()
    {
        health.isInvincible = true;

        SpriteRenderer sr = machine.GetComponent<SpriteRenderer>();

        float timer = 0f;
        while (timer < iFrameDuration)
        {
            if (sr != null)
                sr.enabled = !sr.enabled;

            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        if (sr != null)
            sr.enabled = true;

        health.isInvincible = false;
    }

    public void OnCollisionEnter2D(Collision2D collision) { }
}
