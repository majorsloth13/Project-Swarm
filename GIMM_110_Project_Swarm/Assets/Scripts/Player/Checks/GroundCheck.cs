using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header("Ground Check Settings")]
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayers;

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(transform.position, checkRadius, groundLayers);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
