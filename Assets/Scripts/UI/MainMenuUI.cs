using UnityEngine;

/// <summary>
/// Handles the UI logic for the Main Menu screen.
/// This script should be attached to the Canvas of the MainMenu scene.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    // A constant holding the name of the first level scene.
    // Using a constant prevents typos when calling the SceneManager.
    private const string FIRST_LEVEL_SCENE_NAME = "Level_01";

    /// <summary>
    /// Called when the "Start" button is clicked.
    /// Loads the first playable level.
    /// </summary>
    public void OnStartButtonPressed()
    {
        // We access the Singleton instance of our GameSceneManager
        // and call its public method to load the level.
        Debug.Log("Start Button Pressed. Loading level...");
        GameSceneManager.Instance.LoadLevel(FIRST_LEVEL_SCENE_NAME);
    }

    /// <summary>
    /// Called when the "Level Select" button is clicked.
    /// (For now, this is a placeholder)
    /// </summary>
    public void OnLevelSelectButtonPressed()
    {
        // We'll implement this later. For now, a log is useful for testing.
        Debug.Log("Level Select Button Pressed. (Not Implemented)");
    }

    /// <summary>
    /// Called when the "Quit" button is clicked.
    /// </summary>
    public void OnQuitButtonPressed()
    {
        Debug.Log("Quit Button Pressed.");
        GameSceneManager.Instance.QuitGame();
    }
}