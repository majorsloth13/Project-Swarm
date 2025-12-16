using UnityEngine;

public class slashHitBox : MonoBehaviour
{
    [Header("Damage")]
    public int SlashDamage = 10;

    [Header("Target Layers")]
    public LayerMask enemyLayers;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object's layer is in the enemyLayers mask
        if (((1 << collision.gameObject.layer) & enemyLayers) == 0)
            return;

        // Try to get EnemyHealth
        EnemyHealth hp = collision.GetComponent<EnemyHealth>();
        if (hp != null)
        {
            hp.TakeDamage(SlashDamage);
        }

        
    }
}

