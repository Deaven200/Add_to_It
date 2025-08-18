using System.Collections.Generic;
using UnityEngine;

public class AuraSystem : MonoBehaviour
{
    [Header("Aura Settings")]
    [SerializeField] private float baseAuraRadius = 3f;
    [SerializeField] private LayerMask enemyLayerMask = 1; // Default layer for enemies
    [Tooltip("Set this to the layer your enemies are on. If enemies use the 'Enemy' tag, this can stay as Default.")]
    // Note: Coin magnet aura uses tag-based detection ("Money"), so layer mask isn't needed for coins
    
    [Header("Visual Positioning")]
    [SerializeField] private Vector3 visualOffset = Vector3.zero; // Offset for visual effects from player center
    [SerializeField] private float cylinderHeight = 10f; // Height of the aura cylinder
    [SerializeField] private bool centerOnPlayer = true; // Whether to center effects on player or use offset
    
    [Header("Visual Effects")]
    [SerializeField] private Material coinMagnetMaterial;
    [SerializeField] private Material slowAuraMaterial;
    
    // Simple dictionary - one aura per type
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
        // Check if this aura type is supported (only coin magnet and slow auras)
        if (auraType != UpgradeData.UpgradeType.CoinMagnetAura && auraType != UpgradeData.UpgradeType.SlowAura)
        {
            Debug.LogWarning($"Aura type {auraType} is not supported. Only CoinMagnetAura and SlowAura are available.");
            return;
        }
        
        // If aura already exists, stack the effects by increasing the radius
        if (activeAuras.ContainsKey(auraType))
        {
            AuraEffect existingAura = activeAuras[auraType];
            if (existingAura != null)
            {
                // Calculate new combined radius (additive stacking)
                float existingRadius = existingAura.GetRadius();
                float newRadius = CalculateAuraRadius(value, rarity);
                float combinedRadius = existingRadius + newRadius;
                
                // Ensure the combined radius doesn't exceed a reasonable maximum
                float maxRadius = 20f; // Maximum radius to prevent performance issues
                combinedRadius = Mathf.Min(combinedRadius, maxRadius);
                
                // Update the existing aura with the new combined radius
                Vector3? visualOffsetParam = centerOnPlayer ? Vector3.zero : visualOffset;
                LayerMask layerMask = GetLayerMaskForAuraType(auraType);
                existingAura.Initialize(auraType, combinedRadius, layerMask, visualOffsetParam, cylinderHeight);
                
                Debug.Log($"Stacked {auraType} aura. New radius: {combinedRadius} (was {existingRadius} + {newRadius})");
                return;
            }
        }
        
        // Calculate aura radius based on value and rarity
        float auraRadius = CalculateAuraRadius(value, rarity);
        
        // Create the aura effect
        AuraEffect auraEffect = CreateAuraEffect(auraType, auraRadius);
        
        if (auraEffect != null)
        {
            activeAuras[auraType] = auraEffect;
            Debug.Log($"Added {auraType} aura with value {value}, Radius: {auraRadius}");
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
        // Use value directly as radius, ignoring rarity multiplier
        float radius = value;
        
        // Ensure minimum radius of 1 unit
        radius = Mathf.Max(radius, 1f);
        
        return radius;
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
            default:
                Debug.LogWarning($"Unknown aura type: {auraType}");
                Destroy(auraObject);
                return null;
        }
        
        return auraEffect;
    }
    
    private void ConfigureCoinMagnetAura(AuraEffect auraEffect, float radius, Vector3? visualOffset = null)
    {
        // Coin magnet aura uses tag-based detection, so we pass -1 (all layers)
        auraEffect.Initialize(UpgradeData.UpgradeType.CoinMagnetAura, radius, -1, visualOffset, cylinderHeight);
        
        // Only set material if it's assigned
        if (coinMagnetMaterial != null)
        {
            auraEffect.SetMaterial(coinMagnetMaterial);
        }
        else
        {
            Debug.LogWarning("CoinMagnetAura material is not assigned in AuraSystem inspector!");
        }
        
        // Try to apply coin prefab from AuraManagerUI immediately
        TryApplyCoinPrefabFromManager(auraEffect);
        
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
        
        // Only set material if it's assigned
        if (slowAuraMaterial != null)
        {
            auraEffect.SetMaterial(slowAuraMaterial);
        }
        else
        {
            Debug.LogWarning("SlowAura material is not assigned in AuraSystem inspector!");
        }
        
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
                return -1; // Coin magnet uses tag-based detection, -1 represents all layers
            case UpgradeData.UpgradeType.SlowAura:
                return enemyLayerMask;
            default:
                return LayerMask.GetMask("Default");
        }
    }
    
    // Getters for current settings
    public Vector3 GetVisualOffset() => visualOffset;
    public float GetCylinderHeight() => cylinderHeight;
    public bool GetCenterOnPlayer() => centerOnPlayer;
    
    // Aura management methods
    public Dictionary<UpgradeData.UpgradeType, AuraEffect> GetAllAuras()
    {
        return new Dictionary<UpgradeData.UpgradeType, AuraEffect>(activeAuras);
    }
    
    public List<UpgradeData.UpgradeType> GetActiveAuraTypes()
    {
        return new List<UpgradeData.UpgradeType>(activeAuras.Keys);
    }
    
    public AuraEffect GetAura(UpgradeData.UpgradeType auraType)
    {
        if (activeAuras.ContainsKey(auraType))
        {
            return activeAuras[auraType];
        }
        return null;
    }
    
    public void SetAuraOrbitingPrefab(UpgradeData.UpgradeType auraType, GameObject prefab)
    {
        if (activeAuras.ContainsKey(auraType))
        {
            AuraEffect aura = activeAuras[auraType];
            if (aura != null)
            {
                aura.SetOrbitingPrefab(prefab);
                Debug.Log($"Set orbiting prefab for {auraType} aura");
            }
        }
        else
        {
            Debug.LogWarning($"Cannot set prefab for {auraType} - aura not found");
        }
    }
    
    public void SetAllAurasOrbitingPrefab(GameObject prefab)
    {
        foreach (var aura in activeAuras.Values)
        {
            if (aura != null)
            {
                aura.SetOrbitingPrefab(prefab);
            }
        }
        Debug.Log($"Set orbiting prefab for all {activeAuras.Count} auras");
    }
    
    public void SetAuraOrbitSpeed(UpgradeData.UpgradeType auraType, float speed)
    {
        if (activeAuras.ContainsKey(auraType))
        {
            AuraEffect aura = activeAuras[auraType];
            if (aura != null)
            {
                aura.SetOrbitSpeed(speed);
                Debug.Log($"Set orbit speed for {auraType} aura to {speed}");
            }
        }
        else
        {
            Debug.LogWarning($"Cannot set orbit speed for {auraType} - aura not found");
        }
    }
    
    public void SetAllAurasOrbitSpeed(float speed)
    {
        foreach (var aura in activeAuras.Values)
        {
            if (aura != null)
            {
                aura.SetOrbitSpeed(speed);
            }
        }
        Debug.Log($"Set orbit speed for all {activeAuras.Count} auras to {speed}");
    }
    
    // Try to apply coin prefab from AuraManagerUI
    private void TryApplyCoinPrefabFromManager(AuraEffect auraEffect)
    {
        // Find AuraManagerUI in the scene
        AuraManagerUI auraManager = FindObjectOfType<AuraManagerUI>();
        if (auraManager != null)
        {
            GameObject coinPrefab = auraManager.GetCoinPrefab();
            if (coinPrefab != null)
            {
                auraEffect.SetOrbitingPrefab(coinPrefab);
                Debug.Log($"Applied coin prefab from AuraManagerUI: {coinPrefab.name}");
            }
            else
            {
                Debug.Log("AuraManagerUI found but no coin prefab assigned");
            }
            
            // Apply visual settings if enabled
            if (auraManager.GetApplyVisualSettingsToAll())
            {
                StartCoroutine(DelayedApplyVisualSettings(auraEffect, auraManager));
            }
        }
        else
        {
            Debug.Log("No AuraManagerUI found in scene");
        }
    }
    
    private System.Collections.IEnumerator DelayedApplyVisualSettings(AuraEffect auraEffect, AuraManagerUI auraManager)
    {
        // Wait a bit for orbiting objects to be created
        yield return new WaitForSeconds(0.1f);
        
        // Apply visual settings to the new aura
        auraManager.ApplyVisualSettingsToAura(auraEffect);
    }
    
    // Debug method to list all active auras
    [ContextMenu("List All Active Auras")]
    public void ListAllActiveAuras()
    {
        Debug.Log($"=== Active Auras ({activeAuras.Count}) ===");
        foreach (var kvp in activeAuras)
        {
            UpgradeData.UpgradeType auraType = kvp.Key;
            AuraEffect aura = kvp.Value;
            
            if (aura != null)
            {
                Debug.Log($"- {auraType}: Radius = {aura.GetRadius()}, Enabled = {aura.IsAuraEnabled()}");
            }
            else
            {
                Debug.Log($"- {auraType}: NULL (should be cleaned up)");
            }
        }
        Debug.Log("=== End Aura List ===");
    }
    

}
