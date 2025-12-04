using UnityEngine;

public class EnemyHealth : HealthBase
{
    private EnemyHealthBar bar;
    private SpriteRenderer sr;
    private Color originalColor;
    private Color flashColor = new Color32(0xAC, 0x00, 0x00, 0xFF);
    private bool flashing = false;

    public System.Action OnEnemyDied;

    protected override void Awake()
    {
        base.Awake();

        // Find health bar even if canvas is inactive
        bar = GetComponentInChildren<EnemyHealthBar>(true);
        Debug.Log("EnemyHealth Awake() — healthBar reference = " + bar);

        sr = GetComponentInChildren<SpriteRenderer>();
        originalColor = sr.color;

        if (bar != null)
        {
            bar.Initialize(this);
            bar.gameObject.SetActive(false); // keep it hidden until damaged
        }
        else
        {
            Debug.LogError("EnemyHealthBar component missing on prefab!");
        }
    }

    public override void TakeDamage(float dmg)
    {
        Debug.Log("EnemyHealth.TakeDamage() CALLED on " + gameObject.name);

        base.TakeDamage(dmg);

        if (bar != null)
        {
            // Show the bar if it’s not already active
            if (!bar.gameObject.activeSelf)
            {
                bar.gameObject.SetActive(true);
                Debug.Log("EnemyHealthBar SHOWING for " + gameObject.name);
            }

            bar.UpdateHealth(currentHealth);
        }

        Flash();
    }

    protected override void Die()
    {
        if (KillCounter.Instance != null)
            KillCounter.Instance.AddKill();

        if (bar != null)
        {
            bar.gameObject.SetActive(false); // hide bar on death if you want
            Debug.Log("EnemyHealthBar HIDDEN because enemy died");
        }

        Destroy(gameObject);
    }

    private void Flash()
    {
        if (!flashing)
            StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        flashing = true;
        sr.color = flashColor;
        yield return new WaitForSeconds(0.08f);
        sr.color = originalColor;
        flashing = false;
    }
}
