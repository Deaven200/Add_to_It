using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quick fix script to automatically set up pause menu button connections.
/// Attach this to the PauseMenuPanel and run the context menu options.
/// </summary>
public class PauseMenuQuickFix : MonoBehaviour
{
    [Header("Button Names")]
    [SerializeField] private string resumeButtonName = "ResumeButton";
    [SerializeField] private string settingsButtonName = "SettingsButton";
    [SerializeField] private string quitToMainMenuButtonName = "QuitToMainMenuButton";
    [SerializeField] private string quitGameButtonName = "QuitGameButton";
    
    private PauseManager pauseManager;
    
    void Start()
    {
        pauseManager = GetComponent<PauseManager>();
        if (pauseManager == null)
        {
            Debug.LogError("PauseMenuQuickFix: No PauseManager found on this GameObject!");
        }
    }
    
    [ContextMenu("Auto-Setup All Buttons")]
    public void AutoSetupAllButtons()
    {
        Debug.Log("PauseMenuQuickFix: Starting auto-setup of all buttons...");
        
        SetupButton(resumeButtonName, "OnResumeButtonPressed");
        SetupButton(settingsButtonName, "OnSettingsButtonPressed");
        SetupButton(quitToMainMenuButtonName, "OnQuitToMainMenuButtonPressed");
        SetupButton(quitGameButtonName, "OnQuitGameButtonPressed");
        
        Debug.Log("PauseMenuQuickFix: Auto-setup complete!");
    }
    
    [ContextMenu("Setup Resume Button")]
    public void SetupResumeButton()
    {
        SetupButton(resumeButtonName, "OnResumeButtonPressed");
    }
    
    [ContextMenu("Setup Settings Button")]
    public void SetupSettingsButton()
    {
        SetupButton(settingsButtonName, "OnSettingsButtonPressed");
    }
    
    [ContextMenu("Setup Quit to Main Menu Button")]
    public void SetupQuitToMainMenuButton()
    {
        SetupButton(quitToMainMenuButtonName, "OnQuitToMainMenuButtonPressed");
    }
    
    [ContextMenu("Setup Quit Game Button")]
    public void SetupQuitGameButton()
    {
        SetupButton(quitGameButtonName, "OnQuitGameButtonPressed");
    }
    
    private void SetupButton(string buttonName, string methodName)
    {
        if (pauseManager == null)
        {
            Debug.LogError("PauseMenuQuickFix: PauseManager is null! Cannot setup buttons.");
            return;
        }
        
        // Find the button by name
        Button button = FindButtonByName(buttonName);
        if (button == null)
        {
            Debug.LogWarning($"PauseMenuQuickFix: Button '{buttonName}' not found!");
            return;
        }
        
        // Clear existing events
        button.onClick.RemoveAllListeners();
        
        // Add the correct event
        switch (methodName)
        {
            case "OnResumeButtonPressed":
                button.onClick.AddListener(pauseManager.OnResumeButtonPressed);
                break;
            case "OnSettingsButtonPressed":
                button.onClick.AddListener(pauseManager.OnSettingsButtonPressed);
                break;
            case "OnQuitToMainMenuButtonPressed":
                button.onClick.AddListener(pauseManager.OnQuitToMainMenuButtonPressed);
                break;
            case "OnQuitGameButtonPressed":
                button.onClick.AddListener(pauseManager.OnQuitGameButtonPressed);
                break;
            default:
                Debug.LogError($"PauseMenuQuickFix: Unknown method name: {methodName}");
                return;
        }
        
        Debug.Log($"PauseMenuQuickFix: Successfully setup {buttonName} to call {methodName}");
    }
    
    private Button FindButtonByName(string buttonName)
    {
        // First try to find by exact name
        Button[] allButtons = GetComponentsInChildren<Button>();
        
        foreach (Button button in allButtons)
        {
            if (button.name == buttonName)
            {
                return button;
            }
        }
        
        // If not found, try partial name matching
        foreach (Button button in allButtons)
        {
            if (button.name.ToLower().Contains(buttonName.ToLower()))
            {
                Debug.Log($"PauseMenuQuickFix: Found button '{button.name}' (partial match for '{buttonName}')");
                return button;
            }
        }
        
        return null;
    }
    
    [ContextMenu("List All Buttons")]
    public void ListAllButtons()
    {
        Button[] allButtons = GetComponentsInChildren<Button>();
        Debug.Log($"PauseMenuQuickFix: Found {allButtons.Length} buttons:");
        
        foreach (Button button in allButtons)
        {
            int eventCount = button.onClick.GetPersistentEventCount();
            Debug.Log($"  - {button.name} ({eventCount} events)");
        }
    }
}
