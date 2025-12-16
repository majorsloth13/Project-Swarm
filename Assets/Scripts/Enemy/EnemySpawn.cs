using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject[] Enemy;
    public Transform[] EnemyPortal;

    [Header("Spawn Settings")]
    public float delay = 1f;        // Delay between spawns
    public int enemiesPerRound = 5; // How many enemies per round
    public float timeBetweenRounds = 3f; // Delay between rounds

    public int currentRound = 1;
    private bool roundInProgress = false;
    private int enemiesSpawned = 0;
    public CardChooseUI machine;
    public KillCounter countkill;
    public LevelCounter levelcounter;

    //private int resetKillCounter;
    private void Start()
    {
        StartCoroutine(RoundRoutine());
    }

    IEnumerator RoundRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(StartRound(currentRound));
            // Wait until all enemies are dead
            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 1);

            Debug.Log("Round " + currentRound + " complete!");

            currentRound++;
            enemiesPerRound += 2; // optional: increase enemies per round
            yield return new WaitForSeconds(timeBetweenRounds);
        }
    }

    IEnumerator StartRound(int round)
    {
        if (machine == null)
        {
            Debug.LogError("machine is null in EnemySpawn!");
            yield break;
        }
        levelcounter.AddLevel();
        countkill.ResetKills();
        machine.StartChooseUI();
        machine.Scanning();
        Debug.Log("Starting Round " + round);
        enemiesSpawned = 0;
        roundInProgress = true;

        while (enemiesSpawned < enemiesPerRound)
        {
            SpawnEnemy();
            enemiesSpawned++;
            yield return new WaitForSeconds(delay);
        }

        roundInProgress = false;
    }

    void SpawnEnemy()
    {
        if (Enemy.Length == 0 || EnemyPortal.Length == 0)
        {
            Debug.LogWarning("No enemies or spawn points assigned!");
            return;
        }

        GameObject enemyPrefab = Enemy[Random.Range(0, Enemy.Length)];
        Transform spawnPoint = EnemyPortal[Random.Range(0, EnemyPortal.Length)];

        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log("Spawned enemy at " + spawnPoint.name);
    }
}
