using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages scene loading and unloading for the entire game.
/// Implemented as a Singleton to ensure only one instance exists.
/// </summary>
public class GameSceneManager : MonoBehaviour
{
    // A static reference to the single instance of this class.
    // 'static' means this variable belongs to the class itself, not an instance of the class.
    public static GameSceneManager Instance
    {
        get; private set;
    }

    // The Awake method is called when the script instance is being loaded.
    // It's called before Start().
    private void Awake()
    {
        // This is the core of the Singleton pattern.
        // If an instance of this manager already exists...
        if (Instance != null && Instance != this)
        {
            // ...then destroy this new one. This ensures we never have duplicates.
            Destroy(gameObject);
        }
        else
        {
            // Otherwise, this is the one and only instance.
            Instance = this;

            // Mark this GameObject to not be destroyed when loading a new scene.
            // This is crucial for our manager to persist across the game.
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Loads a scene by its build name (e.g., "Level_01").
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void LoadLevel(string sceneName)
    {
        // Before you can load a scene by name, you must add it to the Build Settings.
        // Go to File > Build Settings > Scenes In Build, and drag your scenes in.
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Loads the main menu scene.
    /// </summary>
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Quits the application.
    /// Note: This only works in a built game, not in the Unity Editor.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("QUIT GAME: Application.Quit() called."); // For testing in the editor
        Application.Quit();
    }
}