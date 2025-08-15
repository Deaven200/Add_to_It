using UnityEngine;

public class PlayerAuraSetup : MonoBehaviour
{
    [Header("Aura System Setup")]
    [SerializeField] private bool autoSetupAuraSystem = true;
    [SerializeField] private LayerMask enemyLayerMask = 1; // Default layer
    [SerializeField] private LayerMask coinLayerMask = 1; // Default layer
    
    void Awake()
    {
        Debug.Log("PlayerAuraSetup: Awake() called - Script is working!");
    }
    
    void Start()
    {
        Debug.Log("PlayerAuraSetup: Start() called on " + gameObject.name);
        
        if (autoSetupAuraSystem)
        {
            Debug.Log("PlayerAuraSetup: Auto setup enabled, calling SetupAuraSystem()");
            SetupAuraSystem();
        }
        else
        {
            Debug.Log("PlayerAuraSetup: Auto setup disabled");
        }
    }
    
    void SetupAuraSystem()
    {
        Debug.Log("PlayerAuraSetup: SetupAuraSystem() called");
        
        // Find the parent player GameObject
        GameObject playerObject = FindParentPlayer();
        if (playerObject == null)
        {
            Debug.LogError("PlayerAuraSetup: Could not find parent player GameObject!");
            return;
        }
        
        Debug.Log("PlayerAuraSetup: Found parent player: " + playerObject.name);
        
        // Check if AuraSystem already exists on the parent
        AuraSystem existingAuraSystem = playerObject.GetComponent<AuraSystem>();
        if (existingAuraSystem != null)
        {
            Debug.Log("PlayerAuraSetup: AuraSystem already exists on parent player.");
            return;
        }
        
        Debug.Log("PlayerAuraSetup: Adding AuraSystem component to parent player...");
        
        // Add AuraSystem component to the parent player
        AuraSystem auraSystem = playerObject.AddComponent<AuraSystem>();
        
        if (auraSystem != null)
        {
            Debug.Log("PlayerAuraSetup: AuraSystem added to parent player successfully!");
            
            // Test adding a simple aura
            StartCoroutine(TestAuraAfterDelay(playerObject));
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
    
    private System.Collections.IEnumerator TestAuraAfterDelay(GameObject playerObject)
    {
        yield return new WaitForSeconds(2f); // Wait 2 seconds
        
        AuraSystem auraSystem = playerObject.GetComponent<AuraSystem>();
        if (auraSystem != null)
        {
            Debug.Log("PlayerAuraSetup: Testing aura system by adding a coin magnet aura...");
            auraSystem.AddAura(UpgradeData.UpgradeType.CoinMagnetAura, 5f, UpgradeData.Rarity.Common);
        }
    }
    
    // Public method to manually setup aura system
    [ContextMenu("Setup Aura System")]
    public void ManualSetupAuraSystem()
    {
        Debug.Log("PlayerAuraSetup: Manual setup called");
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
                Debug.Log("PlayerAuraSetup: AuraSystem removed from parent player.");
            }
            else
            {
                Debug.Log("PlayerAuraSetup: No AuraSystem found on parent player.");
            }
        }
    }
}
