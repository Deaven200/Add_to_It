using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [Header("Loading UI")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Text loadingText;
    [SerializeField] private Slider progressBar;
    
    [Header("Settings")]
    [SerializeField] private bool showLoadingScreen = false;
    [SerializeField] private string[] loadingMessages = {
        "Generating terrain...",
        "Creating chunks...",
        "Setting up world...",
        "Almost ready..."
    };
    
    private ProceduralLevelManager levelManager;
    private bool isLoading = true;
    private float loadingStartTime;
    
    void Start()
    {
        if (showLoadingScreen)
        {
            ShowLoadingScreen();
        }
        
        // Find the level manager
        levelManager = FindObjectOfType<ProceduralLevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("LoadingManager: No ProceduralLevelManager found!");
            HideLoadingScreen();
            return;
        }
        
        loadingStartTime = Time.time;
    }
    
    void Update()
    {
        if (!isLoading || levelManager == null) return;
        
        // Update loading progress
        UpdateLoadingProgress();
        
        // Check if initialization is complete
        if (IsInitializationComplete())
        {
            CompleteLoading();
        }
    }
    
    void ShowLoadingScreen()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }
        
        isLoading = true;
        
        if (loadingText != null)
        {
            loadingText.text = "Initializing...";
        }
        
        if (progressBar != null)
        {
            progressBar.value = 0f;
        }
    }
    
    void HideLoadingScreen()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
        
        isLoading = false;
    }
    
    void UpdateLoadingProgress()
    {
        if (progressBar != null)
        {
            // Calculate progress based on time and chunk generation
            float timeProgress = Mathf.Clamp01((Time.time - loadingStartTime) / 3f); // 3 second max
            float chunkProgress = 0f;
            
            if (levelManager != null)
            {
                // Progress based on active chunks (assuming we need at least 9 chunks for initial area)
                chunkProgress = Mathf.Clamp01(levelManager.ActiveChunkCount / 9f);
            }
            
            float totalProgress = Mathf.Max(timeProgress, chunkProgress);
            progressBar.value = totalProgress;
        }
        
        if (loadingText != null)
        {
            // Cycle through loading messages
            int messageIndex = Mathf.FloorToInt((Time.time - loadingStartTime) * 0.5f) % loadingMessages.Length;
            loadingText.text = loadingMessages[messageIndex];
        }
    }
    
    bool IsInitializationComplete()
    {
        if (levelManager == null) return false;
        
        // Check if we have enough chunks around the player
        int requiredChunks = 9; // 3x3 area around player
        bool hasEnoughChunks = levelManager.ActiveChunkCount >= requiredChunks;
        
        // Check if enough time has passed
        bool enoughTimePassed = (Time.time - loadingStartTime) >= 1f;
        
        return hasEnoughChunks && enoughTimePassed;
    }
    
    void CompleteLoading()
    {
        Debug.Log("Loading complete! World is ready.");
        
        if (loadingText != null)
        {
            loadingText.text = "Ready!";
        }
        
        if (progressBar != null)
        {
            progressBar.value = 1f;
        }
        
        // Hide loading screen after a short delay
        Invoke(nameof(HideLoadingScreen), 0.5f);
    }
    
    // Public method to manually complete loading (for testing)
    [ContextMenu("Complete Loading")]
    public void ForceCompleteLoading()
    {
        CompleteLoading();
    }
    
    // Public method to restart loading (for testing)
    [ContextMenu("Restart Loading")]
    public void RestartLoading()
    {
        loadingStartTime = Time.time;
        ShowLoadingScreen();
    }
}
