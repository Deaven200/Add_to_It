using UnityEngine;

/// <summary>
/// Handles damage priority between shield and health.
/// Shield takes damage first, then health.
/// </summary>
public class PlayerDamageHandler : MonoBehaviour
{
    [Header("Damage Priority")]
    [SerializeField] private bool shieldTakesDamageFirst = true;
    [SerializeField] private bool shieldBlocksAllDamage = false; // If true, shield must be 0 before health takes damage
    
    private PlayerHealth playerHealth;
    private PlayerShield playerShield;
    private PlayerArmor playerArmor;
    
    void Start()
    {
        // Get references to health, shield, and armor components
        playerHealth = GetComponent<PlayerHealth>();
        playerShield = GetComponent<PlayerShield>();
        playerArmor = GetComponent<PlayerArmor>();
        
        if (playerHealth == null)
        {
            Debug.LogError("PlayerDamageHandler: PlayerHealth component not found!");
        }
        
        if (playerShield == null)
        {
            Debug.LogError("PlayerDamageHandler: PlayerShield component not found!");
        }
        
        if (playerArmor == null)
        {
            Debug.LogWarning("PlayerDamageHandler: PlayerArmor component not found! Armor will not be used.");
        }
    }
    
    /// <summary>
    /// Takes damage with armor -> shield -> health priority.
    /// Armor reduces damage first, then shield takes damage, then health.
    /// </summary>
    /// <param name="totalDamage">Total damage to deal</param>
    public void TakeDamage(int totalDamage)
    {
        int remainingDamage = totalDamage;
        
        // First, reduce damage with armor
        if (playerArmor != null && playerArmor.HasArmor())
        {
            remainingDamage = playerArmor.ReduceDamage(remainingDamage);
            Debug.Log($"Armor reduced damage from {totalDamage} to {remainingDamage}");
        }
        
        if (!shieldTakesDamageFirst)
        {
            // If shield doesn't take priority, just damage health
            if (playerHealth != null && remainingDamage > 0)
            {
                playerHealth.TakeDamage(remainingDamage);
            }
            return;
        }
        
        // Second, damage the shield
        if (playerShield != null && playerShield.currentShield > 0 && remainingDamage > 0)
        {
            int shieldDamage = Mathf.Min(remainingDamage, playerShield.currentShield);
            playerShield.TakeShieldDamage(shieldDamage);
            remainingDamage -= shieldDamage;
            
            Debug.Log($"Shield took {shieldDamage} damage. Remaining damage: {remainingDamage}");
        }
        
        // Finally, if there's still damage remaining and shield doesn't block all damage, damage health
        if (remainingDamage > 0 && !shieldBlocksAllDamage)
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(remainingDamage);
                Debug.Log($"Health took {remainingDamage} damage.");
            }
        }
        else if (remainingDamage > 0 && shieldBlocksAllDamage)
        {
            Debug.Log($"Shield blocked all damage. {remainingDamage} damage prevented.");
        }
    }
    
    /// <summary>
    /// Takes damage only to health (bypasses shield)
    /// </summary>
    /// <param name="damage">Damage to deal to health</param>
    public void TakeHealthDamage(int damage)
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }
    
    /// <summary>
    /// Takes damage only to shield (bypasses health)
    /// </summary>
    /// <param name="damage">Damage to deal to shield</param>
    public void TakeShieldDamage(int damage)
    {
        if (playerShield != null)
        {
            playerShield.TakeShieldDamage(damage);
        }
    }
    
    /// <summary>
    /// Heals health
    /// </summary>
    /// <param name="amount">Amount to heal</param>
    public void HealHealth(int amount)
    {
        if (playerHealth != null)
        {
            playerHealth.Heal(amount);
        }
    }
    
    /// <summary>
    /// Regenerates shield
    /// </summary>
    /// <param name="amount">Amount to regenerate</param>
    public void RegenerateShield(int amount)
    {
        if (playerShield != null)
        {
            playerShield.RegenerateShield(amount);
        }
    }
    
    /// <summary>
    /// Adds armor points
    /// </summary>
    /// <param name="amount">Amount of armor to add</param>
    public void AddArmor(int amount)
    {
        if (playerArmor != null)
        {
            playerArmor.AddArmor(amount);
        }
    }
    
    /// <summary>
    /// Sets the maximum armor capacity
    /// </summary>
    /// <param name="newMaxArmor">New maximum armor value</param>
    public void SetMaxArmor(int newMaxArmor)
    {
        if (playerArmor != null)
        {
            playerArmor.SetMaxArmor(newMaxArmor);
        }
    }
    
    /// <summary>
    /// Restores armor to maximum
    /// </summary>
    public void RestoreFullArmor()
    {
        if (playerArmor != null)
        {
            playerArmor.RestoreFullArmor();
        }
    }
    
    /// <summary>
    /// Sets whether shield takes damage first
    /// </summary>
    /// <param name="enabled">True if shield should take damage first</param>
    public void SetShieldPriority(bool enabled)
    {
        shieldTakesDamageFirst = enabled;
    }
    
    /// <summary>
    /// Sets whether shield blocks all damage (health only takes damage when shield is 0)
    /// </summary>
    /// <param name="enabled">True if shield should block all damage</param>
    public void SetShieldBlocksAllDamage(bool enabled)
    {
        shieldBlocksAllDamage = enabled;
    }
    
    // Testing methods
    [ContextMenu("Take 20 Damage (Shield Priority)")]
    public void TestTakeDamage()
    {
        TakeDamage(20);
    }
    
    [ContextMenu("Take 10 Health Damage (Bypass Shield)")]
    public void TestTakeHealthDamage()
    {
        TakeHealthDamage(10);
    }
    
    [ContextMenu("Take 10 Shield Damage (Bypass Health)")]
    public void TestTakeShieldDamage()
    {
        TakeShieldDamage(10);
    }
    
    [ContextMenu("Heal 20 Health")]
    public void TestHealHealth()
    {
        HealHealth(20);
    }
    
    [ContextMenu("Regenerate 20 Shield")]
    public void TestRegenerateShield()
    {
        RegenerateShield(20);
    }
    
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
}
