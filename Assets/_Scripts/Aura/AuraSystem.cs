using System.Collections.Generic;
using UnityEngine;

public class AuraSystem : MonoBehaviour
{
    [Header("Aura Settings")]
    [SerializeField] private float baseAuraRadius = 3f;
    [SerializeField] private LayerMask enemyLayerMask = 1; // Default layer for enemies
    [SerializeField] private LayerMask coinLayerMask = 1; // Default layer for coins
    
    [Header("Visual Positioning")]
    [SerializeField] private Vector3 visualOffset = Vector3.zero; // Offset for visual effects from player center
    [SerializeField] private float cylinderHeight = 10f; // Height of the aura cylinder
    [SerializeField] private bool centerOnPlayer = true; // Whether to center effects on player or use offset
    
    [Header("Visual Effects")]
    [SerializeField] private Material coinMagnetMaterial;
    [SerializeField] private Material slowAuraMaterial;
    [SerializeField] private Material shieldAuraMaterial;
    [SerializeField] private Material damageAuraMaterial;
    [SerializeField] private Material healAuraMaterial;
    
    private Dictionary<UpgradeData.UpgradeType, AuraEffect> activeAuras = new Dictionary<UpgradeData.UpgradeType, AuraEffect>();
    private PlayerHealth playerHealth;
    private PlayerMoney playerMoney;
    
    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerMoney = GetComponent<PlayerMoney>();
    }
    
    public void AddAura(UpgradeData.UpgradeType auraType, float value, UpgradeData.Rarity rarity)
    {
        // Remove existing aura of the same type if it exists
        if (activeAuras.ContainsKey(auraType))
        {
            RemoveAura(auraType);
        }
        
        // Calculate aura radius based on value and rarity
        float auraRadius = CalculateAuraRadius(value, rarity);
        
        // Create the aura effect
        AuraEffect auraEffect = CreateAuraEffect(auraType, auraRadius);
        
        if (auraEffect != null)
        {
            activeAuras[auraType] = auraEffect;
            Debug.Log($"Added {auraType} aura with radius {auraRadius}");
        }
    }
    
    public void RemoveAura(UpgradeData.UpgradeType auraType)
    {
        if (activeAuras.ContainsKey(auraType))
        {
            if (activeAuras[auraType] != null)
            {
                Destroy(activeAuras[auraType].gameObject);
            }
            activeAuras.Remove(auraType);
            Debug.Log($"Removed {auraType} aura");
        }
    }
    
    public void RemoveAllAuras()
    {
        foreach (var aura in activeAuras.Values)
        {
            if (aura != null)
            {
                Destroy(aura.gameObject);
            }
        }
        activeAuras.Clear();
        Debug.Log("Removed all auras");
    }
    
    private float CalculateAuraRadius(float value, UpgradeData.Rarity rarity)
    {
        float rarityMultiplier = GetRarityMultiplier(rarity);
        return baseAuraRadius * (value / 5f) * rarityMultiplier; // Normalize value to 5 as baseline
    }
    
    private float GetRarityMultiplier(UpgradeData.Rarity rarity)
    {
        switch (rarity)
        {
            case UpgradeData.Rarity.Trashy: return 0.5f;
            case UpgradeData.Rarity.Poor: return 0.75f;
            case UpgradeData.Rarity.Common: return 1f;
            case UpgradeData.Rarity.Uncommon: return 1.25f;
            case UpgradeData.Rarity.Rare: return 1.5f;
            case UpgradeData.Rarity.Epic: return 2f;
            case UpgradeData.Rarity.Legendary: return 2.5f;
            case UpgradeData.Rarity.Mythic: return 3f;
            case UpgradeData.Rarity.Exotic: return 4f;
            default: return 1f;
        }
    }
    
    private AuraEffect CreateAuraEffect(UpgradeData.UpgradeType auraType, float radius)
    {
        // Create a child GameObject for the aura
        GameObject auraObject = new GameObject($"{auraType}Aura");
        auraObject.transform.SetParent(transform);
        auraObject.transform.localPosition = Vector3.zero;
        
        // Add the AuraEffect component
        AuraEffect auraEffect = auraObject.AddComponent<AuraEffect>();
        
        // Calculate visual positioning
        Vector3? visualOffsetParam = centerOnPlayer ? Vector3.zero : visualOffset;
        
        // Configure the aura based on type
        switch (auraType)
        {
            case UpgradeData.UpgradeType.CoinMagnetAura:
                ConfigureCoinMagnetAura(auraEffect, radius, visualOffsetParam);
                break;
            case UpgradeData.UpgradeType.SlowAura:
                ConfigureSlowAura(auraEffect, radius, visualOffsetParam);
                break;
            case UpgradeData.UpgradeType.ShieldAura:
                ConfigureShieldAura(auraEffect, radius, visualOffsetParam);
                break;
            case UpgradeData.UpgradeType.DamageAura:
                ConfigureDamageAura(auraEffect, radius, visualOffsetParam);
                break;
            case UpgradeData.UpgradeType.HealAura:
                ConfigureHealAura(auraEffect, radius, visualOffsetParam);
                break;
            default:
                Debug.LogWarning($"Unknown aura type: {auraType}");
                Destroy(auraObject);
                return null;
        }
        
        return auraEffect;
    }
    
    private void ConfigureCoinMagnetAura(AuraEffect auraEffect, float radius, Vector3? visualOffset = null)
    {
        auraEffect.Initialize(UpgradeData.UpgradeType.CoinMagnetAura, radius, coinLayerMask, visualOffset, cylinderHeight);
        auraEffect.SetMaterial(coinMagnetMaterial);
        
        // Handle coin attraction when coins enter the aura
        auraEffect.OnAuraTriggerEnter += (collider) => {
            if (collider.CompareTag("Money"))
            {
                // Start moving coin towards player using transform
                StartCoroutine(MoveCoinTowardsPlayer(collider.gameObject));
            }
        };
        
        // Also add continuous attraction while coins are in the aura
        auraEffect.OnAuraTriggerStay += (collider) => {
            if (collider.CompareTag("Money"))
            {
                // Apply continuous gentle force while in aura
                Vector3 direction = (transform.position - collider.transform.position).normalized;
                collider.transform.position += direction * 2f * Time.deltaTime; // Slower continuous movement
            }
        };
        
    }
    
    private void ConfigureSlowAura(AuraEffect auraEffect, float radius, Vector3? visualOffset = null)
    {
        auraEffect.Initialize(UpgradeData.UpgradeType.SlowAura, radius, enemyLayerMask, visualOffset, cylinderHeight);
        auraEffect.SetMaterial(slowAuraMaterial);
        auraEffect.OnAuraTriggerEnter += (collider) => {
            if (collider.CompareTag("Enemy"))
            {
                // Slow down enemy
                UnityEngine.AI.NavMeshAgent agent = collider.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null)
                {
                    agent.speed *= 0.5f; // Slow to 50% speed
                }
            }
        };
        auraEffect.OnAuraTriggerExit += (collider) => {
            if (collider.CompareTag("Enemy"))
            {
                // Restore enemy speed
                UnityEngine.AI.NavMeshAgent agent = collider.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null)
                {
                    agent.speed *= 2f; // Restore to normal speed
                }
            }
        };
    }
    
    private void ConfigureShieldAura(AuraEffect auraEffect, float radius, Vector3? visualOffset = null)
    {
        auraEffect.Initialize(UpgradeData.UpgradeType.ShieldAura, radius, enemyLayerMask, visualOffset, cylinderHeight);
        auraEffect.SetMaterial(shieldAuraMaterial);
        
        // Keep shield aura as trigger but use custom enemy blocking logic
        // This allows bullets and coins to pass through while blocking enemies
        
        // Shield aura blocks enemies from passing through
        auraEffect.OnAuraTriggerEnter += (collider) => {
            if (collider.CompareTag("Enemy"))
            {
                // Push enemy back from the shield
                Vector3 pushDirection = (collider.transform.position - transform.position).normalized;
                collider.transform.position += pushDirection * 2f;
                
                // If enemy has a NavMeshAgent, stop it temporarily
                UnityEngine.AI.NavMeshAgent agent = collider.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null)
                {
                    agent.isStopped = true;
                    StartCoroutine(ResumeEnemyMovement(agent, 0.5f));
                }
            }
        };
        
        // Shield aura absorbs damage when enemies attack
        auraEffect.OnAuraTriggerStay += (collider) => {
            if (collider.CompareTag("Enemy"))
            {
                // Check if enemy is attacking (you can modify this based on your enemy attack system)
                EnemyAttack enemyAttack = collider.GetComponent<EnemyAttack>();
                if (enemyAttack != null && enemyAttack.IsAttacking())
                {
                    // Absorb the damage and reduce shield
                    float damageToAbsorb = enemyAttack.GetAttackDamage();
                    AbsorbDamage(damageToAbsorb);
                    
                    // Optionally push the enemy back
                    Vector3 pushDirection = (collider.transform.position - transform.position).normalized;
                    collider.transform.position += pushDirection * 1f;
                }
            }
        };
    }
    
    private void ConfigureDamageAura(AuraEffect auraEffect, float radius, Vector3? visualOffset = null)
    {
        auraEffect.Initialize(UpgradeData.UpgradeType.DamageAura, radius, enemyLayerMask, visualOffset, cylinderHeight);
        auraEffect.SetMaterial(damageAuraMaterial);
        auraEffect.OnAuraTriggerStay += (collider) => {
            if (collider.CompareTag("Enemy"))
            {
                // Deal damage to enemy over time
                EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(1f * Time.deltaTime); // 1 damage per second
                }
            }
        };
    }
    
    private void ConfigureHealAura(AuraEffect auraEffect, float radius, Vector3? visualOffset = null)
    {
        auraEffect.Initialize(UpgradeData.UpgradeType.HealAura, radius, LayerMask.GetMask("Player"), visualOffset, cylinderHeight);
        auraEffect.SetMaterial(healAuraMaterial);
        auraEffect.OnAuraTriggerStay += (collider) => {
            if (collider.CompareTag("Player") && playerHealth != null)
            {
                // Heal player over time
                playerHealth.Heal(1); // Heal 1 health per frame (will be limited by fire rate)
            }
        };
    }
    
    // Public methods to check if specific auras are active
    public bool HasAura(UpgradeData.UpgradeType auraType)
    {
        return activeAuras.ContainsKey(auraType);
    }
    
    public int GetActiveAuraCount()
    {
        return activeAuras.Count;
    }
    
    // Debug method
    [ContextMenu("Test Add All Auras")]
    public void TestAddAllAuras()
    {
        AddAura(UpgradeData.UpgradeType.CoinMagnetAura, 5f, UpgradeData.Rarity.Common);
        AddAura(UpgradeData.UpgradeType.SlowAura, 5f, UpgradeData.Rarity.Common);
        AddAura(UpgradeData.UpgradeType.ShieldAura, 5f, UpgradeData.Rarity.Common);
        AddAura(UpgradeData.UpgradeType.DamageAura, 5f, UpgradeData.Rarity.Common);
        AddAura(UpgradeData.UpgradeType.HealAura, 5f, UpgradeData.Rarity.Common);
    }
    
    // Coroutine to move coins towards player
    private System.Collections.IEnumerator MoveCoinTowardsPlayer(GameObject coin)
    {
        float moveSpeed = 5f; // Adjust this value to control attraction speed
        float minDistance = 1f; // Stop moving when this close to player
        
        while (coin != null && Vector3.Distance(coin.transform.position, transform.position) > minDistance)
        {
            if (coin == null) yield break; // Coin was destroyed
            
            Vector3 direction = (transform.position - coin.transform.position).normalized;
            coin.transform.position += direction * moveSpeed * Time.deltaTime;
            
            yield return null; // Wait for next frame
        }
    }
    
    // Coroutine to resume enemy movement after being blocked by shield
    private System.Collections.IEnumerator ResumeEnemyMovement(UnityEngine.AI.NavMeshAgent agent, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (agent != null)
        {
            agent.isStopped = false;
        }
    }
    
    // Shield damage absorption system
    private float currentShieldHealth = 100f;
    private float maxShieldHealth = 100f;
    
    private void AbsorbDamage(float damage)
    {
        if (currentShieldHealth > 0)
        {
            currentShieldHealth -= damage;
            
            // If shield is depleted, remove the shield aura
            if (currentShieldHealth <= 0)
            {
                currentShieldHealth = 0;
                RemoveAura(UpgradeData.UpgradeType.ShieldAura);
                Debug.Log("Shield Aura depleted and removed!");
            }
            else
            {
                Debug.Log($"Shield absorbed {damage} damage. Shield health: {currentShieldHealth}/{maxShieldHealth}");
            }
        }
    }
    
    // Public methods for shield management
    public float GetShieldHealth() => currentShieldHealth;
    public float GetMaxShieldHealth() => maxShieldHealth;
    public float GetShieldHealthPercentage() => currentShieldHealth / maxShieldHealth;
    
    // Public methods for visual positioning
    public void SetVisualOffset(Vector3 offset)
    {
        visualOffset = offset;
        // Update all existing auras with new positioning
        foreach (var aura in activeAuras.Values)
        {
            if (aura != null)
            {
                Vector3? newOffset = centerOnPlayer ? Vector3.zero : visualOffset;
                LayerMask layerMask = GetLayerMaskForAuraType(aura.GetAuraType());
                aura.Initialize(aura.GetAuraType(), aura.GetRadius(), layerMask, newOffset, cylinderHeight);
            }
        }
    }
    
    public void SetCylinderHeight(float height)
    {
        cylinderHeight = height;
        // Update all existing auras with new height
        foreach (var aura in activeAuras.Values)
        {
            if (aura != null)
            {
                Vector3? currentOffset = centerOnPlayer ? Vector3.zero : visualOffset;
                LayerMask layerMask = GetLayerMaskForAuraType(aura.GetAuraType());
                aura.Initialize(aura.GetAuraType(), aura.GetRadius(), layerMask, currentOffset, cylinderHeight);
            }
        }
    }
    
    public void SetCenterOnPlayer(bool center)
    {
        centerOnPlayer = center;
        // Update all existing auras with new positioning
        foreach (var aura in activeAuras.Values)
        {
            if (aura != null)
            {
                Vector3? newOffset = centerOnPlayer ? Vector3.zero : visualOffset;
                LayerMask layerMask = GetLayerMaskForAuraType(aura.GetAuraType());
                aura.Initialize(aura.GetAuraType(), aura.GetRadius(), layerMask, newOffset, cylinderHeight);
            }
        }
    }
    
    private LayerMask GetLayerMaskForAuraType(UpgradeData.UpgradeType auraType)
    {
        switch (auraType)
        {
            case UpgradeData.UpgradeType.CoinMagnetAura:
                return coinLayerMask;
            case UpgradeData.UpgradeType.SlowAura:
            case UpgradeData.UpgradeType.ShieldAura:
            case UpgradeData.UpgradeType.DamageAura:
                return enemyLayerMask;
            case UpgradeData.UpgradeType.HealAura:
                return LayerMask.GetMask("Player");
            default:
                return LayerMask.GetMask("Default");
        }
    }
    
    // Getters for current settings
    public Vector3 GetVisualOffset() => visualOffset;
    public float GetCylinderHeight() => cylinderHeight;
    public bool GetCenterOnPlayer() => centerOnPlayer;
    
    // Method to restore shield health (can be called by healing items or abilities)
    public void RestoreShieldHealth(float amount)
    {
        currentShieldHealth = Mathf.Min(currentShieldHealth + amount, maxShieldHealth);
    }
}
