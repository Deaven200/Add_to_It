using UnityEngine;

[System.Serializable]
public class EnemyDropSettings
{
    [Header("Enemy Drop Settings")]
    [Range(0f, 100f)]
    public float chestDropRate = 10f; // Percentage chance to drop a chest
    [Range(0f, 10f)]
    public float chestDropRadius = 2f; // How far from enemy the chest can spawn
    public GameObject chestPrefab; // The chest prefab to spawn
}

public class EnemyDropManager : MonoBehaviour
{
    [Header("Global Drop Settings")]
    [SerializeField] private EnemyDropSettings dropSettings = new EnemyDropSettings();
    
    [Header("Debug")]
    [SerializeField] private bool showDropChance = true;
    
    private static EnemyDropManager _instance;
    public static EnemyDropManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EnemyDropManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("EnemyDropManager");
                    _instance = go.AddComponent<EnemyDropManager>();
                }
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Called when an enemy dies. Determines if a chest should be dropped.
    /// </summary>
    /// <param name="enemyPosition">The position where the enemy died</param>
    /// <param name="enemyName">Name of the enemy for debug purposes</param>
    public void OnEnemyDeath(Vector3 enemyPosition, string enemyName = "Enemy")
    {
        // Check if we should drop a chest
        if (Random.Range(0f, 100f) <= dropSettings.chestDropRate)
        {
            SpawnChest(enemyPosition, enemyName);
        }
    }
    
    /// <summary>
    /// Spawns a chest at the given position with some random offset
    /// </summary>
    private void SpawnChest(Vector3 position, string enemyName)
    {
        if (dropSettings.chestPrefab == null)
        {
            Debug.LogError("EnemyDropManager: Chest prefab is not assigned!");
            return;
        }
        
        // Calculate random position within drop radius
        Vector2 randomCircle = Random.insideUnitCircle * dropSettings.chestDropRadius;
        Vector3 chestPosition = position + new Vector3(randomCircle.x, 0f, randomCircle.y);
        
        // Spawn the chest
        GameObject chest = Instantiate(dropSettings.chestPrefab, chestPosition, Quaternion.identity);
        
        Debug.Log($"Chest dropped from {enemyName} at position {chestPosition}");
    }
    
    /// <summary>
    /// Set the global chest drop rate for all enemies
    /// </summary>
    public void SetGlobalDropRate(float dropRate)
    {
        dropSettings.chestDropRate = Mathf.Clamp(dropRate, 0f, 100f);
        Debug.Log($"Global chest drop rate set to {dropSettings.chestDropRate}%");
    }
    
    /// <summary>
    /// Get the current global drop rate
    /// </summary>
    public float GetGlobalDropRate()
    {
        return dropSettings.chestDropRate;
    }
    
    /// <summary>
    /// Set the chest prefab to spawn
    /// </summary>
    public void SetChestPrefab(GameObject chestPrefab)
    {
        dropSettings.chestPrefab = chestPrefab;
    }
    
    // Debug method to test chest spawning
    [ContextMenu("Test Chest Drop")]
    public void TestChestDrop()
    {
        Vector3 testPosition = Vector3.zero;
        if (Camera.main != null)
        {
            testPosition = Camera.main.transform.position + Camera.main.transform.forward * 5f;
        }
        SpawnChest(testPosition, "Test");
    }
    
    // Debug method to show current drop rate
    private void OnGUI()
    {
        if (showDropChance)
        {
            GUI.Label(new Rect(10, 10, 200, 20), $"Chest Drop Rate: {dropSettings.chestDropRate}%");
        }
    }
}
