using UnityEngine;

/// <summary>
/// Manages the pause state of the game.
/// Handles showing/hiding the pause menu and controlling game time.
/// </summary>
public class PauseManager : MonoBehaviour
{
    // A reference to the Pause Menu UI Panel.
    // Assign this in the Unity Inspector.
    [SerializeField]
    private GameObject pauseMenuPanel;

    [SerializeField] private GameObject settingsPanelPrefab;
    [SerializeField] private GameObject canvas;
    [SerializeField] private SimpleUpgradeDisplay upgradeDisplay;
    private GameObject _activeSettingsInstance;

    // Tracks the current paused state of the game.
    private bool isPaused = false;
    
    void Start()
    {
        // Try to find the upgrade display if not assigned
        if (upgradeDisplay == null)
        {
            upgradeDisplay = FindObjectOfType<SimpleUpgradeDisplay>();
            if (upgradeDisplay != null)
            {
                Debug.Log("PauseManager: Found SimpleUpgradeDisplay automatically");
            }
            else
            {
                Debug.LogWarning("PauseManager: SimpleUpgradeDisplay not found! Upgrade display won't work.");
            }
        }
    }

    // The Update method is called once per frame.
    void Update()
    {
        // Listen for the "Escape" key press.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If the game is already paused, resume it. Otherwise, pause it.
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    /// <summary>
    /// Pauses the game, shows the menu, and stops time.
    /// </summary>
    public void PauseGame()
    {
        isPaused = true;
        // Set the game's time scale to 0, which freezes all physics-based movement and animations.
        Time.timeScale = 0f;
        // Activate the pause menu UI.
        pauseMenuPanel.SetActive(true);

        // Refresh upgrade display if available
        if (upgradeDisplay != null)
        {
            Debug.Log("PauseManager: Calling upgradeDisplay.OnPauseMenuOpen()");
            upgradeDisplay.OnPauseMenuOpen();
        }
        else
        {
            Debug.LogWarning("PauseManager: upgradeDisplay is null! Cannot refresh upgrade display.");
        }

        // Unlock the cursor and make it visible so we can click buttons.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Resumes the game, hides the menu, and restores time.
    /// This is public so our "Continue" button can call it.
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;
        // Set the time scale back to 1 to resume normal game speed.
        Time.timeScale = 1f;
        // Deactivate the pause menu UI.
        pauseMenuPanel.SetActive(false);

        // Refresh upgrade display if available
        if (upgradeDisplay != null)
        {
            Debug.Log("PauseManager: Calling upgradeDisplay.OnPauseMenuClose()");
            upgradeDisplay.OnPauseMenuClose();
        }

        // Re-lock the cursor and hide it for FPS controls.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Loads the main menu scene using our persistent GameSceneManager.
    /// This is public so our "Quit" button can call it.
    /// </summary>
    public void QuitToMainMenu()
    {
        // We must un-pause the game before leaving the scene to ensure
        // the time scale is normal when we come back.
        Time.timeScale = 1f;

        // Ensure the cursor is unlocked and visible before going to the main menu.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameSceneManager.Instance.LoadMainMenu();
    }

    public void OnSettingsButtonPressed()
    {
        // Create or reuse settings panel
        // Only create a new settings panel if one isn't already active.
        if (_activeSettingsInstance == null)
        {
            _activeSettingsInstance = Instantiate(settingsPanelPrefab, canvas.transform);
        }
    }
}