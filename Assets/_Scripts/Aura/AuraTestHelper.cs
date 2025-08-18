using UnityEngine;

public class AuraTestHelper : MonoBehaviour
{
    [Header("Test Controls")]
    [SerializeField] private KeyCode testCoinMagnetKey = KeyCode.Alpha1;
    [SerializeField] private KeyCode testSlowAuraKey = KeyCode.Alpha2;
    [SerializeField] private KeyCode testAllAurasKey = KeyCode.Alpha0;
    [SerializeField] private KeyCode removeAllAurasKey = KeyCode.R;
    
    [Header("Test Values")]
    [SerializeField] private float testAuraValue = 5f;
    [SerializeField] private UpgradeData.Rarity testRarity = UpgradeData.Rarity.Common;
    
    private AuraSystem auraSystem;
    
    void Start()
    {
        // Find the aura system on the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            auraSystem = player.GetComponent<AuraSystem>();
            if (auraSystem == null)
            {
                Debug.LogWarning("AuraSystem not found on player. Make sure PlayerAuraSetup is added to the player.");
            }
        }
        else
        {
            Debug.LogWarning("Player not found. Make sure the player has the 'Player' tag.");
        }
    }
    
    void Update()
    {
        if (auraSystem == null) return;
        
        // Test individual auras
        if (Input.GetKeyDown(testCoinMagnetKey))
        {
            TestCoinMagnetAura();
        }
        
        if (Input.GetKeyDown(testSlowAuraKey))
        {
            TestSlowAura();
        }
        
        if (Input.GetKeyDown(testAllAurasKey))
        {
            TestAllAuras();
        }
        
        if (Input.GetKeyDown(removeAllAurasKey))
        {
            RemoveAllAuras();
        }
    }
    
    void TestCoinMagnetAura()
    {
        auraSystem.AddAura(UpgradeData.UpgradeType.CoinMagnetAura, testAuraValue, testRarity);
        Debug.Log($"Added Coin Magnet Aura with value {testAuraValue} and rarity {testRarity}");
    }
    
    void TestSlowAura()
    {
        auraSystem.AddAura(UpgradeData.UpgradeType.SlowAura, testAuraValue, testRarity);
        Debug.Log($"Added Slow Aura with value {testAuraValue} and rarity {testRarity}");
    }
    
    void TestAllAuras()
    {
        auraSystem.TestAddAllAuras();
        Debug.Log("Added all auras for testing");
    }
    
    void RemoveAllAuras()
    {
        auraSystem.RemoveAllAuras();
        Debug.Log("Removed all auras");
    }
    
    // Context menu methods for testing in editor
    [ContextMenu("Test Coin Magnet Aura")]
    public void TestCoinMagnetAuraEditor()
    {
        if (Application.isPlaying && auraSystem != null)
        {
            TestCoinMagnetAura();
        }
    }
    
    [ContextMenu("Test Slow Aura")]
    public void TestSlowAuraEditor()
    {
        if (Application.isPlaying && auraSystem != null)
        {
            TestSlowAura();
        }
    }
    
    [ContextMenu("Test All Auras")]
    public void TestAllAurasEditor()
    {
        if (Application.isPlaying && auraSystem != null)
        {
            TestAllAuras();
        }
    }
    
    [ContextMenu("Remove All Auras")]
    public void RemoveAllAurasEditor()
    {
        if (Application.isPlaying && auraSystem != null)
        {
            RemoveAllAuras();
        }
    }
    
    void OnGUI()
    {
        if (!Application.isPlaying) return;
        
        // Display test controls on screen
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("Aura System Test Controls:");
        GUILayout.Label($"1: Coin Magnet Aura");
        GUILayout.Label($"2: Slow Aura");
        GUILayout.Label($"0: All Auras");
        GUILayout.Label($"R: Remove All");
        GUILayout.Label($"Active Auras: {auraSystem?.GetActiveAuraCount() ?? 0}");
        GUILayout.EndArea();
    }
}
