using UnityEngine;

public class CoinDebugTest : MonoBehaviour
{
    [Header("Coin Debug")]
    [SerializeField] private KeyCode debugKey = KeyCode.C;
    
    void Update()
    {
        if (Input.GetKeyDown(debugKey))
        {
            DebugCoins();
        }
    }
    
    void DebugCoins()
    {
        Debug.Log("=== COIN DEBUG TEST ===");
        
        // Find all objects with "Money" tag
        GameObject[] moneyObjects = GameObject.FindGameObjectsWithTag("Money");
        Debug.Log($"Found {moneyObjects.Length} objects with 'Money' tag");
        
        foreach (GameObject money in moneyObjects)
        {
            Collider collider = money.GetComponent<Collider>();
            Rigidbody rb = money.GetComponent<Rigidbody>();
            
            Debug.Log($"Coin: {money.name}");
            Debug.Log($"  - Tag: {money.tag}");
            Debug.Log($"  - Layer: {money.layer} ({LayerMask.LayerToName(money.layer)})");
            Debug.Log($"  - Has Collider: {collider != null}");
            Debug.Log($"  - Has Rigidbody: {rb != null}");
            Debug.Log($"  - Collider isTrigger: {(collider != null ? collider.isTrigger.ToString() : "N/A")}");
            Debug.Log($"  - Position: {money.transform.position}");
            
            if (collider != null)
            {
                Debug.Log($"  - Collider bounds: {collider.bounds}");
            }
        }
        
        // Check AuraSystem layer mask
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            AuraSystem auraSystem = player.GetComponent<AuraSystem>();
            if (auraSystem != null)
            {
                Debug.Log($"AuraSystem found on player");
                // We can't access private fields, but we can check if auras exist
                Debug.Log($"Active auras: {auraSystem.GetActiveAuraCount()}");
            }
        }
        
        Debug.Log("=== END COIN DEBUG ===");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 250, 300, 100));
        GUILayout.Label("Coin Debug Test:");
        GUILayout.Label($"Press {debugKey} to debug coins");
        GUILayout.EndArea();
    }
}
