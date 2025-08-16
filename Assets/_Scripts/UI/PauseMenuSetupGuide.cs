using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Setup guide for fixing pause menu button connections.
/// This script provides instructions and can help verify the setup.
/// </summary>
public class PauseMenuSetupGuide : MonoBehaviour
{
    [Header("Setup Instructions")]
    [TextArea(10, 20)]
    [SerializeField] private string setupInstructions = @"
PAUSE MENU SETUP GUIDE:

1. Make sure the PauseManager script is attached to the PauseMenuPanel GameObject.

2. For each button in the pause menu, set up the OnClick events:
   
   RESUME BUTTON:
   - Select the Resume button
   - In the Inspector, find the Button component
   - In the OnClick() section, click the + button
   - Drag the PauseMenuPanel (with PauseManager) to the Object field
   - Select PauseManager > OnResumeButtonPressed() from the function dropdown
   
   SETTINGS BUTTON:
   - Select the Settings button
   - In the OnClick() section, click the + button
   - Drag the PauseMenuPanel (with PauseManager) to the Object field
   - Select PauseManager > OnSettingsButtonPressed() from the function dropdown
   
   QUIT TO MAIN MENU BUTTON:
   - Select the Quit to Main Menu button
   - In the OnClick() section, click the + button
   - Drag the PauseMenuPanel (with PauseManager) to the Object field
   - Select PauseManager > OnQuitToMainMenuButtonPressed() from the function dropdown
   
   QUIT GAME BUTTON:
   - Select the Quit Game button
   - In the OnClick() section, click the + button
   - Drag the PauseMenuPanel (with PauseManager) to the Object field
   - Select PauseManager > OnQuitGameButtonPressed() from the function dropdown

3. Make sure the UIManager is properly set up in the scene.

4. Test the pause menu by pressing ESC in play mode.
";

    [Header("Verification")]
    [SerializeField] private bool verifySetupOnStart = true;
    
    void Start()
    {
        if (verifySetupOnStart)
        {
            VerifySetup();
        }
    }
    
    [ContextMenu("Verify Pause Menu Setup")]
    public void VerifySetup()
    {
        Debug.Log("=== PAUSE MENU SETUP VERIFICATION ===");
        
        // Check PauseManager
        PauseManager pauseManager = GetComponent<PauseManager>();
        if (pauseManager == null)
        {
            Debug.LogError("❌ PauseManager component not found on this GameObject!");
        }
        else
        {
            Debug.Log("✅ PauseManager component found");
        }
        
        // Check UIManager
        UIManager uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("❌ UIManager.Instance is null!");
        }
        else
        {
            Debug.Log("✅ UIManager.Instance found");
        }
        
        // Check buttons
        Button[] buttons = GetComponentsInChildren<Button>();
        Debug.Log($"Found {buttons.Length} buttons in pause menu");
        
        foreach (Button button in buttons)
        {
            string buttonName = button.name.ToLower();
            int eventCount = button.onClick.GetPersistentEventCount();
            
            if (eventCount == 0)
            {
                Debug.LogError($"❌ Button '{button.name}' has no click events!");
            }
            else
            {
                Debug.Log($"✅ Button '{button.name}' has {eventCount} click events");
                
                // Check if the events are properly connected to PauseManager
                for (int i = 0; i < eventCount; i++)
                {
                    var target = button.onClick.GetPersistentTarget(i);
                    var methodName = button.onClick.GetPersistentMethodName(i);
                    
                    if (target == null)
                    {
                        Debug.LogError($"❌ Button '{button.name}' event {i} has no target!");
                    }
                    else if (target is PauseManager)
                    {
                        Debug.Log($"✅ Button '{button.name}' event {i} correctly connected to PauseManager.{methodName}");
                    }
                    else
                    {
                        Debug.LogWarning($"⚠️ Button '{button.name}' event {i} connected to {target.GetType().Name}.{methodName} (should be PauseManager)");
                    }
                }
            }
        }
        
        Debug.Log("=== VERIFICATION COMPLETE ===");
    }
    
    [ContextMenu("Print Setup Instructions")]
    public void PrintInstructions()
    {
        Debug.Log(setupInstructions);
    }
    
    [ContextMenu("Test Pause Menu Functionality")]
    public void TestPauseMenuFunctionality()
    {
        Debug.Log("=== PAUSE MENU FUNCTIONALITY TEST ===");
        
        // Test UIManager singleton
        UIManager uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("❌ UIManager.Instance is null! This is a critical error.");
            return;
        }
        else
        {
            Debug.Log("✅ UIManager.Instance found");
        }
        
        // Test PauseManager
        PauseManager pauseManager = GetComponent<PauseManager>();
        if (pauseManager == null)
        {
            Debug.LogError("❌ PauseManager component not found!");
            return;
        }
        else
        {
            Debug.Log("✅ PauseManager component found");
        }
        
        // Test pause functionality
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
        
        // Test resume functionality
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
        
        Debug.Log("=== FUNCTIONALITY TEST COMPLETE ===");
    }
    
    [ContextMenu("Check Scene Setup")]
    public void CheckSceneSetup()
    {
        Debug.Log("=== SCENE SETUP CHECK ===");
        
        // Check for EventSystem
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("❌ No EventSystem found in scene! UI buttons will not work.");
        }
        else
        {
            Debug.Log($"✅ EventSystem found: {eventSystem.name}");
        }
        
        // Check for Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ No Canvas found in scene!");
        }
        else
        {
            Debug.Log($"✅ Canvas found: {canvas.name}");
        }
        
        // Check for GraphicRaycaster
        GraphicRaycaster raycaster = FindObjectOfType<GraphicRaycaster>();
        if (raycaster == null)
        {
            Debug.LogError("❌ No GraphicRaycaster found in scene! UI interactions will not work.");
        }
        else
        {
            Debug.Log($"✅ GraphicRaycaster found: {raycaster.name}");
        }
        
        Debug.Log("=== SCENE SETUP CHECK COMPLETE ===");
    }
}
