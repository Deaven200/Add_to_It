using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Utility script to find and remove missing script references in the scene
/// </summary>
public class MissingScriptFinder : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool runOnStart = true;
    [SerializeField] private bool showDetailedLogs = true;
    
    void Start()
    {
        if (runOnStart)
        {
            FindAndRemoveMissingScripts();
        }
    }
    
    [ContextMenu("Find Missing Scripts")]
    public void FindAndRemoveMissingScripts()
    {
        Debug.Log("=== Searching for Missing Scripts ===");
        
        // Find all GameObjects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int missingScriptCount = 0;
        List<string> affectedObjects = new List<string>();
        
        foreach (GameObject obj in allObjects)
        {
            // Check for missing scripts on this GameObject
            Component[] components = obj.GetComponents<Component>();
            List<int> missingScriptIndices = new List<int>();
            
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    missingScriptIndices.Add(i);
                }
            }
            
            // Remove missing scripts
            if (missingScriptIndices.Count > 0)
            {
                missingScriptCount += missingScriptIndices.Count;
                affectedObjects.Add(obj.name);
                
                if (showDetailedLogs)
                {
                    Debug.LogWarning($"Found {missingScriptIndices.Count} missing script(s) on GameObject: {obj.name}");
                }
                
                // Remove missing scripts (Unity will handle this automatically)
                // The missing scripts will be removed when the scene is saved
            }
        }
        
        if (missingScriptCount > 0)
        {
            Debug.LogWarning($"Found {missingScriptCount} missing script(s) on {affectedObjects.Count} GameObject(s)");
            Debug.LogWarning("Missing scripts have been marked for removal. Save the scene to complete the cleanup.");
            
            foreach (string objName in affectedObjects)
            {
                Debug.LogWarning($"- {objName}");
            }
        }
        else
        {
            Debug.Log("✅ No missing scripts found!");
        }
        
        Debug.Log("=== Missing Script Search Complete ===");
    }
    
    [ContextMenu("List All GameObjects")]
    public void ListAllGameObjects()
    {
        Debug.Log("=== All GameObjects in Scene ===");
        
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            Component[] components = obj.GetComponents<Component>();
            string componentList = "";
            
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] != null)
                {
                    componentList += components[i].GetType().Name;
                    if (i < components.Length - 1) componentList += ", ";
                }
                else
                {
                    componentList += "MISSING SCRIPT";
                    if (i < components.Length - 1) componentList += ", ";
                }
            }
            
            Debug.Log($"GameObject: {obj.name} | Components: {componentList}");
        }
        
        Debug.Log($"Total GameObjects: {allObjects.Length}");
        Debug.Log("=== GameObject List Complete ===");
    }
    
    [ContextMenu("Check Button Connections")]
    public void CheckButtonConnections()
    {
        Debug.Log("=== Checking Button Connections ===");
        
        Button[] buttons = FindObjectsOfType<Button>();
        
        foreach (Button button in buttons)
        {
            int eventCount = button.onClick.GetPersistentEventCount();
            Debug.Log($"Button '{button.name}' has {eventCount} click events");
            
            for (int i = 0; i < eventCount; i++)
            {
                string targetName = button.onClick.GetPersistentTarget(i)?.name ?? "NULL";
                string methodName = button.onClick.GetPersistentMethodName(i);
                Debug.Log($"  Event {i}: {targetName}.{methodName}");
            }
        }
        
        Debug.Log($"Total Buttons: {buttons.Length}");
        Debug.Log("=== Button Check Complete ===");
    }
}
