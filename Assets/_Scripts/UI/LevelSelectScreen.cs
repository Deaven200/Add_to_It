using UnityEngine;

/// <summary>
/// Manages the Level Select screen, dynamically creating buttons for available levels.
/// </summary>
public class LevelSelectScreen : MonoBehaviour
{
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private int totalLevels = 3; // Set this to how many levels you have

    void Start()
    {
        int highestLevelUnlocked = ProgressManager.Instance.GetHighestLevelUnlocked();

        // Create a button for each level
        for (int i = 1; i <= totalLevels; i++)
        {
            GameObject buttonGO = Instantiate(levelButtonPrefab, buttonContainer);
            LevelButton levelButton = buttonGO.GetComponent<LevelButton>();

            // Setup the button, unlocking it if its index is <= the highest unlocked level
            bool isUnlocked = i <= highestLevelUnlocked;
            levelButton.Setup(i, isUnlocked);
        }
    }

    /// <summary>
    /// Called by the "Back" button to return to the main menu.
    /// </summary>
    public void OnBackButtonPressed()
    {
        GameSceneManager.Instance.LoadMainMenu();
    }
}