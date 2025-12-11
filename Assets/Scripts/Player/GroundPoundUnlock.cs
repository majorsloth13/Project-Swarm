using UnityEngine;

public class GroundPoundUnlock : MonoBehaviour
{
    public bool CanGroundPound { get; private set; } = false;
    public bool HasUsedGroundPound { get; private set; } = false;

    // The unlock function JackCard is expecting
    public void Unlock()
    {
        if (!HasUsedGroundPound)
        {
            CanGroundPound = true;
            Debug.Log("Ground Pound Unlocked!");
        }
    }

    // Called when the ground pound is actually used
    public void Consume()
    {
        CanGroundPound = false;
        HasUsedGroundPound = true;
        Debug.Log("Ground Pound Used (One-time ability)");
    }
}
