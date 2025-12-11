using UnityEngine;
using Vuforia;

public class JackCard : MonoBehaviour
{
    private ObserverBehaviour observer;

    [Header("Assign Player")]
    public PlayerStateMachine player;

    private GroundPoundUnlock unlock;

    private void Awake()
    {
        observer = GetComponent<ObserverBehaviour>();
        unlock = player.GetComponent<GroundPoundUnlock>();

        if (observer != null)
            observer.OnTargetStatusChanged += OnStatusChanged;
    }

    private void OnDestroy()
    {
        if (observer != null)
            observer.OnTargetStatusChanged -= OnStatusChanged;
    }

    private void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        bool isFound = status.Status == Status.TRACKED ||
                       status.Status == Status.EXTENDED_TRACKED;

        if (isFound)
        {
            unlock.Unlock();
        }
    }
}
