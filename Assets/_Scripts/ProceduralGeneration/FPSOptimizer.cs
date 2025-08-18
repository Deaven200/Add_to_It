using UnityEngine;
using System.Collections;

/// <summary>
/// FPS-based optimization system for procedural generation
/// Monitors frame rate and controls chunk generation to maintain performance
/// </summary>
public class FPSOptimizer : MonoBehaviour
{
    [Header("FPS Monitoring")]
    [SerializeField] private bool enableFPSOptimization = true;
    [SerializeField] private float fpsUpdateInterval = 0.5f; // How often to update FPS calculation
    [SerializeField] private int fpsSampleSize = 30; // Number of frames to average for FPS calculation
    
    [Header("Performance Thresholds")]
    [SerializeField] private float minFPSThreshold = 30f; // Stop generation when FPS drops below this
    [SerializeField] private float resumeFPSThreshold = 45f; // Resume generation when FPS goes above this
    [SerializeField] private float criticalFPSThreshold = 20f; // Emergency measures when FPS drops below this
    
    [Header("Optimization Settings")]
    [SerializeField] private bool enableEmergencyOptimization = true;
    [SerializeField] private float emergencyOptimizationDuration = 5f; // How long to stay in emergency mode
    [SerializeField] private bool reduceRenderDistanceOnLowFPS = true;
    [SerializeField] private int emergencyRenderDistance = 4; // Reduced render distance for emergency mode
    
    [Header("Debug")]
    [SerializeField] private bool showFPSDebug = true;
    [SerializeField] private bool logOptimizationEvents = true;
    
    // FPS tracking
    private float[] fpsBuffer;
    private int fpsBufferIndex = 0;
    private float currentFPS = 60f;
    private float lastFPSUpdate = 0f;
    
    // Performance state
    private bool isGenerationPaused = false;
    private bool isEmergencyMode = false;
    private float emergencyModeStartTime = 0f;
    private int originalRenderDistance = 8;
    
    // References
    private ProceduralLevelManager levelManager;
    private DynamicEnemySpawner enemySpawner;
    
    // Events
    public System.Action<bool> OnGenerationPausedChanged;
    public System.Action<bool> OnEmergencyModeChanged;
    public System.Action<float> OnFPSChanged;
    
    // Public properties
    public float CurrentFPS => currentFPS;
    public bool IsGenerationPaused => isGenerationPaused;
    public bool IsEmergencyMode => isEmergencyMode;
    public bool IsOptimizationEnabled => enableFPSOptimization;
    
    void Start()
    {
        InitializeFPSBuffer();
        FindReferences();
        StartCoroutine(FPSCalculationCoroutine());
    }
    
    void InitializeFPSBuffer()
    {
        fpsBuffer = new float[fpsSampleSize];
        for (int i = 0; i < fpsSampleSize; i++)
        {
            fpsBuffer[i] = 60f; // Initialize with 60 FPS
        }
    }
    
    void FindReferences()
    {
        // Find ProceduralLevelManager
        levelManager = FindObjectOfType<ProceduralLevelManager>();
        if (levelManager == null)
        {
            Debug.LogWarning("FPSOptimizer: No ProceduralLevelManager found!");
        }
        
        // Find DynamicEnemySpawner
        enemySpawner = FindObjectOfType<DynamicEnemySpawner>();
        if (enemySpawner == null)
        {
            Debug.LogWarning("FPSOptimizer: No DynamicEnemySpawner found!");
        }
        
        // Store original render distance
        if (levelManager != null)
        {
            originalRenderDistance = levelManager.RenderDistance;
        }
    }
    
    void Update()
    {
        if (!enableFPSOptimization) return;
        
        // Update FPS calculation
        UpdateFPSCalculation();
        
        // Check performance thresholds
        CheckPerformanceThresholds();
        
        // Handle emergency mode
        HandleEmergencyMode();
    }
    
    void UpdateFPSCalculation()
    {
        // Add current frame time to buffer
        fpsBuffer[fpsBufferIndex] = 1f / Time.deltaTime;
        fpsBufferIndex = (fpsBufferIndex + 1) % fpsSampleSize;
        
        // Calculate average FPS
        float totalFPS = 0f;
        for (int i = 0; i < fpsSampleSize; i++)
        {
            totalFPS += fpsBuffer[i];
        }
        currentFPS = totalFPS / fpsSampleSize;
        
        // Notify FPS change
        OnFPSChanged?.Invoke(currentFPS);
    }
    
    void CheckPerformanceThresholds()
    {
        bool shouldPauseGeneration = false;
        bool shouldEnterEmergencyMode = false;
        
        // Check if we should pause generation
        if (currentFPS < minFPSThreshold && !isGenerationPaused)
        {
            shouldPauseGeneration = true;
        }
        else if (currentFPS > resumeFPSThreshold && isGenerationPaused && !isEmergencyMode)
        {
            shouldPauseGeneration = false;
        }
        
        // Check if we should enter emergency mode
        if (currentFPS < criticalFPSThreshold && enableEmergencyOptimization && !isEmergencyMode)
        {
            shouldEnterEmergencyMode = true;
        }
        
        // Apply changes
        if (shouldPauseGeneration)
        {
            PauseGeneration();
        }
        else if (!shouldPauseGeneration && isGenerationPaused && !isEmergencyMode)
        {
            ResumeGeneration();
        }
        
        if (shouldEnterEmergencyMode)
        {
            EnterEmergencyMode();
        }
    }
    
    void HandleEmergencyMode()
    {
        if (!isEmergencyMode) return;
        
        // Check if emergency mode should end
        if (Time.time - emergencyModeStartTime > emergencyOptimizationDuration)
        {
            ExitEmergencyMode();
        }
    }
    
    void PauseGeneration()
    {
        if (isGenerationPaused) return;
        
        isGenerationPaused = true;
        
        // Pause chunk generation
        if (levelManager != null)
        {
            // We'll need to add a method to ProceduralLevelManager to pause generation
            // For now, we'll use a workaround by setting a flag
            SetChunkGenerationPaused(true);
        }
        
        // Pause enemy spawning
        if (enemySpawner != null)
        {
            enemySpawner.pauseSpawning = true;
        }
        
        if (logOptimizationEvents)
        {
            Debug.Log($"FPS Optimization: Generation paused (FPS: {currentFPS:F1})");
        }
        
        OnGenerationPausedChanged?.Invoke(true);
    }
    
    void ResumeGeneration()
    {
        if (!isGenerationPaused) return;
        
        isGenerationPaused = false;
        
        // Resume chunk generation
        if (levelManager != null)
        {
            SetChunkGenerationPaused(false);
        }
        
        // Resume enemy spawning
        if (enemySpawner != null)
        {
            enemySpawner.pauseSpawning = false;
        }
        
        if (logOptimizationEvents)
        {
            Debug.Log($"FPS Optimization: Generation resumed (FPS: {currentFPS:F1})");
        }
        
        OnGenerationPausedChanged?.Invoke(false);
    }
    
    void EnterEmergencyMode()
    {
        if (isEmergencyMode) return;
        
        isEmergencyMode = true;
        emergencyModeStartTime = Time.time;
        
        // Force pause generation
        if (!isGenerationPaused)
        {
            PauseGeneration();
        }
        
        // Reduce render distance
        if (reduceRenderDistanceOnLowFPS && levelManager != null)
        {
            // We'll need to add a method to ProceduralLevelManager to set render distance
            SetRenderDistance(emergencyRenderDistance);
        }
        
        if (logOptimizationEvents)
        {
            Debug.Log($"FPS Optimization: Emergency mode activated (FPS: {currentFPS:F1})");
        }
        
        OnEmergencyModeChanged?.Invoke(true);
    }
    
    void ExitEmergencyMode()
    {
        if (!isEmergencyMode) return;
        
        isEmergencyMode = false;
        
        // Restore render distance
        if (reduceRenderDistanceOnLowFPS && levelManager != null)
        {
            SetRenderDistance(originalRenderDistance);
        }
        
        // Resume generation if FPS is good enough
        if (currentFPS > resumeFPSThreshold)
        {
            ResumeGeneration();
        }
        
        if (logOptimizationEvents)
        {
            Debug.Log($"FPS Optimization: Emergency mode deactivated (FPS: {currentFPS:F1})");
        }
        
        OnEmergencyModeChanged?.Invoke(false);
    }
    
    // Methods to control ProceduralLevelManager
    void SetChunkGenerationPaused(bool paused)
    {
        if (levelManager != null)
        {
            if (paused)
            {
                levelManager.PauseGeneration();
            }
            else
            {
                levelManager.ResumeGeneration();
            }
        }
    }
    
    void SetRenderDistance(int distance)
    {
        if (levelManager != null)
        {
            levelManager.SetRenderDistance(distance);
        }
    }
    
    System.Collections.IEnumerator FPSCalculationCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(fpsUpdateInterval);
            
            // Additional FPS logging if enabled
            if (showFPSDebug && Time.frameCount % 60 == 0)
            {
                Debug.Log($"FPS: {currentFPS:F1}, Generation Paused: {isGenerationPaused}, Emergency Mode: {isEmergencyMode}");
            }
        }
    }
    
    void OnGUI()
    {
        if (!showFPSDebug) return;
        
        GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 190, 100));
        GUILayout.Label($"FPS: {currentFPS:F1}");
        GUILayout.Label($"Generation: {(isGenerationPaused ? "PAUSED" : "ACTIVE")}");
        GUILayout.Label($"Emergency: {(isEmergencyMode ? "ACTIVE" : "Inactive")}");
        GUILayout.Label($"Thresholds: {minFPSThreshold}/{resumeFPSThreshold}");
        GUILayout.EndArea();
    }
    
    // Public methods for external control
    public void SetOptimizationEnabled(bool enabled)
    {
        enableFPSOptimization = enabled;
        
        if (!enabled)
        {
            // Resume everything when optimization is disabled
            if (isGenerationPaused)
            {
                ResumeGeneration();
            }
            if (isEmergencyMode)
            {
                ExitEmergencyMode();
            }
        }
    }
    
    public void SetFPSThresholds(float minFPS, float resumeFPS, float criticalFPS = 20f)
    {
        minFPSThreshold = minFPS;
        resumeFPSThreshold = resumeFPS;
        criticalFPSThreshold = criticalFPS;
    }
    
    public void ForceEmergencyMode()
    {
        EnterEmergencyMode();
    }
    
    public void ForceResumeGeneration()
    {
        ExitEmergencyMode();
        ResumeGeneration();
    }
}
