using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the Death Screen UI.
/// Attached to the DeathScreenPanel.
/// </summary>
public class DeathScreenUI : MonoBehaviour
{
    /// <summary>
    /// Activates the death screen, pauses the game, and shows the cursor.
    /// This should be called by the PlayerHealth script when the player dies.
    /// </summary>
    public void ShowDeathScreen()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Called when the "Retry" button is clicked.
    /// </summary>
    public void OnRetryButtonPressed()
    {
        // Un-pause the game before reloading
        Time.timeScale = 1f;
        // Reload the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Called when the "Quit to Main Menu" button is clicked.
    /// </summary>
    public void OnQuitToMainMenuPressed()
    {
        // Un-pause the game before changing scenes
        Time.timeScale = 1f;
        GameSceneManager.Instance.LoadMainMenu();
    }
}