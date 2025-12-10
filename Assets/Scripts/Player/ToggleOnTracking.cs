using UnityEngine;

public class ToggleOnTracking : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;

    public void OnTargetFound()
    {
        if (targetObject != null)
            targetObject.SetActive(true);
    }

    public void OnTargetLost()
    {
        if (targetObject != null)
            targetObject.SetActive(false);
    }
}
