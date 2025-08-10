using UnityEngine;

public class HealthBarTester : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private int healAmount = 20;
    
    private PlayerHealth playerHealth;
    
    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth component not found!");
            enabled = false;
            return;
        }
        
        Debug.Log("Health Bar Tester Active!");
        Debug.Log("Press 'H' to heal, 'J' to take damage, 'K' to set max health to 200");
    }
    
    void Update()
    {
        if (playerHealth == null) return;
        
        // Test healing
        if (Input.GetKeyDown(KeyCode.H))
        {
            playerHealth.Heal(healAmount);
            Debug.Log($"Healed {healAmount} health");
        }
        
        // Test damage
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerHealth.TakeDamage(damageAmount);
            Debug.Log($"Took {damageAmount} damage");
        }
        
        // Test max health change
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerHealth.SetMaxHealth(200);
            Debug.Log("Max health set to 200");
        }
    }
} 