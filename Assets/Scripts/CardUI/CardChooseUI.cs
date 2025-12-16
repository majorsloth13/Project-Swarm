using JetBrains.Annotations;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CardChooseUI : MonoBehaviour
{
    public GameObject cardUi;
    public Camera ARCam;
    public PlayerStateMachine machine;

    public GameObject slot1UI; // UI element for first scanned card
    public GameObject slot2UI; // UI element for second scanned card

    void Start()
    {
        StartChooseUI();
        
    }

    void Update()
    {

        Scanning();
        //if (machine == null)
        //{
        //    Debug.Log("machine is null in cardchooseUI");
        //    return;
        //}

        //// Update first slot UI
        ////UpdateSlotUI(machine.powerUpSlots[0], slot1UI);

        ////// Update second slot UI
        ////UpdateSlotUI(machine.powerUpSlots[1], slot2UI);

        //// Close UI when both slots are filled
        //if (machine.powerUpSlots[0] != PlayerStateMachine.ScannedCard.None && machine.powerUpSlots[1] != PlayerStateMachine.ScannedCard.None)
        //{
        //    Debug.Log("both cards were scanned properly");
        //    cardUi.SetActive(false);
        //    Time.timeScale = 1f;
        //    if (ARCam != null)
        //        ARCam.enabled = false;
        //}
    }

    //void UpdateSlotUI(PlayerStateMachine.ScannedCard card, GameObject slotUI)
    //{
    //    if (slotUI == null) return;

    //    // You can customize this to show a card image
    //    slotUI.SetActive(card != PlayerStateMachine.ScannedCard.None);
    //    slotUI.GetComponentInChildren<UnityEngine.UI.Text>().text = card.ToString();
    //}
   
    public void StartChooseUI()
    {
        cardUi.SetActive(true);
        Time.timeScale = 0f;
        if (ARCam != null)
            ARCam.enabled = true;
        machine.powerUpSlots[0] = PlayerStateMachine.ScannedCard.None;
        machine.powerUpSlots[1] = PlayerStateMachine.ScannedCard.None;
    }

    public void Scanning()
    {
        if (machine == null)
        {
            Debug.Log("machine is null in cardchooseUI");
            return;
        }

        // Update first slot UI
        //UpdateSlotUI(machine.powerUpSlots[0], slot1UI);

        //// Update second slot UI
        //UpdateSlotUI(machine.powerUpSlots[1], slot2UI);

        // Close UI when both slots are filled
        if (machine.powerUpSlots[0] != PlayerStateMachine.ScannedCard.None && machine.powerUpSlots[1] != PlayerStateMachine.ScannedCard.None)
        {
            Debug.Log("both cards were scanned properly");
            cardUi.SetActive(false);
            Time.timeScale = 1f;
            if (ARCam != null)
                ARCam.enabled = false;
        }
    }
}










