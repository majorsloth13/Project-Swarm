/*using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    #region Variables
    // Array setup for platforms
    public GameObject[] Enemy;
    public Transform[] EnemyPortal;
    public GameObject currentEnemyPortal;
    public GameObject randomItems;
    private float delay = 2f;
    public int Count;
    private bool SpawnEnemyBool = true;

    #endregion

    #region Unity Methods
    private void Start()
    {
        GetItems();
        StartCoroutine(SpawnRoutine());

    }
    #endregion

    #region Custom Methods

    /// Sets the platforms array to all objects with the tag "ItemPlatform".

    private void Update()
    {
        
    }

    public void GetItems()
    {

        Enemy = GameObject.FindGameObjectsWithTag("Enemy");//creates an array of all objects with the items tag
        Debug.Log("found item");



    }

    IEnumerator SpawnRoutine()
    {
        while (SpawnEnemyBool)
        {
            SpawnEnemy();
            Count++;
            yield return new WaitForSeconds(delay);

            if(Count == 35)
            {
                SpawnEnemyBool = false;
            }
        }
    }




    void SpawnEnemy()
    {
        if (Enemy.Length == 0 || EnemyPortal.Length == 0)
        {
            Debug.LogWarning("No enemies or spawn points assigned!");
            return;
        }

        // choose random enemy
        GameObject enemyPrefab = Enemy[Random.Range(0, Enemy.Length)];

        // choose random spawn point
        Transform spawnPoint = EnemyPortal[Random.Range(0, EnemyPortal.Length)];

        // spawn enemy
        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        Debug.Log("Spawned enemy at " + spawnPoint.name);
    }
}
#endregion*/
