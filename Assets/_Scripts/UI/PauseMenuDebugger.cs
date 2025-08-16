using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Debug script to help identify pause menu issues.
/// Attach this to the PauseMenuPanel to see what's happening.
/// </summary>
public class PauseMenuDebugger : MonoBehaviour
{
    [Header("Debug Info")]
    [SerializeField] private bool showDebugInfo = true;
    
    void Start()
    {
        if (showDebugInfo)
        {
            Debug.Log("PauseMenuDebugger: Pause menu panel started");
            
            // Check if PauseManager is attached
            PauseManager pauseManager = GetComponent<PauseManager>();
            if (pauseManager == null)
            {
                Debug.LogError("PauseMenuDebugger: No PauseManager component found on this GameObject!");
            }
            else
            {
                Debug.Log("PauseMenuDebugger: PauseManager component found");
            }
            
            // Check if UIManager exists
            UIManager uiManager = UIManager.Instance;
            if (uiManager == null)
            {
                Debug.LogError("PauseMenuDebugger: UIManager.Instance is null!");
            }
            else
            {
                Debug.Log("PauseMenuDebugger: UIManager.Instance found");
            }
            
            // Check all buttons in the pause menu
            Button[] buttons = GetComponentsInChildren<Button>();
            Debug.Log($"PauseMenuDebugger: Found {buttons.Length} buttons in pause menu");
            
            foreach (Button button in buttons)
            {
                Debug.Log($"PauseMenuDebugger: Button '{button.name}' - Interactable: {button.interactable}");
                
                // Check button click events
                if (button.onClick.GetPersistentEventCount() == 0)
                {
                    Debug.LogWarning($"PauseMenuDebugger: Button '{button.name}' has no click events!");
                }
                else
                {
                    Debug.Log($"PauseMenuDebugger: Button '{button.name}' has {button.onClick.GetPersistentEventCount()} click events");
                }
            }
        }
    }
    
    void Update()
    {
        if (showDebugInfo && Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("PauseMenuDebugger: F1 pressed - checking pause state");
            
            UIManager uiManager = UIManager.Instance;
            if (uiManager != null)
            {
                Debug.Log($"PauseMenuDebugger: UIManager.IsPaused() = {uiManager.IsPaused()}");
            }
            
            UpgradeManager upgradeManager = FindObjectOfType<UpgradeManager>();
            if (upgradeManager != null)
            {
                Debug.Log($"PauseMenuDebugger: UpgradeManager.isPaused = {upgradeManager.isPaused}");
                Debug.Log($"PauseMenuDebugger: UpgradeManager.IsUpgradeMenuActive() = {upgradeManager.IsUpgradeMenuActive()}");
            }
        }
    }
}
