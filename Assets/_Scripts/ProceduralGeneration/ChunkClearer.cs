using UnityEngine;

public class ChunkClearer : MonoBehaviour
{
    [Header("Clear Settings")]
    [SerializeField] private bool clearOnStart = true;
    [SerializeField] private KeyCode clearKey = KeyCode.C;
    
    void Start()
    {
        if (clearOnStart)
        {
            ClearAllChunks();
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(clearKey))
        {
            ClearAllChunks();
        }
    }
    
    public void ClearAllChunks()
    {
        // Clear PlayerPrefs chunk data
        PlayerPrefs.DeleteKey("ChunkData");
        PlayerPrefs.Save();
        
        // Find and clear any active chunks
        ProceduralLevelManager levelManager = FindObjectOfType<ProceduralLevelManager>();
        if (levelManager != null)
        {
            levelManager.StartNewGame();
        }
        
        Debug.Log("All chunks cleared! Restart the game to see the new chunk size.");
    }
}
