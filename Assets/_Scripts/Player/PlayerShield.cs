using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerShield : MonoBehaviour
{
    public int maxShield = 50;
    public int currentShield;
    
    [Header("Shield Regeneration")]
    [SerializeField] private bool autoRegenerate = true;
    [SerializeField] private float regenerationInterval = 2f; // Time between regeneration ticks
    [SerializeField] private int regenerationAmount = 5; // Amount regenerated per tick
    [SerializeField] private float regenerationDelay = 3f; // Delay before regeneration starts after taking damage
    
    private float lastDamageTime;
    private float nextRegenerationTime;
    
    // Event that gets called when shield changes
    public event Action<int, int> OnShieldChanged;

    void Start()
    {
        // Initialize shield values - respect the inspector values
        if (maxShield < 0)
        {
            maxShield = 0;
        }
        
        // Only set current shield to max if it's invalid (negative or exceeds max)
        if (currentShield < 0 || currentShield > maxShield)
        {
            currentShield = maxShield;
        }
        
        // Initialize regeneration timing
        lastDamageTime = -regenerationDelay; // Start regenerating immediately
        nextRegenerationTime = Time.time + regenerationInterval;
        
        // Notify UI of initial shield
        OnShieldChanged?.Invoke(currentShield, maxShield);
    }
    
    void Update()
    {
        // Handle automatic shield regeneration
        if (autoRegenerate && currentShield < maxShield)
        {
            // Check if enough time has passed since last damage
            if (Time.time >= lastDamageTime + regenerationDelay)
            {
                // Check if it's time for the next regeneration tick
                if (Time.time >= nextRegenerationTime)
                {
                    RegenerateShield(regenerationAmount);
                    nextRegenerationTime = Time.time + regenerationInterval;
                }
            }
        }
    }

    public void RestoreFullShield()
    {
        currentShield = maxShield;
        OnShieldChanged?.Invoke(currentShield, maxShield);
    }

    public void TakeShieldDamage(int amount)
    {
        currentShield -= amount;
        currentShield = Mathf.Max(0, currentShield); // Ensure shield doesn't go below 0
        
        // Update damage time for regeneration delay
        lastDamageTime = Time.time;
        
        // Notify UI of shield change
        OnShieldChanged?.Invoke(currentShield, maxShield);

        if (currentShield <= 0)
        {
            ShieldBroken();
        }
    }
    
    public void RegenerateShield(int amount)
    {
        currentShield += amount;
        currentShield = Mathf.Min(currentShield, maxShield); // Ensure shield doesn't exceed max
        
        // Notify UI of shield change
        OnShieldChanged?.Invoke(currentShield, maxShield);
    }
    
    public void SetMaxShield(int newMaxShield)
    {
        maxShield = newMaxShield;
        currentShield = Mathf.Min(currentShield, maxShield); // Adjust current shield if needed
        
        // Notify UI of shield change
        OnShieldChanged?.Invoke(currentShield, maxShield);
    }
    
    /// <summary>
    /// Manually trigger shield regeneration (useful for power-ups or abilities)
    /// </summary>
    public void ForceRegenerateShield(int amount)
    {
        RegenerateShield(amount);
    }
    
    /// <summary>
    /// Enable or disable automatic shield regeneration
    /// </summary>
    public void SetAutoRegeneration(bool enabled)
    {
        autoRegenerate = enabled;
    }
    
    /// <summary>
    /// Set the regeneration interval (time between regeneration ticks)
    /// </summary>
    public void SetRegenerationInterval(float interval)
    {
        regenerationInterval = interval;
    }
    
    /// <summary>
    /// Set the amount of shield regenerated per tick
    /// </summary>
    public void SetRegenerationAmount(int amount)
    {
        regenerationAmount = amount;
    }
    
    /// <summary>
    /// Set the delay before regeneration starts after taking damage
    /// </summary>
    public void SetRegenerationDelay(float delay)
    {
        regenerationDelay = delay;
    }

    void ShieldBroken()
    {
        Debug.Log("Shield broken!");
        // Add any shield broken effects here
    }
    
    // Testing methods for changing max shield
    [ContextMenu("Set Max Shield to 100")]
    public void TestSetMaxShield100()
    {
        SetMaxShield(100);
    }
    
    [ContextMenu("Set Max Shield to 200")]
    public void TestSetMaxShield200()
    {
        SetMaxShield(200);
    }
    
    [ContextMenu("Take 10 Shield Damage")]
    public void TestTakeShieldDamage()
    {
        TakeShieldDamage(10);
    }
    
    [ContextMenu("Regenerate 20 Shield")]
    public void TestRegenerateShield()
    {
        RegenerateShield(20);
    }
    
    [ContextMenu("Toggle Auto Regeneration")]
    public void TestToggleAutoRegeneration()
    {
        autoRegenerate = !autoRegenerate;
        Debug.Log($"Auto regeneration: {(autoRegenerate ? "ON" : "OFF")}");
    }
}
