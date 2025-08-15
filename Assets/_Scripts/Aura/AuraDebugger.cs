using UnityEngine;

public class AuraDebugger : MonoBehaviour
{
    private AuraSystem auraSystem;
    private int lastAuraCount = -1;
    
    void Start()
    {
        // Check for AuraTestHelper components
        AuraTestHelper[] testHelpers = FindObjectsOfType<AuraTestHelper>();
        if (testHelpers.Length > 0)
        {
            Debug.LogError($"Found {testHelpers.Length} AuraTestHelper components in scene! These might be adding auras automatically.");
            foreach (var helper in testHelpers)
            {
                Debug.LogError($"AuraTestHelper found on: {helper.gameObject.name}");
            }
        }
        
        // Check for AuraSystem components
        AuraSystem[] auraSystems = FindObjectsOfType<AuraSystem>();
        Debug.Log($"Found {auraSystems.Length} AuraSystem components in scene");
        foreach (var system in auraSystems)
        {
            Debug.Log($"AuraSystem on: {system.gameObject.name}, Active auras: {system.GetActiveAuraCount()}");
            auraSystem = system; // Store reference to the first one
        }
        
        // Check for PlayerAuraSetup components
        PlayerAuraSetup[] setups = FindObjectsOfType<PlayerAuraSetup>();
        Debug.Log($"Found {setups.Length} PlayerAuraSetup components in scene");
        foreach (var setup in setups)
        {
            Debug.Log($"PlayerAuraSetup on: {setup.gameObject.name}");
        }
        
        // Check for any AuraEffect components
        AuraEffect[] auraEffects = FindObjectsOfType<AuraEffect>();
        Debug.Log($"Found {auraEffects.Length} AuraEffect components in scene");
        foreach (var effect in auraEffects)
        {
            Debug.Log($"AuraEffect on: {effect.gameObject.name}, Type: {effect.GetAuraType()}, Active: {effect.IsActive()}");
        }
        
        // Check for any GameObjects with "Aura" in the name
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int auraObjects = 0;
        foreach (var obj in allObjects)
        {
            if (obj.name.ToLower().Contains("aura"))
            {
                Debug.Log($"Found GameObject with 'aura' in name: {obj.name}");
                auraObjects++;
            }
        }
        Debug.Log($"Found {auraObjects} GameObjects with 'aura' in the name");
        
        // Check for any GameObjects with "GroundRing" in the name
        int ringObjects = 0;
        foreach (var obj in allObjects)
        {
            if (obj.name.ToLower().Contains("groundring"))
            {
                Debug.Log($"Found GameObject with 'groundring' in name: {obj.name}");
                ringObjects++;
            }
        }
        Debug.Log($"Found {ringObjects} GameObjects with 'groundring' in the name");
    }
    
    void Update()
    {
        if (auraSystem != null)
        {
            int currentAuraCount = auraSystem.GetActiveAuraCount();
            if (currentAuraCount != lastAuraCount)
            {
                Debug.LogError($"AURA COUNT CHANGED: {lastAuraCount} -> {currentAuraCount} at frame {Time.frameCount}");
                lastAuraCount = currentAuraCount;
                
                // Log the call stack to see what caused this change
                Debug.LogError($"Call stack when aura count changed: {System.Environment.StackTrace}");
            }
        }
    }
    
    [ContextMenu("Check for Aura Components")]
    public void CheckForAuraComponents()
    {
        Start();
    }
}
