using UnityEngine;

public class ToggleOnTracking : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    public PlayerStateMachine player;
    public PlayerStateMachine.ScannedCard cardType;   // assign in inspector

    public void OnTargetFound()
    {
        if (targetObject != null)
            targetObject.SetActive(true);
        
        if (player != null)
            player.AssignScannedCardToSlot(cardType);
        

        Debug.Log("Card Scanned: " + cardType);
    }

    public void OnTargetLost()
    {
        if (targetObject != null)
            targetObject.SetActive(false);

        //player.scannedCard = PlayerStateMachine.ScannedCard.None;
    }
}


