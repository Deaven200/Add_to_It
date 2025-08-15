using UnityEngine;

public class FixCoinColliders : MonoBehaviour
{
    [Header("Coin Fix Settings")]
    [SerializeField] private KeyCode fixKey = KeyCode.F;
    [SerializeField] private bool autoFixOnStart = true;
    
    void Start()
    {
        if (autoFixOnStart)
        {
            FixAllCoinColliders();
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(fixKey))
        {
            FixAllCoinColliders();
        }
    }
    
    void FixAllCoinColliders()
    {
        Debug.Log("=== FIXING COIN COLLIDERS ===");
        
        // Find all objects with "Money" tag
        GameObject[] moneyObjects = GameObject.FindGameObjectsWithTag("Money");
        int fixedCount = 0;
        
        foreach (GameObject money in moneyObjects)
        {
            Collider collider = money.GetComponent<Collider>();
            if (collider != null && collider.isTrigger)
            {
                collider.isTrigger = false;
                Debug.Log($"Fixed collider on {money.name} - set isTrigger to false");
                fixedCount++;
            }
        }
        
        Debug.Log($"Fixed {fixedCount} coin colliders");
        Debug.Log("=== COIN COLLIDERS FIXED ===");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 350, 300, 100));
        GUILayout.Label("Coin Collider Fix:");
        GUILayout.Label($"Press {fixKey} to fix coin colliders");
        GUILayout.Label("(Auto-fix on start enabled)");
        GUILayout.EndArea();
    }
}
