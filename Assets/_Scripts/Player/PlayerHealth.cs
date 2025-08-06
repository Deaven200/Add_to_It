using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public void RestoreFullHealth()
{
    currentHealth = maxHealth;
    OnHealthChanged?.Invoke(currentHealth, maxHealth);
}

    [SerializeField] private DeathScreenUI deathScreenUI;
    
    // Event that gets called when health changes
    public event Action<int, int> OnHealthChanged;

    void Start()
    {
        currentHealth = maxHealth;
        // Notify UI of initial health
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth); // Ensure health doesn't go below 0
        
        Debug.Log("Player took damage. Health: " + currentHealth);
        
        // Notify UI of health change
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Ensure health doesn't exceed max
        
        Debug.Log("Player healed. Health: " + currentHealth);
        
        // Notify UI of health change
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Adjust current health if needed
        
        Debug.Log("Max health set to: " + maxHealth);
        
        // Notify UI of health change
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Die()
    {
        // Check if we have a persistent player system
        PersistentPlayer persistentPlayer = GetComponent<PersistentPlayer>();
        
        if (persistentPlayer != null)
        {
            // Use persistent player respawn system
            persistentPlayer.OnPlayerDeath();
        }
        else
        {
            // Fall back to old death screen system
            if (deathScreenUI != null)
            {
                deathScreenUI.ShowDeathScreen();
            }
            
            // Make sure to disable player controls or destroy the player object to prevent moving while dead.
            PlayerMovement movement = GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.enabled = false;
            }
        }
    }
}