using UnityEngine;

/// <summary>
/// Handles the UI logic for the Main Menu screen.
/// This script should be attached to the Canvas of the MainMenu scene.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    // A constant holding the name of the first level scene.
    // Using a constant prevents typos when calling the SceneManager.
    private const string FIRST_LEVEL_SCENE_NAME = "PlayerRoom";

    [SerializeField] private GameObject settingsPanelPrefab;
    private GameObject _activeSettingsInstance;

    /// <summary>
    /// Called when the "Start" button is clicked.
    /// Loads the first playable level.
    /// </summary>
    public void OnStartButtonPressed()
    {
        // We access the Singleton instance of our GameSceneManager
        // and call its public method to load the level.
        // Load the first playable level
        GameSceneManager.Instance.LoadLevel(FIRST_LEVEL_SCENE_NAME);
    }

    /// <summary>
    /// Called when the "Level Select" button is clicked.
    /// </summary>
    public void OnLevelSelectButtonPressed()
    {
        GameSceneManager.Instance.LoadLevel("LevelSelect");
    }

    /// <summary>
    /// Called when the "Quit" button is clicked.
    /// </summary>
    public void OnQuitButtonPressed()
    {
        // Quit the game
        GameSceneManager.Instance.QuitGame();
    }

    public void OnSettingsButtonPressed()
    {
        // Only create a new settings panel if one isn't already active.
        if (_activeSettingsInstance == null)
        {
            _activeSettingsInstance = Instantiate(settingsPanelPrefab, transform.parent);
        }
    }
}