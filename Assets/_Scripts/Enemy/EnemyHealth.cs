using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 10f;
    private float currentHealth;

    public GameObject coinPrefab; // ← assign this in the Inspector

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
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
