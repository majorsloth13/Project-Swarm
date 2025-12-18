using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBox : MonoBehaviour
{
    [SerializeField] private float damageAmount = 999f;
    [SerializeField] private Color gizmoColor = new Color(1f, 0f, 0f, 0.3f); // Semi-transparent red

    private bool hasHitPlayer;

    private void OnEnable()
    {
        // Reset when slashbox activates
        hasHitPlayer = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (hasHitPlayer)
            return;

        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                hasHitPlayer = true; // prevent multi-hits
            }
        }
    }



private void OnDrawGizmos()
    {
        // Set color for visualization in the editor
        Gizmos.color = gizmoColor;

        // Try to match collider bounds (works for BoxCollider2D and CircleCollider2D)
        Collider2D col = GetComponent<Collider2D>();
        if (col is BoxCollider2D box)
        {
            // Draw a filled box that matches the trigger’s size and position
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.offset, box.size);
            Gizmos.color = Color.red; // Draw outline in red
            Gizmos.DrawWireCube(box.offset, box.size);
        }
        else if (col is CircleCollider2D circle)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(circle.offset, circle.radius);
        }
    }
}
