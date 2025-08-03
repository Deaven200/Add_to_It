using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject playerPrefab; // Assign in Inspector
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
    }
}
