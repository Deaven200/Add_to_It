using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 10f;
    private float currentHealth;

    public GameObject coinPrefab; // ← assign this in the Inspector
    
    [Header("Enemy Info")]
    public string enemyName = "Enemy"; // Name for debug purposes
    
    [Header("Individual Drop Settings")]
    public bool useIndividualDropRate = false;
    [Range(0f, 100f)]
    public float individualDropRate = 10f;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Spawn coin
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
        
        // Handle chest drop
        HandleChestDrop();
        
        // Destroy the enemy
        Destroy(gameObject);
    }
    
    void HandleChestDrop()
    {
        // Check if EnemyDropManager exists
        if (EnemyDropManager.Instance != null)
        {
            // Use individual drop rate if enabled, otherwise use global rate
            if (useIndividualDropRate)
            {
                // Temporarily override global drop rate for this enemy
                float originalRate = EnemyDropManager.Instance.GetGlobalDropRate();
                EnemyDropManager.Instance.SetGlobalDropRate(individualDropRate);
                EnemyDropManager.Instance.OnEnemyDeath(transform.position, enemyName);
                EnemyDropManager.Instance.SetGlobalDropRate(originalRate);
            }
            else
            {
                // Use global drop rate
                EnemyDropManager.Instance.OnEnemyDeath(transform.position, enemyName);
            }
        }
        else
        {
            Debug.LogWarning("EnemyDropManager not found! Chest drops will not work.");
        }
    }
}
