using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform playerTransform; // Assign the player in Inspector
    public float spawnInterval = 5f;

    public float minDistance = 10f;
    public float maxDistance = 30f;

    public EnemyStatManager statManager; // Assign this in the Inspector

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        // Random direction on XZ plane
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float distance = Random.Range(minDistance, maxDistance);
        Vector3 spawnOffset = new Vector3(randomDir.x, 0, randomDir.y) * distance;

        Vector3 spawnPosition = playerTransform.position + spawnOffset;
        spawnPosition.y = 0f; // Adjust Y if needed for ground level

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // Apply stat manager values to new enemy
        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
        if (health != null)
            health.maxHealth = statManager.health;

        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.moveSpeed = statManager.speed;
            ai.damage = statManager.damage;
        }
    }
}
