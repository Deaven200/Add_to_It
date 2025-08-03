using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the UI shown when a level is successfully completed.
/// </summary>
public class LevelCompleteUI : MonoBehaviour
{
    private int nextLevelIndex;

    /// <summary>
    /// Shows the panel, saves progress, and prepares the "Next Level" button.
    /// </summary>
    /// <param name="levelToUnlockIndex">The build index of the next level to unlock.</param>
    public void ShowScreen(int levelToUnlockIndex)
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        this.nextLevelIndex = levelToUnlockIndex;
        ProgressManager.Instance.UnlockLevel(levelToUnlockIndex);
    }

    /// <summary>
    /// Loads the next level in the sequence.
    /// </summary>
    public void OnNextLevelButtonPressed()
    {
        Time.timeScale = 1f;
        // Note: This assumes you have a scene named "Level_X" where X is the index.
        // You'll need to create more level scenes for this to work.
        SceneManager.LoadScene("Level_" + nextLevelIndex);
    }

    /// <summary>
    /// Returns the player to the main menu.
    /// </summary>
    public void OnMainMenuButtonPressed()
    {
        Time.timeScale = 1f;
        GameSceneManager.Instance.LoadMainMenu();
    }
}