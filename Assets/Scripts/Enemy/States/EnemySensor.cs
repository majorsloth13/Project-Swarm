using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    [System.Flags]
    public enum DirectionMask
    {
        None = 0,
        Up = 1 << 0,
        Down = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3
    }

    [Header("Directional Detection")]
    public DirectionMask detectDirections = DirectionMask.Left;
    public float detectRange = 3f;
    public LayerMask playerLayer;

    [Header("Detection Results")]
    public bool playerDetected;
    public Transform playerRef;

    void Update()
    {
        playerDetected = false;
        playerRef = null;

        // Check every direction bit
        TryDirection(DirectionMask.Up, Vector2.up);
        TryDirection(DirectionMask.Down, Vector2.down);
        TryDirection(DirectionMask.Left, Vector2.left);
        TryDirection(DirectionMask.Right, Vector2.right);
    }

    private void TryDirection(DirectionMask mask, Vector2 dir)
    {
        if ((detectDirections & mask) == 0)
            return; // this direction is not enabled

        var hit = Physics2D.Raycast(transform.position, dir, detectRange, playerLayer);

        Debug.DrawRay(transform.position, dir * detectRange, hit.collider ? Color.green : Color.red);

        if (hit.collider != null)
        {
            playerDetected = true;
            playerRef = hit.collider.transform;
        }
    }
}
