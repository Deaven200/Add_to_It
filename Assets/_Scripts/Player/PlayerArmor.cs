using UnityEngine;
using System;
using TMPro;

/// <summary>
/// Manages player armor that reduces incoming damage.
/// Armor blocks damage before it reaches shield/health.
/// </summary>
public class PlayerArmor : MonoBehaviour
{
    [Header("Armor Settings")]
    public int maxArmor = 0;
    public int currentArmor = 0;
    
    [Header("Armor UI")]
    [SerializeField] private bool showArmorUI = true;
    [SerializeField] private TextMeshProUGUI armorText;
    
    // Event that gets called when armor changes
    public event Action<int, int> OnArmorChanged;

    void Start()
    {
        // Initialize armor values
        if (maxArmor < 0)
        {
            maxArmor = 0;
        }
        
        if (currentArmor < 0 || currentArmor > maxArmor)
        {
            currentArmor = maxArmor;
        }
        
        // Update UI
        UpdateArmorUI();
        
        // Notify UI of initial armor
        OnArmorChanged?.Invoke(currentArmor, maxArmor);
    }
    
    /// <summary>
    /// Reduces incoming damage by armor amount.
    /// Returns the remaining damage after armor reduction.
    /// Armor is permanent and doesn't get consumed.
    /// </summary>
    /// <param name="incomingDamage">The original damage amount</param>
    /// <returns>The damage after armor reduction</returns>
    public int ReduceDamage(int incomingDamage)
    {
        if (currentArmor <= 0)
        {
            return incomingDamage; // No armor, take full damage
        }
        
        int damageAfterArmor = Mathf.Max(0, incomingDamage - currentArmor);
        
        // Armor is permanent and doesn't get consumed
        Debug.Log($"Armor blocked {currentArmor} damage. Remaining damage: {damageAfterArmor}");
        
        return damageAfterArmor;
    }
    
    /// <summary>
    /// Adds armor points
    /// </summary>
    /// <param name="amount">Amount of armor to add</param>
    public void AddArmor(int amount)
    {
        currentArmor += amount;
        currentArmor = Mathf.Min(currentArmor, maxArmor);
        
        // Notify UI of armor change
        OnArmorChanged?.Invoke(currentArmor, maxArmor);
        UpdateArmorUI();
    }
    
    /// <summary>
    /// Sets the maximum armor capacity
    /// </summary>
    /// <param name="newMaxArmor">New maximum armor value</param>
    public void SetMaxArmor(int newMaxArmor)
    {
        maxArmor = Mathf.Max(0, newMaxArmor);
        currentArmor = Mathf.Min(currentArmor, maxArmor);
        
        // Notify UI of armor change
        OnArmorChanged?.Invoke(currentArmor, maxArmor);
        UpdateArmorUI();
    }
    
    /// <summary>
    /// Restores armor to maximum
    /// </summary>
    public void RestoreFullArmor()
    {
        currentArmor = maxArmor;
        
        // Notify UI of armor change
        OnArmorChanged?.Invoke(currentArmor, maxArmor);
        UpdateArmorUI();
    }
    
    /// <summary>
    /// Gets the current armor value
    /// </summary>
    /// <returns>Current armor points</returns>
    public int GetCurrentArmor()
    {
        return currentArmor;
    }
    
    /// <summary>
    /// Gets the maximum armor value
    /// </summary>
    /// <returns>Maximum armor points</returns>
    public int GetMaxArmor()
    {
        return maxArmor;
    }
    
    /// <summary>
    /// Checks if player has any armor
    /// </summary>
    /// <returns>True if armor > 0</returns>
    public bool HasArmor()
    {
        return currentArmor > 0;
    }
    
    private void UpdateArmorUI()
    {
        if (showArmorUI && armorText != null)
        {
            armorText.text = currentArmor.ToString();
        }
    }
    
    // Testing methods
    [ContextMenu("Add 5 Armor")]
    public void TestAddArmor()
    {
        AddArmor(5);
    }
    
    [ContextMenu("Set Max Armor to 10")]
    public void TestSetMaxArmor10()
    {
        SetMaxArmor(10);
    }
    
    [ContextMenu("Restore Full Armor")]
    public void TestRestoreFullArmor()
    {
        RestoreFullArmor();
    }
    
    [ContextMenu("Test Damage Reduction (5 damage)")]
    public void TestDamageReduction()
    {
        int originalDamage = 5;
        int reducedDamage = ReduceDamage(originalDamage);
        Debug.Log($"Original damage: {originalDamage}, After armor: {reducedDamage}");
    }
}
