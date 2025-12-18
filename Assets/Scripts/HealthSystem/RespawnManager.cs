using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform player;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    private Health playerHealth;

    [Header("Settings")]
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private float idleTransitionDelay = 1.2f;
    //[SerializeField] private string deathBoxTag = "Deathbox";

    private void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("Player not assigned to RespawnManager!");
            return;
        }

        playerHealth = player.GetComponent<Health>();

        if (spawnPoints.Length == 0)
            Debug.LogWarning("No spawn points assigned to RespawnManager!");

    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnCoroutine());
    }

    public IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        // Pick a random spawn point
        int index = Random.Range(0, spawnPoints.Length);
        Transform chosen = spawnPoints[index];

        // Choose a random spawn point
        //Transform chosen = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Debug.Log($"Respawning at: {chosen.name}  ({chosen.position})");

        //Offset upward to prevent ground clipping
        player.position = chosen.position + Vector3.up * 0.5f;

        // Reset health if applicable
        if (playerHealth != null)
            playerHealth.ResetHealth();

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Re-enable movement
        PlayerStateMachine movement = player.GetComponent<PlayerStateMachine>();
        if (movement != null)
            movement.enabled = true;

        // Trigger animation
        /*Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("respawn");
            yield return new WaitForSeconds(idleTransitionDelay);
            anim.SetTrigger("idle");
        }*/
        
        Debug.Log("Player respawned at RespawnManager position.");
    }

    /*" private void OnTriggerEnter2D(Collider2D collision)
     {
         if (collision.CompareTag(deathBoxTag))
         {
             RespawnPlayer();
         }
     }*/
}
