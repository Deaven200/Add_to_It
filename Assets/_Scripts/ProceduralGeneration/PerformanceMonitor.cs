using UnityEngine;
using System.Collections.Generic;
using System;

public class PerformanceMonitor : MonoBehaviour
{
    [Header("Display Settings")]
    [SerializeField] private bool showPerformanceUI = true;
    [SerializeField] private bool showInConsole = false;
    [SerializeField] private float updateInterval = 0.5f;
    
    [Header("Monitoring")]
    [SerializeField] private bool monitorFPS = true;
    [SerializeField] private bool monitorChunks = true;
    [SerializeField] private bool monitorMemory = true;
    
    // Performance data
    private float fps;
    private float avgFPS;
    private int frameCount;
    private float timeElapsed;
    
    // Chunk data
    private ProceduralLevelManager levelManager;
    private int activeChunks;
    private int pooledChunks;
    private int totalChunksGenerated;
    
    // Memory data
    private long totalMemory;
    private long usedMemory;
    
    // UI
    private GUIStyle style;
    private Rect windowRect = new Rect(10, 10, 300, 200);
    
    void Start()
    {
        // Find the procedural level manager
        levelManager = FindObjectOfType<ProceduralLevelManager>();
        
        // Initialize UI style
        style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;
    }
    
    void Update()
    {
        // Update FPS
        if (monitorFPS)
        {
            frameCount++;
            timeElapsed += Time.deltaTime;
            
            if (timeElapsed >= updateInterval)
            {
                fps = frameCount / timeElapsed;
                avgFPS = Mathf.Lerp(avgFPS, fps, 0.1f);
                
                frameCount = 0;
                timeElapsed = 0;
            }
        }
        
        // Update chunk data
        if (monitorChunks && levelManager != null)
        {
            activeChunks = levelManager.ActiveChunkCount;
            pooledChunks = levelManager.PooledChunkCount;
            // Note: totalChunksGenerated would need to be exposed from ProceduralLevelManager
        }
        
        // Update memory data
        if (monitorMemory)
        {
            totalMemory = System.GC.GetTotalMemory(false);
            usedMemory = System.GC.GetTotalMemory(true);
        }
        
        // Log to console if enabled
        if (showInConsole && Time.frameCount % 60 == 0)
        {
            LogPerformanceData();
        }
    }
    
    void LogPerformanceData()
    {
        string log = "Performance Data:\n";
        
        if (monitorFPS)
        {
            log += $"FPS: {fps:F1} (Avg: {avgFPS:F1})\n";
        }
        
        if (monitorChunks)
        {
            log += $"Active Chunks: {activeChunks}, Pooled: {pooledChunks}\n";
        }
        
        if (monitorMemory)
        {
            log += $"Memory: {FormatBytes(usedMemory)} / {FormatBytes(totalMemory)}\n";
        }
        
        Debug.Log(log);
    }
    
    string FormatBytes(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB" };
        int counter = 0;
        decimal number = bytes;
        
        while (Math.Round(number / 1024) >= 1)
        {
            number /= 1024;
            counter++;
        }
        
        return $"{number:n1} {suffixes[counter]}";
    }
    
    void OnGUI()
    {
        if (!showPerformanceUI) return;
        
        windowRect = GUI.Window(0, windowRect, DrawPerformanceWindow, "Performance Monitor");
    }
    
    void DrawPerformanceWindow(int windowID)
    {
        GUILayout.BeginVertical();
        
        if (monitorFPS)
        {
            GUILayout.Label($"FPS: {fps:F1}", style);
            GUILayout.Label($"Avg FPS: {avgFPS:F1}", style);
        }
        
        if (monitorChunks)
        {
            GUILayout.Space(10);
            GUILayout.Label($"Active Chunks: {activeChunks}", style);
            GUILayout.Label($"Pooled Chunks: {pooledChunks}", style);
        }
        
        if (monitorMemory)
        {
            GUILayout.Space(10);
            GUILayout.Label($"Memory: {FormatBytes(usedMemory)}", style);
            GUILayout.Label($"Total: {FormatBytes(totalMemory)}", style);
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Log to Console"))
        {
            LogPerformanceData();
        }
        
        if (GUILayout.Button("Close"))
        {
            showPerformanceUI = false;
        }
        
        GUILayout.EndVertical();
        
        // Make window draggable
        GUI.DragWindow();
    }
    
    // Public methods for external access
    public float GetFPS() => fps;
    public float GetAverageFPS() => avgFPS;
    public int GetActiveChunks() => activeChunks;
    public int GetPooledChunks() => pooledChunks;
    public long GetUsedMemory() => usedMemory;
    public long GetTotalMemory() => totalMemory;
    
    // Public method to toggle UI
    public void ToggleUI()
    {
        showPerformanceUI = !showPerformanceUI;
    }
}
