using UnityEngine;
using TMPro;

public class KillCounter : MonoBehaviour
{
    public static KillCounter Instance;

    [SerializeField] private TMP_Text killText;

    public int killCount = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void AddKill()
    {
        killCount++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (killText != null)
        {
            killText.text = "Kills: " + killCount;
        }
    }
}
