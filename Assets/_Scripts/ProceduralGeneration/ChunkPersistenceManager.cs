using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChunkPersistenceManager : MonoBehaviour
{
    [Header("Persistence Settings")]
    [SerializeField] private bool enablePersistence = true;
    [SerializeField] private string saveKey = "ChunkData";
    
    private MonoBehaviour chunkManager; // Can be ProceduralLevelManager
    private Dictionary<Vector2Int, ChunkData> savedChunks = new Dictionary<Vector2Int, ChunkData>();
    
    [System.Serializable]
    public class ChunkData
    {
        public int x;
        public int z;
        public float[] worldPosition = new float[3];
        public bool isGenerated;
        public long timestamp;
        
        public ChunkData(Vector2Int position, Vector3 worldPos, bool generated)
        {
            x = position.x;
            z = position.y; // Vector2Int.y stores the Z coordinate
            worldPosition[0] = worldPos.x;
            worldPosition[1] = worldPos.y;
            worldPosition[2] = worldPos.z;
            isGenerated = generated;
            timestamp = System.DateTime.Now.Ticks;
        }
        
        public Vector2Int GetPosition()
        {
            return new Vector2Int(x, z);
        }
        
        public Vector3 GetWorldPosition()
        {
            return new Vector3(worldPosition[0], worldPosition[1], worldPosition[2]);
        }
    }
    
    void Start()
    {
        if (enablePersistence)
        {
            LoadChunkData();
        }
    }
    

    
    public void SetChunkManager(ProceduralLevelManager manager)
    {
        chunkManager = manager;
    }
    
    public void SaveChunk(Vector2Int position, Vector3 worldPosition, bool isGenerated)
    {
        if (!enablePersistence) return;
        
        ChunkData chunkData = new ChunkData(position, worldPosition, isGenerated);
        savedChunks[position] = chunkData;
        
        // Save to PlayerPrefs immediately
        SaveToPlayerPrefs();
    }
    
    public void RemoveChunk(Vector2Int position)
    {
        if (!enablePersistence) return;
        
        if (savedChunks.ContainsKey(position))
        {
            savedChunks.Remove(position);
            SaveToPlayerPrefs();
        }
    }
    
    public bool HasChunk(Vector2Int position)
    {
        return savedChunks.ContainsKey(position) && savedChunks[position].isGenerated;
    }
    
    public Vector3 GetChunkWorldPosition(Vector2Int position)
    {
        if (savedChunks.ContainsKey(position))
        {
            return savedChunks[position].GetWorldPosition();
        }
        return Vector3.zero;
    }
    
    public void ClearAllChunks()
    {
        savedChunks.Clear();
        PlayerPrefs.DeleteKey(saveKey);
        PlayerPrefs.Save();
        Debug.Log("Cleared all saved chunk data");
    }
    
    public void StartNewGame()
    {
        ClearAllChunks();
        Debug.Log("Started new game - chunk data cleared");
    }
    
    private void SaveToPlayerPrefs()
    {
        try
        {
            // Convert dictionary to JSON
            List<ChunkData> chunkList = savedChunks.Values.ToList();
            string json = JsonUtility.ToJson(new ChunkDataWrapper { chunks = chunkList });
            
            // Save to PlayerPrefs
            PlayerPrefs.SetString(saveKey, json);
            PlayerPrefs.Save();
            
            // Debug logging removed to reduce console spam
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving chunk data: {e.Message}");
        }
    }
    
    private void LoadChunkData()
    {
        try
        {
            if (PlayerPrefs.HasKey(saveKey))
            {
                string json = PlayerPrefs.GetString(saveKey);
                ChunkDataWrapper wrapper = JsonUtility.FromJson<ChunkDataWrapper>(json);
                
                savedChunks.Clear();
                foreach (ChunkData chunkData in wrapper.chunks)
                {
                    savedChunks[chunkData.GetPosition()] = chunkData;
                }
                
                Debug.Log($"Loaded {savedChunks.Count} chunks from persistent storage");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading chunk data: {e.Message}");
            savedChunks.Clear();
        }
    }
    
    [System.Serializable]
    private class ChunkDataWrapper
    {
        public List<ChunkData> chunks = new List<ChunkData>();
    }
    
    // Public method to get chunk count for debugging
    public int GetSavedChunkCount()
    {
        return savedChunks.Count;
    }
    
    // Public method to get all saved chunk positions
    public List<Vector2Int> GetSavedChunkPositions()
    {
        return savedChunks.Keys.ToList();
    }
}
