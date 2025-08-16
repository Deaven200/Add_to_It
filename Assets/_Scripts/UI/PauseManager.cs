using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles pause menu button interactions.
/// This script should be attached to the PauseMenuPanel GameObject.
/// </summary>
public class PauseManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIManager uiManager;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugging = false; // Disabled by default for performance
    
    void Start()
    {
        Debug.Log($"PauseManager: Start() called in scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        
        // Ensure EventSystem exists (only once at startup)
        EnsureEventSystemExists();
        
        // Always use the singleton instance of UIManager
        // This ensures we get the persistent UIManager that survives scene changes
        uiManager = UIManager.Instance;
        
        if (uiManager == null)
        {
            Debug.LogError("PauseManager: UIManager.Instance is null! Pause menu buttons will not work.");
        }
        else if (enableDebugging)
        {
            Debug.Log("PauseManager: UIManager found successfully via singleton");
        }
        
        // Debug button setup (only if debugging is enabled)
        if (enableDebugging)
        {
            DebugButtonSetup();
        }
        
        Debug.Log($"PauseManager: Start() completed in scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
    }
    
    // Called when the scene changes
    void OnEnable()
    {
        Debug.Log($"PauseManager: OnEnable() called in scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        EnsureEventSystemExists();
    }
    
    void EnsureEventSystemExists()
    {
        if (EventSystem.current == null)
        {
            EventSystem existingEventSystem = FindObjectOfType<EventSystem>();
            if (existingEventSystem != null)
            {
                EventSystem.current = existingEventSystem;
                if (enableDebugging)
                {
                    Debug.Log($"PauseManager: Set EventSystem.current to {existingEventSystem.name}");
                }
            }
            else
            {
                Debug.LogWarning("PauseManager: No EventSystem found in scene! Creating one automatically...");
                
                // Create EventSystem automatically
                GameObject eventSystemGO = new GameObject("EventSystem");
                EventSystem newEventSystem = eventSystemGO.AddComponent<EventSystem>();
                eventSystemGO.AddComponent<StandaloneInputModule>();
                
                // Set it as current
                EventSystem.current = newEventSystem;
                
                if (enableDebugging)
                {
                    Debug.Log($"PauseManager: Created EventSystem: {eventSystemGO.name}");
                }
            }
        }
    }
    
    void DebugButtonSetup()
    {
        if (!enableDebugging) return; // Early exit if debugging is disabled
        
        Button[] buttons = GetComponentsInChildren<Button>();
        Debug.Log($"PauseManager: Found {buttons.Length} buttons in pause menu");
        
        foreach (Button button in buttons)
        {
            int eventCount = button.onClick.GetPersistentEventCount();
            Debug.Log($"PauseManager: Button '{button.name}' has {eventCount} click events");
            
            if (eventCount == 0)
            {
                Debug.LogWarning($"PauseManager: Button '{button.name}' has no click events! It won't work.");
            }
            else
            {
                for (int i = 0; i < eventCount; i++)
                {
                    var target = button.onClick.GetPersistentTarget(i);
                    var methodName = button.onClick.GetPersistentMethodName(i);
                    Debug.Log($"PauseManager: Button '{button.name}' event {i}: {target?.GetType().Name}.{methodName}");
                }
            }
        }
    }
    
    /// <summary>
    /// Called when the "Resume" button is clicked.
    /// </summary>
    public void OnResumeButtonPressed()
    {
        Debug.Log($"PauseManager: OnResumeButtonPressed called in scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        
        if (uiManager != null)
        {
            uiManager.ResumeGame();
        }
        else
        {
            Debug.LogError("PauseManager: UIManager is null! Cannot resume game.");
        }
    }
    
    /// <summary>
    /// Called when the "Settings" button is clicked.
    /// </summary>
    public void OnSettingsButtonPressed()
    {
        Debug.Log($"PauseManager: OnSettingsButtonPressed called in scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        
        if (uiManager != null)
        {
            uiManager.OnSettingsButtonPressed();
        }
        else
        {
            Debug.LogError("PauseManager: UIManager is null! Cannot open settings.");
        }
    }
    
    /// <summary>
    /// Called when the "Quit to Main Menu" button is clicked.
    /// </summary>
    public void OnQuitToMainMenuButtonPressed()
    {
        Debug.Log($"PauseManager: OnQuitToMainMenuButtonPressed called in scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        
        if (uiManager != null)
        {
            uiManager.QuitToMainMenu();
        }
        else
        {
            Debug.LogError("PauseManager: UIManager is null! Cannot quit to main menu.");
        }
    }
    
    /// <summary>
    /// Called when the "Quit Game" button is clicked.
    /// </summary>
    public void OnQuitGameButtonPressed()
    {
        Debug.Log($"PauseManager: OnQuitGameButtonPressed called in scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    [ContextMenu("Test Resume Button")]
    public void TestResumeButton()
    {
        Debug.Log("PauseManager: Testing resume button...");
        OnResumeButtonPressed();
    }
    
    [ContextMenu("Test Settings Button")]
    public void TestSettingsButton()
    {
        Debug.Log("PauseManager: Testing settings button...");
        OnSettingsButtonPressed();
    }
    
    [ContextMenu("Test Quit to Main Menu Button")]
    public void TestQuitToMainMenuButton()
    {
        Debug.Log("PauseManager: Testing quit to main menu button...");
        OnQuitToMainMenuButtonPressed();
    }
    
    [ContextMenu("Check Button Connections")]
    public void CheckButtonConnections()
    {
        DebugButtonSetup();
    }
    
    [ContextMenu("Test UIManager Connection")]
    public void TestUIManagerConnection()
    {
        Debug.Log("=== PauseManager UIManager Connection Test ===");
        
        if (uiManager == null)
        {
            Debug.LogError("❌ UIManager reference is null!");
            
            // Try to get it again
            uiManager = UIManager.Instance;
            if (uiManager == null)
            {
                Debug.LogError("❌ UIManager.Instance is also null!");
            }
            else
            {
                Debug.Log("✅ Successfully reconnected to UIManager.Instance");
            }
        }
        else
        {
            Debug.Log("✅ UIManager reference is valid");
            Debug.Log($"UIManager GameObject: {uiManager.gameObject.name}");
            Debug.Log($"UIManager is paused: {uiManager.IsPaused()}");
        }
        
        Debug.Log("=== Connection Test Complete ===");
    }
    
    [ContextMenu("Test All Pause Menu Functions")]
    public void TestAllPauseMenuFunctions()
    {
        Debug.Log("=== Testing All Pause Menu Functions ===");
        
        // Test each button function
        OnResumeButtonPressed();
        OnSettingsButtonPressed();
        OnQuitToMainMenuButtonPressed();
        OnQuitGameButtonPressed();
        
        Debug.Log("=== All Functions Tested ===");
    }
    
    [ContextMenu("Test Button Interactions")]
    public void TestButtonInteractions()
    {
        Debug.Log("=== TESTING BUTTON INTERACTIONS ===");
        
        Button[] buttons = GetComponentsInChildren<Button>();
        Debug.Log($"Found {buttons.Length} buttons to test");
        
        foreach (Button button in buttons)
        {
            Debug.Log($"Button '{button.name}':");
            Debug.Log($"  - Interactable: {button.interactable}");
            Debug.Log($"  - Enabled: {button.enabled}");
            Debug.Log($"  - GameObject Active: {button.gameObject.activeInHierarchy}");
            Debug.Log($"  - OnClick Events Count: {button.onClick.GetPersistentEventCount()}");
            
            // Test if button can be clicked programmatically
            if (button.interactable && button.enabled && button.gameObject.activeInHierarchy)
            {
                Debug.Log($"  - ✅ Button appears clickable");
            }
            else
            {
                Debug.Log($"  - ❌ Button is NOT clickable");
            }
        }
        
        // Test EventSystem
        if (EventSystem.current != null)
        {
            Debug.Log($"EventSystem.current: {EventSystem.current.name}");
            Debug.Log($"EventSystem enabled: {EventSystem.current.enabled}");
        }
        else
        {
            Debug.LogError("EventSystem.current is null!");
        }
        
        Debug.Log("=== BUTTON INTERACTION TEST COMPLETE ===");
    }
    
    [ContextMenu("Force Reconnect to UIManager")]
    public void ForceReconnectToUIManager()
    {
        Debug.Log("=== FORCING UIMANAGER RECONNECTION ===");
        
        // Force reconnection
        uiManager = UIManager.Instance;
        
        if (uiManager == null)
        {
            Debug.LogError("❌ UIManager.Instance is still null after force reconnect!");
        }
        else
        {
            Debug.Log("✅ Successfully reconnected to UIManager.Instance");
            Debug.Log($"UIManager GameObject: {uiManager.gameObject.name}");
        }
        
        Debug.Log("=== RECONNECTION COMPLETE ===");
    }
}
