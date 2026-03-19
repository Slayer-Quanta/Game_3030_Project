using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public float spawnInterval = 3f;
    public int maxEnemiesInScene = 10;

    void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned to EnemySpawner");
            return;
        }

        if (enemyPrefabs.Length == 0)
        {
            Debug.LogError("No enemy prefabs assigned to EnemySpawner");
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

            if (currentEnemies < maxEnemiesInScene)
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
        Transform selectedSpawnPoint = spawnPoints[randomSpawnIndex];

        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject selectedEnemyPrefab = enemyPrefabs[randomEnemyIndex];

        Instantiate(selectedEnemyPrefab, selectedSpawnPoint.position, selectedSpawnPoint.rotation);
    }
}