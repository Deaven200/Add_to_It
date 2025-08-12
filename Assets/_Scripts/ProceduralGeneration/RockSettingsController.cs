using UnityEngine;

public class RockSettingsController : MonoBehaviour
{
    [Header("Rock Settings")]
    [SerializeField] private bool enableRocks = true;
    [SerializeField, Range(0f, 0.1f)] private float rockDensity = 0.02f;
    [SerializeField, Range(1f, 10f)] private float rockSpacing = 3f;
    [SerializeField, Range(0f, 1f)] private float rockDensityVariation = 0.5f;
    
    [Header("Real-time Control")]
    [SerializeField] private bool updateInRealTime = false;
    [SerializeField] private KeyCode toggleRocksKey = KeyCode.R;
    [SerializeField] private KeyCode increaseDensityKey = KeyCode.Equals;
    [SerializeField] private KeyCode decreaseDensityKey = KeyCode.Minus;
    
    private TerrainSettings terrainSettings;
    private ProceduralLevelManager levelManager;
    
    void Start()
    {
        // Find the level manager
        levelManager = FindObjectOfType<ProceduralLevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("RockSettingsController: No ProceduralLevelManager found!");
            return;
        }
        
        // Find terrain settings
        TerrainGenerator terrainGen = levelManager.GetComponent<TerrainGenerator>();
        if (terrainGen != null)
        {
            // We'll need to access the settings through reflection or modify TerrainGenerator
            // Debug.Log("RockSettingsController: Found TerrainGenerator");
        }
        
        // Try to find existing terrain settings
        terrainSettings = FindObjectOfType<TerrainSettings>();
        if (terrainSettings == null)
        {
            // Create a new one if none exists
            terrainSettings = ScriptableObject.CreateInstance<TerrainSettings>();
            // Debug.Log("RockSettingsController: Created new TerrainSettings");
        }
        
        // Apply initial settings
        ApplyRockSettings();
    }
    
    void Update()
    {
        if (!updateInRealTime) return;
        
        // Handle keyboard controls
        if (Input.GetKeyDown(toggleRocksKey))
        {
            enableRocks = !enableRocks;
            ApplyRockSettings();
            // Debug.Log($"Rocks {(enableRocks ? "enabled" : "disabled")}");
        }
        
        if (Input.GetKeyDown(increaseDensityKey))
        {
            rockDensity = Mathf.Min(rockDensity + 0.01f, 0.1f);
            ApplyRockSettings();
            // Debug.Log($"Rock density increased to: {rockDensity:F3}");
        }
        
        if (Input.GetKeyDown(decreaseDensityKey))
        {
            rockDensity = Mathf.Max(rockDensity - 0.01f, 0f);
            ApplyRockSettings();
            // Debug.Log($"Rock density decreased to: {rockDensity:F3}");
        }
    }
    
    void ApplyRockSettings()
    {
        if (terrainSettings == null) return;
        
        // Apply settings to terrain settings
        // Note: We need to use reflection since these are private fields
        var settingsType = terrainSettings.GetType();
        
        // Set enable rocks
        var enableRocksField = settingsType.GetField("enableRocks", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (enableRocksField != null)
        {
            enableRocksField.SetValue(terrainSettings, enableRocks);
        }
        
        // Set rock density
        var rockDensityField = settingsType.GetField("rockDensity", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (rockDensityField != null)
        {
            rockDensityField.SetValue(terrainSettings, rockDensity);
        }
        
        // Set rock spacing
        var rockSpacingField = settingsType.GetField("rockSpacing", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (rockSpacingField != null)
        {
            rockSpacingField.SetValue(terrainSettings, rockSpacing);
        }
        
        // Set rock density variation
        var rockDensityVariationField = settingsType.GetField("rockDensityVariation", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (rockDensityVariationField != null)
        {
            rockDensityVariationField.SetValue(terrainSettings, rockDensityVariation);
        }
        
        // Debug.Log($"Applied rock settings - Density: {rockDensity:F3}, Spacing: {rockSpacing:F1}, Variation: {rockDensityVariation:F2}");
    }
    
    // Public methods for external control
    public void SetRockDensity(float density)
    {
        rockDensity = Mathf.Clamp(density, 0f, 0.1f);
        ApplyRockSettings();
    }
    
    public void SetRockSpacing(float spacing)
    {
        rockSpacing = Mathf.Clamp(spacing, 1f, 10f);
        ApplyRockSettings();
    }
    
    public void ToggleRocks()
    {
        enableRocks = !enableRocks;
        ApplyRockSettings();
    }
    
    public void EnableRocks(bool enable)
    {
        enableRocks = enable;
        ApplyRockSettings();
    }
    
    // Context menu options
    [ContextMenu("Disable Rocks")]
    public void DisableRocks()
    {
        enableRocks = false;
        ApplyRockSettings();
    }
    
    [ContextMenu("Enable Rocks")]
    public void EnableRocks()
    {
        enableRocks = true;
        ApplyRockSettings();
    }
    
    [ContextMenu("Set Low Rock Density")]
    public void SetLowRockDensity()
    {
        rockDensity = 0.01f;
        rockSpacing = 5f;
        ApplyRockSettings();
    }
    
    [ContextMenu("Set Medium Rock Density")]
    public void SetMediumRockDensity()
    {
        rockDensity = 0.02f;
        rockSpacing = 3f;
        ApplyRockSettings();
    }
    
    [ContextMenu("Set High Rock Density")]
    public void SetHighRockDensity()
    {
        rockDensity = 0.05f;
        rockSpacing = 2f;
        ApplyRockSettings();
    }
    
    void OnGUI()
    {
        if (!updateInRealTime) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 250, 200));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("Rock Settings Control", GUI.skin.box);
        
        GUILayout.Label($"Rocks: {(enableRocks ? "ON" : "OFF")}");
        GUILayout.Label($"Density: {rockDensity:F3}");
        GUILayout.Label($"Spacing: {rockSpacing:F1}");
        
        GUILayout.Space(10);
        
        GUILayout.Label("Controls:");
        GUILayout.Label($"R - Toggle rocks");
        GUILayout.Label($"+ - Increase density");
        GUILayout.Label($"- - Decrease density");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
