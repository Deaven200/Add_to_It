using UnityEngine;

/// <summary>
/// Simple performance monitor to help identify lag spikes.
/// Attach this to any GameObject to monitor performance.
/// </summary>
public class PauseMenuPerformanceMonitor : MonoBehaviour
{
    [Header("Performance Monitoring")]
    [SerializeField] private bool enableMonitoring = true;
    [SerializeField] private float frameTimeThreshold = 16.67f; // 60 FPS threshold
    [SerializeField] private int logInterval = 60; // Log every 60 frames
    
    private float[] frameTimes;
    private int frameCount = 0;
    private float lastLogTime = 0f;
    
    void Start()
    {
        if (enableMonitoring)
        {
            frameTimes = new float[300]; // Store last 300 frames
            Debug.Log("PauseMenuPerformanceMonitor: Started monitoring performance");
        }
    }
    
    void Update()
    {
        if (!enableMonitoring) return;
        
        // Record frame time
        float frameTime = Time.unscaledDeltaTime * 1000f; // Convert to milliseconds
        frameTimes[frameCount % frameTimes.Length] = frameTime;
        frameCount++;
        
        // Check for lag spikes
        if (frameTime > frameTimeThreshold)
        {
            Debug.LogWarning($"PauseMenuPerformanceMonitor: Lag spike detected! Frame time: {frameTime:F2}ms (threshold: {frameTimeThreshold:F2}ms)");
        }
        
        // Log performance stats periodically
        if (Time.time - lastLogTime > 5f) // Every 5 seconds
        {
            LogPerformanceStats();
            lastLogTime = Time.time;
        }
    }
    
    void LogPerformanceStats()
    {
        if (frameCount < frameTimes.Length) return;
        
        float totalTime = 0f;
        float maxTime = 0f;
        float minTime = float.MaxValue;
        
        for (int i = 0; i < frameTimes.Length; i++)
        {
            totalTime += frameTimes[i];
            if (frameTimes[i] > maxTime) maxTime = frameTimes[i];
            if (frameTimes[i] < minTime) minTime = frameTimes[i];
        }
        
        float avgTime = totalTime / frameTimes.Length;
        float avgFPS = 1000f / avgTime;
        
        Debug.Log($"PauseMenuPerformanceMonitor: Avg FPS: {avgFPS:F1}, Avg Frame Time: {avgTime:F2}ms, Min: {minTime:F2}ms, Max: {maxTime:F2}ms");
    }
    
    [ContextMenu("Log Current Performance")]
    public void LogCurrentPerformance()
    {
        LogPerformanceStats();
    }
    
    [ContextMenu("Toggle Monitoring")]
    public void ToggleMonitoring()
    {
        enableMonitoring = !enableMonitoring;
        Debug.Log($"PauseMenuPerformanceMonitor: Monitoring {(enableMonitoring ? "enabled" : "disabled")}");
    }
}
