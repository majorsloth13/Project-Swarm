using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform player;
    
    private Health playerHealth;
    private Vector3 respawnPoint;

    

    [Header("Settings")]
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private float idleTransitionDelay = 1.2f;
    [SerializeField] private string deathBoxTag = "Deathbox";

    private void Start()
    {
        
        if (player == null)
        {
            Debug.LogWarning("Player not assigned to RespawnManager!");
            return;
        }

        respawnPoint = transform.position; // Always respawn at THIS object
        playerHealth = player.GetComponent<Health>();
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnCoroutine());
        
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        // Move player to RespawnManager's position
        player.position = respawnPoint + Vector3.up * 0.5f;

        // Reset health if applicable
        if (playerHealth != null)
            playerHealth.ResetHealth();

        // Re-enable movement
        PlayerStateMachine movement = player.GetComponent<PlayerStateMachine>();
        if (movement != null)
            movement.enabled = true;

        // Trigger animation
        //Animator anim = player.GetComponent<Animator>();
        //if (anim != null)
        //{
        //    anim.SetBool("isWalking", true);
        //    //yield return new WaitForSeconds(idleTransitionDelay);
        //    //anim.SetTrigger("idle");
        //}

        Debug.Log("Player respawned at RespawnManager position.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(deathBoxTag))
        {
            RespawnPlayer();
        }
    }
}
