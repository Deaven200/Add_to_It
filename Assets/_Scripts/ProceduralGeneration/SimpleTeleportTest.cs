using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple test script for the teleportation system.
/// Add this to any scene and press T to test teleporting to Main_level.
/// </summary>
public class SimpleTeleportTest : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private KeyCode teleportKey = KeyCode.T;
    [SerializeField] private string targetScene = "Main_level";
    
    void Update()
    {
        if (Input.GetKeyDown(teleportKey))
        {
            Debug.Log($"Simple Teleport Test: Teleporting to {targetScene}");
            SceneManager.LoadScene(targetScene);
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 120));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("Simple Teleport Test", GUI.skin.box);
        GUILayout.Label($"Press {teleportKey} to teleport to {targetScene}");
        GUILayout.Label($"Current Scene: {SceneManager.GetActiveScene().name}");
        GUILayout.Label("After teleporting, wait 3-5 seconds");
        GUILayout.Label("for terrain generation to complete.");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
