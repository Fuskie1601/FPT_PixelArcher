using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[System.Serializable]
public struct SummonEnemy
{
    [CanBeNull] public GameObject enemyPrefab;
    [Range(0, 1)]
    public float weight;
}

public class MiniSpawner : MonoBehaviour
{
    public List<SummonEnemy> enemiesToSpawn;
    public List<Transform> spawnPoints;
    public UnityEvent WaveCleared;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private float totalWeight, randomValue, cumulativeWeight;

    private void Start()
    {
        SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            // Check if spawn point is valid
            Vector3 spawnPosition = GetValidSpawnPosition(spawnPoint.position);
            if (spawnPosition != Vector3.zero)
            {
                // Choose an enemy to spawn based on weight
                GameObject selectedEnemy = GetRandomEnemy();
                if (selectedEnemy != null)
                {
                    GameObject spawnedEnemy = Instantiate(selectedEnemy, spawnPosition, Quaternion.identity, transform);
                    spawnedEnemies.Add(spawnedEnemy);

                    // Set up a listener to detect when the enemy is destroyed
                    spawnedEnemy.AddComponent<EnemyDestructHandler>().OnDestroyed += HandleEnemyDestroyed;
                }
            }
        }

        // Check if no enemies were spawned
        if (spawnedEnemies.Count == 0)
        {
            WaveCleared?.Invoke();
        }
    }

    private Vector3 GetValidSpawnPosition(Vector3 center)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(center, out hit, 1.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return Vector3.zero;
    }

    private GameObject GetRandomEnemy()
    {
        totalWeight = 0f;
        foreach (var enemy in enemiesToSpawn)
        {
            totalWeight += enemy.weight;
        }

        randomValue = Random.value * totalWeight;
        cumulativeWeight = 0f;

        foreach (var enemy in enemiesToSpawn)
        {
            cumulativeWeight += enemy.weight;
            if (randomValue <= cumulativeWeight && enemy.enemyPrefab != null)
                return enemy.enemyPrefab;
        }
        return null;
    }

    private void HandleEnemyDestroyed(GameObject enemy)
    {
        if (spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Remove(enemy);

            // Check if all enemies are destroyed
            if (spawnedEnemies.Count == 0)
            {
                WaveCleared?.Invoke();
            }
        }
    }
}
