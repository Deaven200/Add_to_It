using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls a single button on the Level Select screen.
/// </summary>
public class LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button buttonComponent;

    private int levelIndex;

    /// <summary>
    /// Sets up the button's text, level, and interactable state.
    /// </summary>
    public void Setup(int levelNum, bool isUnlocked)
    {
        levelIndex = levelNum;
        levelText.text = "Level " + levelIndex;

        // If the level is unlocked, the button is clickable. Otherwise, it's disabled.
        buttonComponent.interactable = isUnlocked;
    }

    /// <summary>
    /// Called when the button is clicked. Loads the corresponding level.
    /// </summary>
    public void OnClick()
    {
        // We'll assume scenes are named "Level_1", "Level_2", etc.
        GameSceneManager.Instance.LoadLevel("Level_" + levelIndex);
    }
}