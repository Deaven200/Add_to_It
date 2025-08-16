using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic; // Added for List
using System.Linq; // Added for Select

/// <summary>
/// Comprehensive troubleshooting guide for pause menu issues.
/// This script helps diagnose and fix common problems with the pause menu system.
/// </summary>
public class PauseMenuTroubleshootingGuide : MonoBehaviour
{
    [Header("Troubleshooting Options")]
    [SerializeField] private bool autoCheckOnStart = true;
    [SerializeField] private bool enableVerboseLogging = false;
    
    void Start()
    {
        if (autoCheckOnStart)
        {
            RunFullDiagnostic();
        }
    }
    
    [ContextMenu("Run Full Diagnostic")]
    public void RunFullDiagnostic()
    {
        Debug.Log("=== PAUSE MENU FULL DIAGNOSTIC ===");
        
        // Step 1: Check UIManager Singleton
        CheckUIManagerSingleton();
        
        // Step 2: Check PauseManager Setup
        CheckPauseManagerSetup();
        
        // Step 3: Check UI Button Connections
        CheckButtonConnections();
        
        // Step 4: Check Scene Requirements
        CheckSceneRequirements();
        
        // Step 5: Test Functionality
        TestPauseMenuFunctionality();
        
        Debug.Log("=== DIAGNOSTIC COMPLETE ===");
    }
    
    private void CheckUIManagerSingleton()
    {
        Debug.Log("--- Checking UIManager Singleton ---");
        
        UIManager uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("❌ CRITICAL: UIManager.Instance is null!");
            Debug.LogError("This means the UIManager singleton is not properly initialized.");
            Debug.LogError("Make sure there is a UIManager GameObject in your scene with DontDestroyOnLoad.");
            return;
        }
        
        Debug.Log("✅ UIManager singleton found");
        Debug.Log($"UIManager GameObject: {uiManager.gameObject.name}");
        Debug.Log($"UIManager is in DontDestroyOnLoad: {uiManager.gameObject.scene.name == "DontDestroyOnLoad"}");
        
        if (enableVerboseLogging)
        {
            Debug.Log($"Current pause state: {uiManager.IsPaused()}");
            Debug.Log($"Current scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        }
    }
    
    private void CheckPauseManagerSetup()
    {
        Debug.Log("--- Checking PauseManager Setup ---");
        
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager == null)
        {
            Debug.LogError("❌ No PauseManager found in scene!");
            Debug.LogError("Make sure the PauseManager script is attached to your PauseMenuPanel GameObject.");
            return;
        }
        
        Debug.Log("✅ PauseManager found");
        Debug.Log($"PauseManager GameObject: {pauseManager.gameObject.name}");
        
        // Check if PauseManager is on the correct GameObject
        if (!pauseManager.gameObject.name.ToLower().Contains("pause"))
        {
            Debug.LogWarning("⚠️ PauseManager is not on a pause-related GameObject");
            Debug.LogWarning("Consider moving PauseManager to a GameObject named 'PauseMenuPanel'");
        }
        
        if (enableVerboseLogging)
        {
            // Test UIManager connection
            pauseManager.SendMessage("TestUIManagerConnection", SendMessageOptions.DontRequireReceiver);
        }
    }
    
    private void CheckButtonConnections()
    {
        Debug.Log("--- Checking Button Connections ---");
        
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager == null) return;
        
        Button[] buttons = pauseManager.GetComponentsInChildren<Button>();
        Debug.Log($"Found {buttons.Length} buttons in pause menu");
        
        if (buttons.Length == 0)
        {
            Debug.LogError("❌ No buttons found in pause menu!");
            Debug.LogError("Make sure your pause menu has Button components.");
            return;
        }
        
        foreach (Button button in buttons)
        {
            int eventCount = button.onClick.GetPersistentEventCount();
            string buttonName = button.name;
            
            if (eventCount == 0)
            {
                Debug.LogError($"❌ Button '{buttonName}' has no click events!");
                Debug.LogError($"Set up the OnClick event for '{buttonName}' in the Inspector.");
            }
            else
            {
                Debug.Log($"✅ Button '{buttonName}' has {eventCount} click events");
                
                // Check if events are connected to PauseManager
                bool connectedToPauseManager = false;
                for (int i = 0; i < eventCount; i++)
                {
                    var target = button.onClick.GetPersistentTarget(i);
                    var methodName = button.onClick.GetPersistentMethodName(i);
                    
                    if (target is PauseManager)
                    {
                        connectedToPauseManager = true;
                        Debug.Log($"  ✅ Event {i}: Connected to PauseManager.{methodName}");
                    }
                    else
                    {
                        Debug.LogWarning($"  ⚠️ Event {i}: Connected to {target?.GetType().Name}.{methodName} (should be PauseManager)");
                    }
                }
                
                if (!connectedToPauseManager)
                {
                    Debug.LogError($"❌ Button '{buttonName}' is not connected to PauseManager!");
                    Debug.LogError($"Connect '{buttonName}' to PauseManager methods in the Inspector.");
                }
            }
        }
    }
    
    private void CheckSceneRequirements()
    {
        Debug.Log("--- Checking Scene Requirements ---");
        
        // Check EventSystem
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("❌ No EventSystem found in scene!");
            Debug.LogError("UI buttons will not work without an EventSystem.");
            Debug.LogError("Add an EventSystem GameObject to your scene.");
        }
        else
        {
            Debug.Log("✅ EventSystem found");
            Debug.Log($"EventSystem GameObject: {eventSystem.name}");
        }
        
        // Check Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ No Canvas found in scene!");
            Debug.LogError("UI elements need a Canvas to render properly.");
        }
        else
        {
            Debug.Log("✅ Canvas found");
            Debug.Log($"Canvas GameObject: {canvas.name}");
            Debug.Log($"Canvas Render Mode: {canvas.renderMode}");
        }
        
        // Check GraphicRaycaster
        GraphicRaycaster raycaster = FindObjectOfType<GraphicRaycaster>();
        if (raycaster == null)
        {
            Debug.LogError("❌ No GraphicRaycaster found in scene!");
            Debug.LogError("UI interactions will not work without a GraphicRaycaster.");
            Debug.LogError("Add a GraphicRaycaster component to your Canvas.");
        }
        else
        {
            Debug.Log("✅ GraphicRaycaster found");
            Debug.Log($"GraphicRaycaster GameObject: {raycaster.name}");
        }
    }
    
    private void TestPauseMenuFunctionality()
    {
        Debug.Log("--- Testing Pause Menu Functionality ---");
        
        UIManager uiManager = UIManager.Instance;
        if (uiManager == null) return;
        
        // Test pause
        Debug.Log("Testing pause functionality...");
        uiManager.PauseGame();
        
        if (uiManager.IsPaused())
        {
            Debug.Log("✅ Pause functionality works");
        }
        else
        {
            Debug.LogError("❌ Pause functionality failed");
        }
        
        // Test resume
        Debug.Log("Testing resume functionality...");
        uiManager.ResumeGame();
        
        if (!uiManager.IsPaused())
        {
            Debug.Log("✅ Resume functionality works");
        }
        else
        {
            Debug.LogError("❌ Resume functionality failed");
        }
        
        // Test button methods
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager != null)
        {
            Debug.Log("Testing button methods...");
            
            // Test each button method
            pauseManager.OnResumeButtonPressed();
            pauseManager.OnSettingsButtonPressed();
            pauseManager.OnQuitToMainMenuButtonPressed();
            pauseManager.OnQuitGameButtonPressed();
            
            Debug.Log("✅ All button methods executed successfully");
        }
    }
    
    [ContextMenu("Quick Fix: Reconnect PauseManager to UIManager")]
    public void QuickFixReconnectPauseManager()
    {
        Debug.Log("--- Quick Fix: Reconnecting PauseManager to UIManager ---");
        
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager == null)
        {
            Debug.LogError("❌ No PauseManager found to fix!");
            return;
        }
        
        // Force reconnection by calling Start method
        pauseManager.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
        
        Debug.Log("✅ PauseManager reconnection attempted");
        Debug.Log("Check the console for any error messages.");
    }
    
    [ContextMenu("Quick Fix: Ensure EventSystem")]
    public void QuickFixEnsureEventSystem()
    {
        Debug.Log("--- Quick Fix: Ensuring EventSystem ---");
        
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("❌ No EventSystem found! Creating one...");
            
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
            
            Debug.Log("✅ EventSystem created");
        }
        else
        {
            Debug.Log("✅ EventSystem already exists");
        }
    }
    
    [ContextMenu("Print Setup Instructions")]
    public void PrintSetupInstructions()
    {
        Debug.Log(@"
=== PAUSE MENU SETUP INSTRUCTIONS ===

1. UIMANAGER SETUP:
   - Create a GameObject named 'UIManager'
   - Attach the UIManager script to it
   - The script will automatically use DontDestroyOnLoad

2. PAUSE MANAGER SETUP:
   - Create a GameObject named 'PauseMenuPanel'
   - Attach the PauseManager script to it
   - Make sure it's a child of a Canvas

3. BUTTON SETUP:
   - For each button in your pause menu:
     * Select the button in the Inspector
     * In the Button component's OnClick() section
     * Click the + button to add an event
     * Drag the PauseMenuPanel (with PauseManager) to the Object field
     * Select the appropriate PauseManager method:
       - OnResumeButtonPressed() for Resume button
       - OnSettingsButtonPressed() for Settings button
       - OnQuitToMainMenuButtonPressed() for Quit to Main Menu button
       - OnQuitGameButtonPressed() for Quit Game button

4. SCENE REQUIREMENTS:
   - EventSystem GameObject (automatically created by Unity UI)
   - Canvas with GraphicRaycaster component
   - PauseMenuPanel as child of Canvas

5. TESTING:
   - Press ESC to pause/unpause
   - Click pause menu buttons to test functionality
   - Use this troubleshooting guide to diagnose issues

=== SETUP COMPLETE ===");
    }
    
    [ContextMenu("Deep Button Interaction Test")]
    public void DeepButtonInteractionTest()
    {
        Debug.Log("=== DEEP BUTTON INTERACTION TEST ===");
        
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager == null)
        {
            Debug.LogError("❌ No PauseManager found!");
            return;
        }
        
        Button[] buttons = pauseManager.GetComponentsInChildren<Button>();
        Debug.Log($"Testing {buttons.Length} buttons...");
        
        foreach (Button button in buttons)
        {
            Debug.Log($"--- Testing Button: {button.name} ---");
            
            // Check if button is interactable
            Debug.Log($"Interactable: {button.interactable}");
            
            // Check if button is active
            Debug.Log($"GameObject Active: {button.gameObject.activeInHierarchy}");
            
            // Check if button has a graphic
            Debug.Log($"Has Graphic: {button.targetGraphic != null}");
            
            // Check if button is under a canvas
            Canvas parentCanvas = button.GetComponentInParent<Canvas>();
            Debug.Log($"Parent Canvas: {(parentCanvas != null ? parentCanvas.name : "None")}");
            
            // Check if button is under a GraphicRaycaster
            GraphicRaycaster parentRaycaster = button.GetComponentInParent<GraphicRaycaster>();
            Debug.Log($"Parent GraphicRaycaster: {(parentRaycaster != null ? parentRaycaster.name : "None")}");
            
            // Test button click programmatically
            Debug.Log("Testing programmatic button click...");
            button.onClick.Invoke();
            
            Debug.Log($"--- End Test for {button.name} ---");
        }
        
        Debug.Log("=== DEEP BUTTON TEST COMPLETE ===");
    }
    
    [ContextMenu("Test EventSystem in Detail")]
    public void TestEventSystemInDetail()
    {
        Debug.Log("=== DETAILED EVENTSYSTEM TEST ===");
        
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            Debug.LogError("❌ EventSystem.current is null!");
            
            EventSystem[] allEventSystems = FindObjectsOfType<EventSystem>();
            Debug.Log($"Found {allEventSystems.Length} EventSystems in scene:");
            foreach (EventSystem es in allEventSystems)
            {
                Debug.Log($"  - {es.name} (Active: {es.gameObject.activeInHierarchy})");
            }
            return;
        }
        
        Debug.Log($"✅ EventSystem.current: {eventSystem.name}");
        Debug.Log($"EventSystem Active: {eventSystem.gameObject.activeInHierarchy}");
        Debug.Log($"EventSystem Enabled: {eventSystem.enabled}");
        
        // Check input module
        BaseInputModule inputModule = eventSystem.GetComponent<BaseInputModule>();
        if (inputModule == null)
        {
            Debug.LogError("❌ No BaseInputModule found on EventSystem!");
        }
        else
        {
            Debug.Log($"✅ Input Module: {inputModule.GetType().Name}");
            Debug.Log($"Input Module Enabled: {inputModule.enabled}");
        }
        
        // Test if EventSystem can find UI elements
        Debug.Log("Testing EventSystem UI detection...");
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = new Vector2(Screen.width / 2, Screen.height / 2);
        
        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerData, results);
        
        Debug.Log($"EventSystem found {results.Count} UI elements at screen center:");
        foreach (RaycastResult result in results)
        {
            Debug.Log($"  - {result.gameObject.name} (Layer: {result.gameObject.layer})");
        }
        
        Debug.Log("=== EVENTSYSTEM TEST COMPLETE ===");
    }
    
    [ContextMenu("Test Pause Menu in New Scene")]
    public void TestPauseMenuInNewScene()
    {
        Debug.Log("=== TESTING PAUSE MENU IN NEW SCENE ===");
        
        // First, test current scene
        Debug.Log("--- Current Scene Test ---");
        TestPauseMenuFunctionality();
        
        // Then test after a scene change simulation
        Debug.Log("--- Simulating Scene Change ---");
        
        // Force UIManager to re-find UI elements
        UIManager uiManager = UIManager.Instance;
        if (uiManager != null)
        {
            // Force UIManager to re-find UI elements
            uiManager.FindUIReferencesInNewScene();
            uiManager.VerifyPauseMenuSetup();
        }
        
        // Test again after "scene change"
        Debug.Log("--- After Scene Change Test ---");
        TestPauseMenuFunctionality();
        
        Debug.Log("=== NEW SCENE TEST COMPLETE ===");
    }
    
    [ContextMenu("Simple Scene Change Test")]
    public void SimpleSceneChangeTest()
    {
        Debug.Log("=== SIMPLE SCENE CHANGE TEST ===");
        
        // Test current state
        Debug.Log("--- Current State ---");
        UIManager uiManager = UIManager.Instance;
        if (uiManager != null)
        {
            Debug.Log($"UIManager found: {uiManager.gameObject.name}");
            Debug.Log($"Pause state: {uiManager.IsPaused()}");
        }
        
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager != null)
        {
            Debug.Log($"PauseManager found: {pauseManager.gameObject.name}");
            
            // Test button interactions
            Debug.Log("Testing button interactions...");
            pauseManager.TestButtonInteractions();
        }
        else
        {
            Debug.LogError("No PauseManager found in scene!");
        }
        
        Debug.Log("=== SIMPLE TEST COMPLETE ===");
    }
    
    [ContextMenu("Test Pause Menu in Scene 3")]
    public void TestPauseMenuInScene3()
    {
        Debug.Log("=== TESTING PAUSE MENU IN SCENE 3 ===");
        
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Debug.Log($"Current scene: {currentScene}");
        
        // Test UIManager
        UIManager uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("❌ UIManager.Instance is null!");
            return;
        }
        Debug.Log("✅ UIManager.Instance found");
        
        // Test PauseManager
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager == null)
        {
            Debug.LogError("❌ PauseManager not found in scene!");
            return;
        }
        Debug.Log("✅ PauseManager found");
        
        // Test EventSystem
        if (EventSystem.current == null)
        {
            Debug.LogError("❌ EventSystem.current is null!");
            return;
        }
        Debug.Log($"✅ EventSystem.current: {EventSystem.current.name}");
        
        // Test if pause menu can be opened
        Debug.Log("Testing pause menu open...");
        uiManager.PauseGame();
        
        // Wait a frame then test buttons
        StartCoroutine(TestButtonsAfterPause());
    }
    
    private System.Collections.IEnumerator TestButtonsAfterPause()
    {
        yield return null; // Wait one frame
        
        Debug.Log("Testing button interactions...");
        
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager != null)
        {
            pauseManager.TestButtonInteractions();
        }
        
        // Test a button click programmatically
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            if (button.name == "ContinueButton")
            {
                Debug.Log($"Testing ContinueButton click programmatically...");
                button.onClick.Invoke();
                break;
            }
        }
        
        // Test EventSystem input handling
        TestEventSystemInput();
    }
    
    private void TestEventSystemInput()
    {
        Debug.Log("=== TESTING EVENTSYSTEM INPUT ===");
        
        if (EventSystem.current == null)
        {
            Debug.LogError("❌ EventSystem.current is null!");
            return;
        }
        
        EventSystem eventSystem = EventSystem.current;
        Debug.Log($"EventSystem: {eventSystem.name}");
        Debug.Log($"EventSystem enabled: {eventSystem.enabled}");
        Debug.Log($"EventSystem GameObject active: {eventSystem.gameObject.activeInHierarchy}");
        
        // Check input modules
        StandaloneInputModule inputModule = eventSystem.GetComponent<StandaloneInputModule>();
        if (inputModule == null)
        {
            Debug.LogError("❌ No StandaloneInputModule found on EventSystem!");
        }
        else
        {
            Debug.Log($"✅ StandaloneInputModule found and enabled: {inputModule.enabled}");
        }
        
        // Check if mouse is over UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("✅ Mouse is over UI element");
        }
        else
        {
            Debug.Log("⚠️ Mouse is NOT over UI element");
        }
        
        // Check cursor state
        Debug.Log($"Cursor visible: {Cursor.visible}");
        Debug.Log($"Cursor lock state: {Cursor.lockState}");
        
        Debug.Log("=== EVENTSYSTEM INPUT TEST COMPLETE ===");
    }

    [ContextMenu("Check for EventSystem Conflicts")]
    public void CheckForEventSystemConflicts()
    {
        Debug.Log("=== CHECKING FOR EVENTSYSTEM CONFLICTS ===");
        
        EventSystem[] allEventSystems = FindObjectsOfType<EventSystem>();
        Debug.Log($"Found {allEventSystems.Length} EventSystem(s) in scene");
        
        for (int i = 0; i < allEventSystems.Length; i++)
        {
            EventSystem es = allEventSystems[i];
            Debug.Log($"EventSystem {i + 1}: {es.name}");
            Debug.Log($"  - Enabled: {es.enabled}");
            Debug.Log($"  - GameObject Active: {es.gameObject.activeInHierarchy}");
            Debug.Log($"  - Is Current: {es == EventSystem.current}");
            Debug.Log($"  - Has StandaloneInputModule: {es.GetComponent<StandaloneInputModule>() != null}");
        }
        
        if (allEventSystems.Length > 1)
        {
            Debug.LogWarning("⚠️ Multiple EventSystems found! This can cause conflicts.");
            Debug.LogWarning("Only the first EventSystem should be active.");
        }
        
        // Check if current EventSystem is properly set
        if (EventSystem.current == null)
        {
            Debug.LogError("❌ EventSystem.current is null!");
        }
        else
        {
            Debug.Log($"✅ EventSystem.current is set to: {EventSystem.current.name}");
        }
        
        Debug.Log("=== EVENTSYSTEM CONFLICT CHECK COMPLETE ===");
    }

    [ContextMenu("Quick Fix: Move PauseManager to Correct GameObject")]
    public void QuickFixMovePauseManager()
    {
        Debug.Log("--- Quick Fix: Moving PauseManager to Correct GameObject ---");
        
        // Find PauseManager
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager == null)
        {
            Debug.LogError("❌ No PauseManager found!");
            return;
        }
        
        // Find PauseMenuPanel
        GameObject pauseMenuPanel = GameObject.Find("PauseMenuPanel");
        if (pauseMenuPanel == null)
        {
            Debug.LogError("❌ No PauseMenuPanel found!");
            return;
        }
        
        // Check if PauseManager is already on the correct GameObject
        if (pauseManager.gameObject == pauseMenuPanel)
        {
            Debug.Log("✅ PauseManager is already on the correct GameObject");
            return;
        }
        
        // Check if PauseMenuPanel already has a PauseManager
        PauseManager existingPauseManager = pauseMenuPanel.GetComponent<PauseManager>();
        if (existingPauseManager != null)
        {
            Debug.LogWarning("⚠️ PauseMenuPanel already has a PauseManager! Removing the duplicate...");
            DestroyImmediate(existingPauseManager);
        }
        
        // Move PauseManager to PauseMenuPanel
        Debug.Log($"Moving PauseManager from {pauseManager.gameObject.name} to {pauseMenuPanel.name}");
        
        // Create a new PauseManager on the correct GameObject
        PauseManager newPauseManager = pauseMenuPanel.AddComponent<PauseManager>();
        
        // Remove the old PauseManager
        DestroyImmediate(pauseManager);
        
        Debug.Log("✅ PauseManager moved successfully!");
        Debug.Log("The pause menu should now work correctly.");
    }

    [ContextMenu("Check for Scene 3 Crash Issues")]
    public void CheckForScene3CrashIssues()
    {
        Debug.Log("=== CHECKING FOR SCENE 3 CRASH ISSUES ===");
        
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Debug.Log($"Current scene: {currentScene}");
        
        // Check for multiple PauseManagers
        PauseManager[] allPauseManagers = FindObjectsOfType<PauseManager>();
        Debug.Log($"Found {allPauseManagers.Length} PauseManager(s) in scene");
        
        if (allPauseManagers.Length > 1)
        {
            Debug.LogError("❌ Multiple PauseManagers found! This can cause crashes.");
            for (int i = 0; i < allPauseManagers.Length; i++)
            {
                Debug.LogError($"  PauseManager {i + 1}: {allPauseManagers[i].gameObject.name}");
            }
        }
        
        // Check for PauseManager on wrong GameObject
        foreach (PauseManager pm in allPauseManagers)
        {
            if (pm.gameObject.name == "UIManager")
            {
                Debug.LogError("❌ PauseManager is on UIManager GameObject! This can cause crashes.");
                Debug.LogError("Move PauseManager to PauseMenuPanel GameObject.");
            }
        }
        
        // Check for missing essential components
        if (FindObjectOfType<Canvas>() == null)
        {
            Debug.LogError("❌ No Canvas found! This can cause crashes.");
        }
        
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogError("❌ No EventSystem found! This can cause crashes.");
        }
        
        // Check for null references
        UIManager uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("❌ UIManager.Instance is null! This can cause crashes.");
        }
        
        Debug.Log("=== CRASH CHECK COMPLETE ===");
    }

    [ContextMenu("Quick Fix: Add Canvas to Player for Pause Menu")]
    public void QuickFixAddCanvasToPlayer()
    {
        Debug.Log("--- Quick Fix: Adding Canvas to Player for Pause Menu ---");
        
        // Find the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            // Try to find by name
            player = GameObject.Find("Player");
            if (player == null)
            {
                player = GameObject.Find("PlayerController");
            }
        }
        
        if (player == null)
        {
            Debug.LogError("❌ No player found! Make sure your player has the 'Player' tag or is named 'Player'.");
            return;
        }
        
        Debug.Log($"Found player: {player.name}");
        
        // Check if player already has Canvas
        Canvas existingCanvas = player.GetComponent<Canvas>();
        if (existingCanvas != null)
        {
            Debug.Log("✅ Player already has Canvas component");
        }
        else
        {
            // Add Canvas to player
            Canvas newCanvas = player.AddComponent<Canvas>();
            newCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            newCanvas.sortingOrder = 100; // Ensure it renders on top
            Debug.Log("✅ Added Canvas component to player");
        }
        
        // Check if player has GraphicRaycaster
        GraphicRaycaster existingRaycaster = player.GetComponent<GraphicRaycaster>();
        if (existingRaycaster != null)
        {
            Debug.Log("✅ Player already has GraphicRaycaster component");
        }
        else
        {
            // Add GraphicRaycaster to player
            player.AddComponent<GraphicRaycaster>();
            Debug.Log("✅ Added GraphicRaycaster component to player");
        }
        
        // Check if player has PauseManager
        PauseManager existingPauseManager = player.GetComponent<PauseManager>();
        if (existingPauseManager != null)
        {
            Debug.Log("✅ Player already has PauseManager component");
        }
        else
        {
            Debug.LogWarning("⚠️ Player does not have PauseManager component");
            Debug.LogWarning("Add the PauseManager script to the player GameObject");
        }
        
        // Ensure EventSystem exists
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogWarning("⚠️ No EventSystem found! Creating one...");
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
            Debug.Log("✅ Created EventSystem");
        }
        else
        {
            Debug.Log("✅ EventSystem already exists");
        }
        
        Debug.Log("✅ Player pause menu setup complete!");
        Debug.Log("The pause menu should now work correctly.");
    }

    [ContextMenu("Analyze Working Scene 2 Pause Menu")]
    public void AnalyzeWorkingScene2PauseMenu()
    {
        Debug.Log("=== ANALYZING WORKING SCENE 2 PAUSE MENU ===");
        
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Debug.Log($"Current scene: {currentScene}");
        
        // Check for UIManager
        UIManager uiManager = UIManager.Instance;
        if (uiManager != null)
        {
            Debug.Log("✅ UIManager found in scene");
            Debug.Log($"UIManager GameObject: {uiManager.gameObject.name}");
        }
        else
        {
            Debug.Log("❌ No UIManager found - using different system");
        }
        
        // Check for PauseManager
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager != null)
        {
            Debug.Log("✅ PauseManager found in scene");
            Debug.Log($"PauseManager GameObject: {pauseManager.gameObject.name}");
        }
        else
        {
            Debug.Log("❌ No PauseManager found - using different system");
        }
        
        // Check for Canvas
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        Debug.Log($"Found {allCanvases.Length} Canvas(es) in scene:");
        foreach (Canvas canvas in allCanvases)
        {
            Debug.Log($"  - {canvas.name} (RenderMode: {canvas.renderMode})");
        }
        
        // Check for EventSystem
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem != null)
        {
            Debug.Log($"✅ EventSystem found: {eventSystem.name}");
        }
        else
        {
            Debug.Log("❌ No EventSystem found");
        }
        
        // Check for pause menu buttons
        Button[] allButtons = FindObjectsOfType<Button>();
        Debug.Log($"Found {allButtons.Length} Button(s) in scene:");
        foreach (Button button in allButtons)
        {
            Debug.Log($"  - {button.name} (Parent: {button.transform.parent?.name})");
            int eventCount = button.onClick.GetPersistentEventCount();
            Debug.Log($"    Events: {eventCount}");
        }
        
        // Check for pause-related GameObjects
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains("pause") || obj.name.ToLower().Contains("menu"))
            {
                Debug.Log($"Found pause-related GameObject: {obj.name}");
                Debug.Log($"  Parent: {obj.transform.parent?.name}");
                Debug.Log($"  Components: {string.Join(", ", obj.GetComponents<Component>().Select(c => c.GetType().Name))}");
            }
        }
        
        Debug.Log("=== SCENE 2 ANALYSIS COMPLETE ===");
        Debug.Log("Use this information to set up Scene 3 the same way.");
    }
}
