using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform playerTransform;

    public int startingEnemiesPerWave = 10;
    public int increasePerWave = 3;
    public float minDistance = 10f;
    public float maxDistance = 30f;
    public float timeBetweenWaves = 5f;

    public EnemyStatManager statManager;

    private int currentWave = 0;
    private List<GameObject> currentEnemies = new List<GameObject>();
    private bool waveInProgress = false;

    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    void Update()
    {
        // Clean up dead enemies
        currentEnemies.RemoveAll(enemy => enemy == null);

        if (waveInProgress && currentEnemies.Count == 0)
        {
            waveInProgress = false;
            StartCoroutine(StartNextWaveAfterDelay());
        }
    }

    IEnumerator StartNextWaveAfterDelay()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        waveInProgress = true;
        currentWave++;

        int enemiesToSpawn = startingEnemiesPerWave + (increasePerWave * (currentWave - 1));

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float distance = Random.Range(minDistance, maxDistance);
            Vector3 spawnOffset = new Vector3(randomDir.x, 0, randomDir.y) * distance;
            Vector3 spawnPosition = playerTransform.position + spawnOffset;
            spawnPosition.y = 0f;

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            currentEnemies.Add(enemy);

            // Apply stat manager values
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
                health.maxHealth = statManager.health;

            EnemyAI ai = enemy.GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.moveSpeed = statManager.speed;
                ai.damage = statManager.damage;
            }

            yield return null; // Optional: delay between enemy spawns
        }
    }
}
