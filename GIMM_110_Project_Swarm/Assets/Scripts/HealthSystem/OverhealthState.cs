using System.Collections;
using UnityEngine;

public class OverhealthState : MonoBehaviour
{
    [Header("Overhealth Settings")]
    [SerializeField] private float overhealthAmount = 50f;
    [SerializeField] private float overhealthDuration = 5f;
    [SerializeField] private float decaySpeed = 20f; // Health per second

    private Health playerHealth;
    private bool isActive = false;
    private float baselineHealth;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        if (playerHealth == null)
        {
            Debug.LogError("OverhealthState requires a Health component on the same GameObject.");
        }
    }

    private void Update()
    {
        // Press 1 to test overhealth
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TriggerOverhealth();
        }
    }

    public void TriggerOverhealth()
    {
        if (playerHealth == null || isActive) return;

        StartCoroutine(OverhealthRoutine());
    }

    private IEnumerator OverhealthRoutine()
    {
        isActive = true;

        // Store baseline health
        baselineHealth = playerHealth.currentHealth;

        // Apply overhealth
        float targetHealth = baselineHealth + overhealthAmount;
        playerHealth.SetTemporaryHealth(targetHealth);
        Debug.Log($"Overhealth applied. Target health: {targetHealth}");

        // Wait for duration
        yield return new WaitForSeconds(overhealthDuration);

        // Gradually decay to baseline
        while (playerHealth.currentHealth > baselineHealth)
        {
            playerHealth.SetTemporaryHealth(Mathf.Max(baselineHealth, playerHealth.currentHealth - decaySpeed * Time.deltaTime));
            Debug.Log($"Overhealth decaying. Current health: {playerHealth.currentHealth}");
            yield return null;
        }

        Debug.Log($"Overhealth ended. Final health: {playerHealth.currentHealth}");
        isActive = false;
    }
}
