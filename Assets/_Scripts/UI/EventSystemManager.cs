using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Ensures that an EventSystem exists in the scene for UI interactions
/// This script should be added to any scene that needs UI functionality
/// </summary>
public class EventSystemManager : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool autoCreateEventSystem = true;
    [SerializeField] private bool logDebugInfo = true;
    
    void Awake()
    {
        if (autoCreateEventSystem)
        {
            EnsureEventSystemExists();
        }
    }
    
    void Start()
    {
        if (autoCreateEventSystem)
        {
            // Double-check in Start to make sure EventSystem is working
            if (EventSystem.current == null)
            {
                Debug.LogWarning("EventSystemManager: EventSystem.current is null in Start! Recreating...");
                EnsureEventSystemExists();
            }
            else if (logDebugInfo)
            {
                Debug.Log($"EventSystemManager: EventSystem is working - {EventSystem.current.name}");
            }
        }
    }
    
    public void EnsureEventSystemExists()
    {
        // Check if there's already an EventSystem in the scene
        EventSystem existingEventSystem = FindObjectOfType<EventSystem>();
        
        if (existingEventSystem == null)
        {
            if (logDebugInfo)
            {
                Debug.Log("EventSystemManager: No EventSystem found! Creating one...");
            }
            
            // Create EventSystem GameObject
            GameObject eventSystemGO = new GameObject("EventSystem");
            
            // Add EventSystem component
            EventSystem eventSystem = eventSystemGO.AddComponent<EventSystem>();
            
            // Add StandaloneInputModule for input handling
            StandaloneInputModule inputModule = eventSystemGO.AddComponent<StandaloneInputModule>();
            
            // Set as current EventSystem
            EventSystem.current = eventSystem;
            
            if (logDebugInfo)
            {
                Debug.Log("EventSystemManager: EventSystem created successfully!");
            }
        }
        else
        {
            if (logDebugInfo)
            {
                Debug.Log($"EventSystemManager: EventSystem found: {existingEventSystem.name}");
            }
            
            // Make sure it's set as current
            EventSystem.current = existingEventSystem;
        }
        
        // Verify EventSystem is working
        if (EventSystem.current == null)
        {
            Debug.LogError("EventSystemManager: EventSystem.current is still null after creation! This is a critical error.");
        }
        else if (logDebugInfo)
        {
            Debug.Log($"EventSystemManager: EventSystem.current is now: {EventSystem.current.name}");
        }
    }
    
    // Public method to manually trigger EventSystem creation
    [ContextMenu("Create EventSystem")]
    public void CreateEventSystem()
    {
        EnsureEventSystemExists();
    }
    
    // Public method to check EventSystem status
    [ContextMenu("Check EventSystem Status")]
    public void CheckEventSystemStatus()
    {
        if (EventSystem.current == null)
        {
            Debug.LogError("EventSystemManager: EventSystem.current is NULL!");
        }
        else
        {
            Debug.Log($"EventSystemManager: EventSystem.current is {EventSystem.current.name}");
            
            // Check if it has required components
            StandaloneInputModule inputModule = EventSystem.current.GetComponent<StandaloneInputModule>();
            if (inputModule == null)
            {
                Debug.LogError("EventSystemManager: EventSystem is missing StandaloneInputModule!");
            }
            else
            {
                Debug.Log("EventSystemManager: EventSystem has StandaloneInputModule ✓");
            }
        }
    }
}
