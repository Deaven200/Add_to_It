using UnityEngine;
using UnityEngine.UI;

public class ChunkPersistenceUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button clearChunksButton;
    [SerializeField] private Text statusText;
    
    private ChunkGenerator chunkGenerator;
    private ChunkPersistenceManager persistenceManager;
    
    void Start()
    {
        // Find the chunk generator
        chunkGenerator = FindObjectOfType<ChunkGenerator>();
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
        if (chunkGenerator != null)
        {
            chunkGenerator.StartNewGame();
            Debug.Log("New game started via UI");
        }
        else
        {
            Debug.LogWarning("ChunkGenerator not found!");
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
            
            if (chunkGenerator != null)
            {
                // We can't directly access the active chunks count, so we'll estimate
                // based on render distance
                activeChunks = 9; // 3x3 render distance = 9 chunks
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
