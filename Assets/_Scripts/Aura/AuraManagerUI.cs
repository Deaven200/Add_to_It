using System.Collections.Generic;
using UnityEngine;

public class AuraManagerUI : MonoBehaviour
{
    [Header("Aura System Reference")]
    [SerializeField] private AuraSystem auraSystem;
    
    [Header("Prefab Management")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject slowAuraPrefab;
    [SerializeField] private GameObject defaultPrefab;
    
    [Header("Orbit Settings")]
    [SerializeField] private float globalOrbitSpeed = 30f;
    [SerializeField] private bool randomizePositions = true;
    
    [Header("Visual Settings")]
    [SerializeField, Range(0f, 1f)] private float globalTransparency = 1f;
    [SerializeField, Range(0f, 2f)] private float globalGlow = 0f;
    [SerializeField] private bool applyVisualSettingsToAll = true;
    
    [Header("Debug Info")]
    [SerializeField] private bool showDebugInfo = true;
    
    private void Start()
    {
        // Auto-find AuraSystem if not assigned
        if (auraSystem == null)
        {
            auraSystem = FindObjectOfType<AuraSystem>();
            if (auraSystem == null)
            {
                Debug.LogError("AuraManagerUI: No AuraSystem found in scene!");
                return;
            }
        }
        
        // Apply initial settings
        ApplyGlobalSettings();
        
        // Apply specific prefab settings immediately
        ApplySpecificPrefabSettings();
        
        // Apply visual settings after a short delay to ensure auras are created
        StartCoroutine(DelayedApplyVisualSettings());
    }
    
    private void Update()
    {
        if (showDebugInfo && Input.GetKeyDown(KeyCode.F1))
        {
            ListAllAuras();
        }
    }
    
    [ContextMenu("List All Auras")]
    public void ListAllAuras()
    {
        if (auraSystem == null) return;
        
        Debug.Log("=== AURA MANAGER UI ===");
        auraSystem.ListAllActiveAuras();
        
        // Show detailed info for each aura
        var allAuras = auraSystem.GetAllAuras();
        foreach (var kvp in allAuras)
        {
            var aura = kvp.Value;
            if (aura != null)
            {
                Debug.Log($"Detailed Info: {aura.GetAuraInfo()}");
            }
        }
    }
    
    [ContextMenu("Apply Global Settings")]
    public void ApplyGlobalSettings()
    {
        if (auraSystem == null) return;
        
        // Set orbit speed for all auras
        auraSystem.SetAllAurasOrbitSpeed(globalOrbitSpeed);
        
        // Set default prefab for all auras if assigned
        if (defaultPrefab != null)
        {
            auraSystem.SetAllAurasOrbitingPrefab(defaultPrefab);
        }
        
        // Apply visual settings to all auras
        if (applyVisualSettingsToAll)
        {
            ApplyVisualSettingsToAllAuras();
        }
        
        Debug.Log("Applied global aura settings");
    }
    
    [ContextMenu("Apply Specific Prefab Settings")]
    public void ApplySpecificPrefabSettings()
    {
        if (auraSystem == null) return;
        
        // Apply coin prefab to coin magnet aura
        if (coinPrefab != null)
        {
            auraSystem.SetAuraOrbitingPrefab(UpgradeData.UpgradeType.CoinMagnetAura, coinPrefab);
            Debug.Log("Applied coin prefab to CoinMagnetAura");
        }
        
        // Apply slow aura prefab to slow aura
        if (slowAuraPrefab != null)
        {
            auraSystem.SetAuraOrbitingPrefab(UpgradeData.UpgradeType.SlowAura, slowAuraPrefab);
            Debug.Log("Applied slow aura prefab to SlowAura");
        }
    }
    
    [ContextMenu("Force Refresh All Auras")]
    public void ForceRefreshAllAuras()
    {
        if (auraSystem == null) return;
        
        // Get all active auras and force them to recreate their orbiting objects
        var allAuras = auraSystem.GetAllAuras();
        foreach (var kvp in allAuras)
        {
            var aura = kvp.Value;
            if (aura != null)
            {
                // Force the aura to update its orbiting prefab count (this will recreate objects)
                aura.SetOrbitingPrefab(aura.GetOrbitingPrefab());
                Debug.Log($"Refreshed {kvp.Key} aura orbiting objects");
            }
        }
        
        Debug.Log("Force refreshed all aura orbiting objects");
    }
    
    [ContextMenu("Apply Visual Settings to All Auras")]
    public void ApplyVisualSettingsToAllAuras()
    {
        if (auraSystem == null) return;
        
        var allAuras = auraSystem.GetAllAuras();
        foreach (var kvp in allAuras)
        {
            var aura = kvp.Value;
            if (aura != null)
            {
                ApplyVisualSettingsToAura(aura);
            }
        }
        
        Debug.Log($"Applied visual settings to {allAuras.Count} auras (Transparency: {globalTransparency}, Glow: {globalGlow})");
    }
    
    public void ApplyVisualSettingsToAura(AuraEffect aura)
    {
        // Get all orbiting objects from the aura
        var orbitingObjects = aura.GetOrbitingObjects();
        
        foreach (GameObject orbitingObj in orbitingObjects)
        {
            if (orbitingObj != null)
            {
                ApplyVisualSettingsToObject(orbitingObj);
            }
        }
    }
    
    private void ApplyVisualSettingsToObject(GameObject obj)
    {
        // Get the renderer
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer == null) return;
        
        // Get or create material
        Material material = renderer.material;
        if (material == null)
        {
            material = new Material(Shader.Find("Standard"));
            renderer.material = material;
        }
        
        // Apply transparency
        Color color = material.color;
        color.a = globalTransparency;
        material.color = color;
        
        // Apply glow (emission)
        if (globalGlow > 0f)
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * globalGlow);
        }
        else
        {
            material.DisableKeyword("_EMISSION");
        }
        
        // Enable transparency mode if needed
        if (globalTransparency < 1f)
        {
            material.SetFloat("_Mode", 3); // Transparent mode
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
        }
    }
    
    [ContextMenu("Set Coin Prefab for Coin Magnet")]
    public void SetCoinPrefabForCoinMagnet()
    {
        if (auraSystem == null || coinPrefab == null) return;
        
        auraSystem.SetAuraOrbitingPrefab(UpgradeData.UpgradeType.CoinMagnetAura, coinPrefab);
    }
    
    [ContextMenu("Set Prefab for Slow Aura")]
    public void SetPrefabForSlowAura()
    {
        if (auraSystem == null || slowAuraPrefab == null) return;
        
        auraSystem.SetAuraOrbitingPrefab(UpgradeData.UpgradeType.SlowAura, slowAuraPrefab);
    }
    
    [ContextMenu("Set Default Prefab for All")]
    public void SetDefaultPrefabForAll()
    {
        if (auraSystem == null || defaultPrefab == null) return;
        
        auraSystem.SetAllAurasOrbitingPrefab(defaultPrefab);
    }
    
    // Public methods for external calls
    public void SetCoinPrefab(GameObject prefab)
    {
        coinPrefab = prefab;
        if (auraSystem != null)
        {
            auraSystem.SetAuraOrbitingPrefab(UpgradeData.UpgradeType.CoinMagnetAura, prefab);
        }
    }
    
    public void SetSlowAuraPrefab(GameObject prefab)
    {
        slowAuraPrefab = prefab;
        if (auraSystem != null)
        {
            auraSystem.SetAuraOrbitingPrefab(UpgradeData.UpgradeType.SlowAura, prefab);
        }
    }
    
    public void SetGlobalOrbitSpeed(float speed)
    {
        globalOrbitSpeed = speed;
        if (auraSystem != null)
        {
            auraSystem.SetAllAurasOrbitSpeed(speed);
        }
    }
    
    public void SetGlobalPrefab(GameObject prefab)
    {
        defaultPrefab = prefab;
        if (auraSystem != null)
        {
            auraSystem.SetAllAurasOrbitingPrefab(prefab);
        }
    }
    
    // Visual settings methods
    public void SetGlobalTransparency(float transparency)
    {
        globalTransparency = Mathf.Clamp01(transparency);
        if (applyVisualSettingsToAll && auraSystem != null)
        {
            ApplyVisualSettingsToAllAuras();
        }
    }
    
    public void SetGlobalGlow(float glow)
    {
        globalGlow = Mathf.Clamp(glow, 0f, 2f);
        if (applyVisualSettingsToAll && auraSystem != null)
        {
            ApplyVisualSettingsToAllAuras();
        }
    }
    
    public void SetApplyVisualSettingsToAll(bool apply)
    {
        applyVisualSettingsToAll = apply;
    }
    
    private System.Collections.IEnumerator DelayedApplyVisualSettings()
    {
        // Wait a bit for auras to be created
        yield return new WaitForSeconds(0.2f);
        
        // Apply visual settings to all existing auras
        if (applyVisualSettingsToAll)
        {
            ApplyVisualSettingsToAllAuras();
        }
    }
    
    // Getter methods
    public AuraSystem GetAuraSystem() => auraSystem;
    public GameObject GetCoinPrefab() => coinPrefab;
    public GameObject GetSlowAuraPrefab() => slowAuraPrefab;
    public GameObject GetDefaultPrefab() => defaultPrefab;
    public float GetGlobalOrbitSpeed() => globalOrbitSpeed;
    public float GetGlobalTransparency() => globalTransparency;
    public float GetGlobalGlow() => globalGlow;
    public bool GetApplyVisualSettingsToAll() => applyVisualSettingsToAll;
    
    // Method to get all active aura types
    public List<UpgradeData.UpgradeType> GetActiveAuraTypes()
    {
        if (auraSystem == null) return new List<UpgradeData.UpgradeType>();
        return auraSystem.GetActiveAuraTypes();
    }
    
    // Method to get specific aura
    public AuraEffect GetAura(UpgradeData.UpgradeType auraType)
    {
        if (auraSystem == null) return null;
        return auraSystem.GetAura(auraType);
    }
}
