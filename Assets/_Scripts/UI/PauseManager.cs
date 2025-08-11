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
    private GameObject _activeSettingsInstance;

    // Tracks the current paused state of the game.
    private bool isPaused = false;

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