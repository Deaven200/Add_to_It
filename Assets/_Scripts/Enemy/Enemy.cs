using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private string enemyName = "Enemy";
    
    [Header("Individual Drop Settings")]
    [SerializeField] private bool useIndividualDropRate = false;
    [Range(0f, 100f)]
    [SerializeField] private float individualDropRate = 10f;
    
    private float currentHealth;
    
    void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    
    private void Die()
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
        
        // Destroy the enemy
        Destroy(gameObject);
    }
    
    // Method to test damage (for debugging)
    [ContextMenu("Test Damage")]
    public void TestDamage()
    {
        TakeDamage(50f);
    }
    
    // Method to test instant death
    [ContextMenu("Test Death")]
    public void TestDeath()
    {
        Die();
    }
}
