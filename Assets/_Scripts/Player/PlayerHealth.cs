using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    
    // Event that gets called when health changes
    public event Action<int, int> OnHealthChanged;

    void Start()
    {
        // Use the inspector values as the source of truth
        // Only load from UIManager if we don't have inspector values set
        if (maxHealth <= 0)
        {
            // If inspector maxHealth is not set, load from UIManager
            if (UIManager.Instance != null)
            {
                currentHealth = UIManager.Instance.GetPlayerHealth();
                maxHealth = UIManager.Instance.GetMaxPlayerHealth();
            }
            else
            {
                maxHealth = 100;
                currentHealth = maxHealth;
            }
        }
        else
        {
            // Use inspector values, but ensure currentHealth is valid
            if (currentHealth <= 0 || currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
        
        // Notify UI of initial health
        
        // Notify UI of initial health
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // Update UIManager with our health values (this will save them)
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetPlayerHealth(currentHealth, maxHealth);
        }
    }

    public void RestoreFullHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // Update UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetPlayerHealth(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth); // Ensure health doesn't go below 0
        
        // Notify UI of health change
        
        // Notify UI of health change
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // Update UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetPlayerHealth(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Ensure health doesn't exceed max
        
        // Notify UI of health change
        
        // Notify UI of health change
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // Update UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetPlayerHealth(currentHealth, maxHealth);
        }
    }
    
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Adjust current health if needed
        
        // Notify UI of health change
        
        // Notify UI of health change
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // Update UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetPlayerHealth(currentHealth, maxHealth);
        }
    }

    void Die()
    {
        // Show death screen using UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowDeathScreen();
        }
        
        // Make sure to disable player controls or destroy the player object to prevent moving while dead.
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }
        
        // Show death screen using UIManager
    }
    
    // Testing methods for changing max health
    [ContextMenu("Set Max Health to 150")]
    public void TestSetMaxHealth150()
    {
        SetMaxHealth(150);
    }
    
    [ContextMenu("Set Max Health to 200")]
    public void TestSetMaxHealth200()
    {
        SetMaxHealth(200);
    }
    
    [ContextMenu("Set Max Health to 50")]
    public void TestSetMaxHealth50()
    {
        SetMaxHealth(50);
    }
    
    [ContextMenu("Take 20 Damage")]
    public void TestTakeDamage()
    {
        TakeDamage(20);
    }
    
    [ContextMenu("Heal 30 Health")]
    public void TestHeal()
    {
        Heal(30);
    }
    
    [ContextMenu("Clear Saved Health Data")]
    public void ClearSavedHealthData()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ResetPlayerData();
        }
        Debug.Log("PlayerHealth: Cleared saved health data");
    }
}