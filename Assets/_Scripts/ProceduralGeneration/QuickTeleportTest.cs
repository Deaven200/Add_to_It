using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Quick test script for the teleportation system.
/// Add this to any scene to test teleporting to Main_level.
/// </summary>
public class QuickTeleportTest : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private KeyCode teleportKey = KeyCode.T;
    [SerializeField] private string targetScene = "Main_level";
    
    void Update()
    {
        if (Input.GetKeyDown(teleportKey))
        {
            Debug.Log($"Quick Teleport Test: Teleporting to {targetScene}");
            SceneManager.LoadScene(targetScene);
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 250, 100));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("Quick Teleport Test", GUI.skin.box);
        GUILayout.Label($"Press {teleportKey} to teleport to {targetScene}");
        GUILayout.Label($"Current Scene: {SceneManager.GetActiveScene().name}");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
