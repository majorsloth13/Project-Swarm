using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 mousePos;
    private Camera mainCam;
    private Rigidbody2D rb;
    public float bulletSpeed;
    public int bulletDamage;
    public LayerMask LayerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 bulletAim = mousePos - transform.position;
        Vector3 rotation = transform.position - mousePos;
        rb.linearVelocity = new Vector2(bulletAim.x, bulletAim.y).normalized * bulletSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;

        // Direction toward click
        Vector3 direction = (mousePos - transform.position).normalized;

        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Replace CompareLayer with layer mask check
        if ((LayerMask.value & (1 << collision.gameObject.layer)) == 0)
            return;

        EnemyHealth hp = collision.GetComponent<EnemyHealth>();
        if (hp != null)
            hp.TakeDamage(bulletDamage);

        Destroy(gameObject);
        
    }

}
