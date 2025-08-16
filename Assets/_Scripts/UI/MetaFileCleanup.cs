using UnityEngine;
using System.IO;

/// <summary>
/// Utility script to help clean up orphaned .meta files
/// Run this in the editor to find and optionally delete orphaned .meta files
/// </summary>
public class MetaFileCleanup : MonoBehaviour
{
    [Header("Cleanup Settings")]
    [SerializeField] private bool runOnStart = false;
    [SerializeField] private bool deleteOrphanedMetaFiles = false;
    
    void Start()
    {
        if (runOnStart)
        {
            CheckForOrphanedMetaFiles();
        }
    }
    
    [ContextMenu("Check for Orphaned Meta Files")]
    public void CheckForOrphanedMetaFiles()
    {
        Debug.Log("=== META FILE CLEANUP ===");
        
        // Check for specific orphaned .meta files that were mentioned in the error
        string[] orphanedMetaFiles = {
            "Assets/_Scripts/UI/PauseDebugger.cs.meta",
            "Assets/_Scripts/UI/PauseManager.cs.meta"
        };
        
        foreach (string metaFilePath in orphanedMetaFiles)
        {
            if (File.Exists(metaFilePath))
            {
                Debug.LogWarning($"Found orphaned .meta file: {metaFilePath}");
                
                if (deleteOrphanedMetaFiles)
                {
                    try
                    {
                        File.Delete(metaFilePath);
                        Debug.Log($"Deleted orphaned .meta file: {metaFilePath}");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Failed to delete {metaFilePath}: {e.Message}");
                    }
                }
            }
            else
            {
                Debug.Log($"No orphaned .meta file found: {metaFilePath}");
            }
        }
        
        Debug.Log("=== META FILE CLEANUP COMPLETE ===");
        
        if (!deleteOrphanedMetaFiles)
        {
            Debug.Log("To delete orphaned .meta files, set 'deleteOrphanedMetaFiles' to true and run again.");
        }
    }
    
    [ContextMenu("Refresh Unity Project")]
    public void RefreshUnityProject()
    {
        Debug.Log("Please manually refresh Unity project: Assets > Refresh");
        Debug.Log("This will regenerate .meta files and resolve orphaned .meta file issues.");
    }
}
