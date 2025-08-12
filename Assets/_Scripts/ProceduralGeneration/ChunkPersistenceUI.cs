using UnityEngine;
using UnityEngine.UI;

public class ChunkPersistenceUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button clearChunksButton;
    [SerializeField] private Text statusText;
    
    private ProceduralLevelManager levelManager;
    private ChunkPersistenceManager persistenceManager;
    
    void Start()
    {
        // Find the level manager
        levelManager = FindObjectOfType<ProceduralLevelManager>();
        persistenceManager = FindObjectOfType<ChunkPersistenceManager>();
        
        // Set up button listeners
        if (newGameButton != null)
        {
            newGameButton.onClick.AddListener(StartNewGame);
        }
        
        if (clearChunksButton != null)
        {
            clearChunksButton.onClick.AddListener(ClearAllChunks);
        }
        
        UpdateStatusText();
    }
    
    void Update()
    {
        // Update status text every frame to show current chunk count
        UpdateStatusText();
    }
    
    public void StartNewGame()
    {
        if (levelManager != null)
        {
            levelManager.StartNewGame();
            Debug.Log("New game started via UI");
        }
        else
        {
            Debug.LogWarning("ProceduralLevelManager not found!");
        }
    }
    
    public void ClearAllChunks()
    {
        if (persistenceManager != null)
        {
            persistenceManager.ClearAllChunks();
            Debug.Log("All chunks cleared via UI");
        }
        else
        {
            Debug.LogWarning("ChunkPersistenceManager not found!");
        }
    }
    
    void UpdateStatusText()
    {
        if (statusText != null)
        {
            int savedChunks = 0;
            int activeChunks = 0;
            
            if (persistenceManager != null)
            {
                savedChunks = persistenceManager.GetSavedChunkCount();
            }
            
            if (levelManager != null)
            {
                activeChunks = levelManager.ActiveChunkCount;
            }
            
            statusText.text = $"Saved Chunks: {savedChunks}\nActive Chunks: ~{activeChunks}";
        }
    }
    
    // Public method to get chunk info for debugging
    public void PrintChunkInfo()
    {
        if (persistenceManager != null)
        {
            Debug.Log($"Saved chunks: {persistenceManager.GetSavedChunkCount()}");
            var positions = persistenceManager.GetSavedChunkPositions();
            foreach (var pos in positions)
            {
                Debug.Log($"Saved chunk at: {pos}");
            }
        }
    }
}
