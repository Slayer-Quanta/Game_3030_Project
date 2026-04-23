using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs; 
    [SerializeField] private int enemyCount = 5;
    [SerializeField] private Border spawnHorizontalBounds;
    [SerializeField] private Border spawnVerticalBounds;

    void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("No enemy prefabs assigned to EnemySpawner");
            return;
        }

        for (int i = 0; i < enemyCount; i++)
        {
         Vector3 spawnPosition = new Vector3(Random.Range(spawnHorizontalBounds.Min, spawnHorizontalBounds.Max), Random.Range(spawnVerticalBounds.Min, spawnVerticalBounds.Max), -1);
         GameObject randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(randomEnemy, spawnPosition, Quaternion.identity);
        }
    }
}