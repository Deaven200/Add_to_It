using UnityEngine;

/// <summary>
/// Manages saving and loading player progress, such as unlocked levels.
/// Uses PlayerPrefs for simple data storage.
/// </summary>
public class ProgressManager : MonoBehaviour
{
    // The Singleton instance
    public static ProgressManager Instance
    {
        get; private set;
    }

    // The key we'll use to save our data in PlayerPrefs.
    // Using a constant string prevents typos.
    private const string HighestLevelUnlockedKey = "HighestLevelUnlocked";

    private void Awake()
    {
        // Standard Singleton and persistence setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Saves that a new level has been unlocked.
    /// It only saves if the new level is higher than the previously saved one.
    /// </summary>
    /// <param name="levelNumber">The build index of the level being unlocked.</param>
    public void UnlockLevel(int levelNumber)
    {
        int currentHighestLevel = GetHighestLevelUnlocked();

        if (levelNumber > currentHighestLevel)
        {
            PlayerPrefs.SetInt(HighestLevelUnlockedKey, levelNumber);
            PlayerPrefs.Save(); // Immediately writes data to disk
            // New highest level unlocked
        }
    }

    /// <summary>
    /// Retrieves the highest level the player has unlocked.
    /// </summary>
    /// <returns>The build index of the highest unlocked level.</returns>
    public int GetHighestLevelUnlocked()
    {
        // PlayerPrefs.GetInt takes a key and a default value.
        // If the key doesn't exist (e.g., first time playing), it will return 1.
        return PlayerPrefs.GetInt(HighestLevelUnlockedKey, 1);
    }
}