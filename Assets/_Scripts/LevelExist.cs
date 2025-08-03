using UnityEngine;

/// <summary>
/// Triggers the level completion UI when the player enters this volume.
/// </summary>
public class LevelExit : MonoBehaviour
{
    [SerializeField] private LevelCompleteUI levelCompleteUI;
    [SerializeField] private int nextLevelBuildIndex = 2;

    /// <summary>
    /// This method is called by Unity automatically when another collider enters this trigger.
    /// </summary>
    /// <param name="other">The collider of the object that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered has the "Player" tag.
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has reached the exit!");
            levelCompleteUI.ShowScreen(nextLevelBuildIndex);

            // Disable the trigger so it doesn't fire multiple times.
            gameObject.SetActive(false);
        }
    }
}