using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Spawning")]
    [SerializeField] public GameObject playerPrefab; // Assign in Inspector
    private GameObject currentPlayer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RespawnPlayer()
    {
        SceneManager.LoadScene("PlayerRoom");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Don't spawn player in menu scenes
        if (scene.name == "MainMenu" || scene.name == "LevelSelect")
        {
            return;
        }
        
        // For PlayerRoom, use the specific spawn point
        if (scene.name == "PlayerRoom")
        {
            Transform spawnPoint = GameObject.Find("PlayerSpawnPoint")?.transform;

            if (spawnPoint != null && playerPrefab != null)
            {
                if (currentPlayer != null)
                {
                    Destroy(currentPlayer);
                }

                currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            }
            else
            {
                Debug.LogWarning("Missing PlayerSpawnPoint or PlayerPrefab!");
            }
        }
        // For other game scenes, spawn at origin or find a spawn point
        else
        {
            // Try to find a spawn point first
            Transform spawnPoint = GameObject.Find("PlayerSpawnPoint")?.transform;
            
            if (spawnPoint != null && playerPrefab != null)
            {
                if (currentPlayer != null)
                {
                    Destroy(currentPlayer);
                }

                currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            }
            else if (playerPrefab != null)
            {
                // If no spawn point found, spawn at origin
                if (currentPlayer != null)
                {
                    Destroy(currentPlayer);
                }

                currentPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                Debug.LogWarning($"No PlayerSpawnPoint found in {scene.name}, spawning player at origin.");
            }
            else
            {
                Debug.LogWarning("PlayerPrefab not assigned in GameManager!");
            }
        }
    }
}
