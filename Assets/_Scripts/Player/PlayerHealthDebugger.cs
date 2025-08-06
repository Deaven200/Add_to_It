using UnityEngine;

public class PlayerHealthDebugger : MonoBehaviour
{
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
        
        // Subscribe to health changes
        playerHealth.OnHealthChanged += OnHealthChanged;
        
        Debug.Log($"PlayerHealth Debugger Active! Current Health: {playerHealth.currentHealth}/{playerHealth.maxHealth}");
    }
    
    void OnHealthChanged(int currentHealth, int maxHealth)
    {
        Debug.Log($"Health Changed: {currentHealth}/{maxHealth} ({(float)currentHealth/maxHealth*100:F1}%)");
        
        if (currentHealth <= 0)
        {
            Debug.LogWarning("Player Health reached 0! Player should be dead.");
        }
    }
    
    void Update()
    {
        // Log current health every few seconds for debugging
        if (Time.frameCount % 300 == 0) // Every 5 seconds at 60fps
        {
            Debug.Log($"Current Health Check: {playerHealth.currentHealth}/{playerHealth.maxHealth}");
        }
    }
    
    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= OnHealthChanged;
        }
    }
} 