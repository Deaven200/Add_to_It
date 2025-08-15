using UnityEngine;

public class PlayerAuraSetup : MonoBehaviour
{
    [Header("Aura System Setup")]
    [SerializeField] private bool autoSetupAuraSystem = true;
    [SerializeField] private LayerMask enemyLayerMask = 1; // Default layer
    [SerializeField] private LayerMask coinLayerMask = 1; // Default layer
    
    void Start()
    {
        if (autoSetupAuraSystem)
        {
            SetupAuraSystem();
        }
    }
    
    void SetupAuraSystem()
    {
        // Find the parent player GameObject
        GameObject playerObject = FindParentPlayer();
        if (playerObject == null)
        {
            Debug.LogError("PlayerAuraSetup: Could not find parent player GameObject!");
            return;
        }
        
        // Check if AuraSystem already exists on the parent
        AuraSystem existingAuraSystem = playerObject.GetComponent<AuraSystem>();
        if (existingAuraSystem != null)
        {
            return;
        }
        
        // Add AuraSystem component to the parent player
        AuraSystem auraSystem = playerObject.AddComponent<AuraSystem>();
        
        // Add PlayerUpgradeTracker component
        PlayerUpgradeTracker upgradeTracker = playerObject.GetComponent<PlayerUpgradeTracker>();
        if (upgradeTracker == null)
        {
            upgradeTracker = playerObject.AddComponent<PlayerUpgradeTracker>();
        }
        
        if (auraSystem != null)
        {
            // Aura system setup complete
        }
        else
        {
            Debug.LogError("PlayerAuraSetup: Failed to add AuraSystem component!");
        }
    }
    
    private GameObject FindParentPlayer()
    {
        // First, try to find a parent with "Player" tag
        Transform current = transform;
        while (current != null)
        {
            if (current.CompareTag("Player"))
            {
                return current.gameObject;
            }
            current = current.parent;
        }
        
        // If no parent with "Player" tag, try to find by name
        current = transform;
        while (current != null)
        {
            if (current.name.ToLower().Contains("player"))
            {
                return current.gameObject;
            }
            current = current.parent;
        }
        
        // If still not found, return the root parent
        return transform.root.gameObject;
    }
    

    
    // Public method to manually setup aura system
    [ContextMenu("Setup Aura System")]
    public void ManualSetupAuraSystem()
    {
        SetupAuraSystem();
    }
    
    // Public method to remove aura system (for testing)
    [ContextMenu("Remove Aura System")]
    public void RemoveAuraSystem()
    {
        GameObject playerObject = FindParentPlayer();
        if (playerObject != null)
        {
            AuraSystem auraSystem = playerObject.GetComponent<AuraSystem>();
            if (auraSystem != null)
            {
                DestroyImmediate(auraSystem);
            }
        }
    }
}
