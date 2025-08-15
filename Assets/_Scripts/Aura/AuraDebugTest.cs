using UnityEngine;

public class AuraDebugTest : MonoBehaviour
{
    [Header("Debug Controls")]
    [SerializeField] private KeyCode testKey = KeyCode.T;
    [SerializeField] private bool showDebugInfo = true;
    
    private AuraSystem auraSystem;
    private PlayerAuraSetup playerAuraSetup;
    private GameObject playerObject;
    
    void Start()
    {
        // Find the player
        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("AuraDebugTest: No GameObject with 'Player' tag found!");
            return;
        }
        
        // Get components
        auraSystem = playerObject.GetComponent<AuraSystem>();
        
        // Look for PlayerAuraSetup in children
        playerAuraSetup = playerObject.GetComponentInChildren<PlayerAuraSetup>();
        
        Debug.Log($"AuraDebugTest: Player found: {playerObject.name}");
        Debug.Log($"AuraDebugTest: AuraSystem found: {auraSystem != null}");
        Debug.Log($"AuraDebugTest: PlayerAuraSetup found: {playerAuraSetup != null}");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            TestAuraSystem();
        }
    }
    
    void TestAuraSystem()
    {
        Debug.Log("=== AURA SYSTEM DEBUG TEST ===");
        
        // Check if player exists
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null)
            {
                Debug.LogError("No player found!");
                return;
            }
        }
        
        // Check components
        auraSystem = playerObject.GetComponent<AuraSystem>();
        playerAuraSetup = playerObject.GetComponentInChildren<PlayerAuraSetup>();
        
        Debug.Log($"Player: {playerObject.name}");
        Debug.Log($"AuraSystem: {(auraSystem != null ? "FOUND" : "MISSING")}");
        Debug.Log($"PlayerAuraSetup: {(playerAuraSetup != null ? "FOUND" : "MISSING")}");
        
        // Try to add aura system if missing
        if (auraSystem == null && playerAuraSetup != null)
        {
            Debug.Log("Trying to manually setup aura system...");
            playerAuraSetup.ManualSetupAuraSystem();
            
            // Check again
            auraSystem = playerObject.GetComponent<AuraSystem>();
            Debug.Log($"AuraSystem after manual setup: {(auraSystem != null ? "FOUND" : "STILL MISSING")}");
        }
        
        // Test adding an aura
        if (auraSystem != null)
        {
            Debug.Log("Testing aura addition...");
            auraSystem.AddAura(UpgradeData.UpgradeType.CoinMagnetAura, 5f, UpgradeData.Rarity.Common);
            Debug.Log($"Active auras: {auraSystem.GetActiveAuraCount()}");
        }
        
        Debug.Log("=== END DEBUG TEST ===");
    }
    
    void OnGUI()
    {
        if (!showDebugInfo) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("Aura System Debug Info:");
        GUILayout.Label($"Press {testKey} to run debug test");
        
        if (playerObject != null)
        {
            AuraSystem auraSystem = playerObject.GetComponent<AuraSystem>();
            PlayerAuraSetup playerAuraSetup = playerObject.GetComponentInChildren<PlayerAuraSetup>();
            
            GUILayout.Label($"Player: {playerObject.name}");
            GUILayout.Label($"AuraSystem: {(auraSystem != null ? "✓" : "✗")}");
            GUILayout.Label($"PlayerAuraSetup: {(playerAuraSetup != null ? "✓" : "✗")}");
            
            if (auraSystem != null)
            {
                GUILayout.Label($"Active Auras: {auraSystem.GetActiveAuraCount()}");
            }
        }
        else
        {
            GUILayout.Label("Player not found!");
        }
        
        GUILayout.EndArea();
    }
}
