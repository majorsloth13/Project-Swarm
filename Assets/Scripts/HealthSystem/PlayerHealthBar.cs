using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthbar : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;
    private const int totalHealthUnits = 10;

    void Start()
    {
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealthbar: No player health assigned.");
            return;
        }

        if (totalHealthBar != null)
        {
            float maxRatio = playerHealth.maxHealth / totalHealthUnits;
            totalHealthBar.fillAmount = maxRatio;
        }

        UpdateHealthUI();
    }

    void Update()
    {
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (currentHealthBar != null)
        {
            float fill = playerHealth.currentHealth / playerHealth.maxHealth;
            currentHealthBar.fillAmount = fill * totalHealthBar.fillAmount;
        }
    }
}
