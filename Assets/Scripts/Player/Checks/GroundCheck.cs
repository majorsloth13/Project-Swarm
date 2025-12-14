using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header("Ground Check Settings")]
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayers;

    public bool ignoreGroundCheck = false;


    public bool IsGrounded()
    {
        if (ignoreGroundCheck)
            return false;
        
        return Physics2D.OverlapCircle(transform.position, checkRadius, groundLayers);
    }

    public Collider2D GetGroundCollider()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, checkRadius, groundLayers);

        if (hitColliders.Length > 0)
        {
            // Return the first collider found
            return hitColliders[0];
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
