using System.Collections.Generic;
using UnityEngine;

public class AuraSystem : MonoBehaviour
{
    [Header("Aura Settings")]
    [SerializeField] private float baseAuraRadius = 3f;
    [SerializeField] private LayerMask enemyLayerMask = 1; // Default layer for enemies
    [SerializeField] private LayerMask coinLayerMask = 1; // Default layer for coins
    
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
        
        // Configure the aura based on type
        switch (auraType)
        {
            case UpgradeData.UpgradeType.CoinMagnetAura:
                ConfigureCoinMagnetAura(auraEffect, radius);
                break;
            case UpgradeData.UpgradeType.SlowAura:
                ConfigureSlowAura(auraEffect, radius);
                break;
            case UpgradeData.UpgradeType.ShieldAura:
                ConfigureShieldAura(auraEffect, radius);
                break;
            case UpgradeData.UpgradeType.DamageAura:
                ConfigureDamageAura(auraEffect, radius);
                break;
            case UpgradeData.UpgradeType.HealAura:
                ConfigureHealAura(auraEffect, radius);
                break;
            default:
                Debug.LogWarning($"Unknown aura type: {auraType}");
                Destroy(auraObject);
                return null;
        }
        
        return auraEffect;
    }
    
    private void ConfigureCoinMagnetAura(AuraEffect auraEffect, float radius)
    {
        auraEffect.Initialize(UpgradeData.UpgradeType.CoinMagnetAura, radius, coinLayerMask);
        auraEffect.SetMaterial(coinMagnetMaterial);
        
        // Add debug logging for trigger events
        auraEffect.OnAuraTriggerEnter += (collider) => {
            Debug.Log($"CoinMagnetAura: TriggerEnter with {collider.name} (Tag: {collider.tag}, Layer: {collider.gameObject.layer})");
            
            if (collider.CompareTag("Money"))
            {
                Debug.Log($"CoinMagnetAura: Money detected! Moving coin {collider.name} towards player");
                // Move coin towards player
                Rigidbody coinRb = collider.GetComponent<Rigidbody>();
                if (coinRb != null)
                {
                    Vector3 direction = (transform.position - collider.transform.position).normalized;
                    coinRb.AddForce(direction * 10f, ForceMode.Force);
                    Debug.Log($"CoinMagnetAura: Applied force to {collider.name}");
                }
                else
                {
                    Debug.LogWarning($"CoinMagnetAura: No Rigidbody found on {collider.name}");
                }
            }
            else
            {
                Debug.Log($"CoinMagnetAura: Object {collider.name} doesn't have 'Money' tag (has '{collider.tag}' instead)");
            }
        };
        
        Debug.Log($"CoinMagnetAura: Configured with radius {radius}, layer mask: {coinLayerMask}");
    }
    
    private void ConfigureSlowAura(AuraEffect auraEffect, float radius)
    {
        auraEffect.Initialize(UpgradeData.UpgradeType.SlowAura, radius, enemyLayerMask);
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
    
    private void ConfigureShieldAura(AuraEffect auraEffect, float radius)
    {
        auraEffect.Initialize(UpgradeData.UpgradeType.ShieldAura, radius, enemyLayerMask);
        auraEffect.SetMaterial(shieldAuraMaterial);
        // Shield aura provides passive protection - no trigger events needed
    }
    
    private void ConfigureDamageAura(AuraEffect auraEffect, float radius)
    {
        auraEffect.Initialize(UpgradeData.UpgradeType.DamageAura, radius, enemyLayerMask);
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
    
    private void ConfigureHealAura(AuraEffect auraEffect, float radius)
    {
        auraEffect.Initialize(UpgradeData.UpgradeType.HealAura, radius, LayerMask.GetMask("Player"));
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
}
