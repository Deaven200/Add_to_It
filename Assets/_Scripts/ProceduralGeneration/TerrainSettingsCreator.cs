using UnityEngine;
using UnityEditor;

public class TerrainSettingsCreator : MonoBehaviour
{
    [Header("Creation Settings")]
    [SerializeField] private bool createOnStart = false;
    [SerializeField] private string assetName = "DefaultTerrainSettings";
    
    void Start()
    {
        if (createOnStart)
        {
            CreateTerrainSettings();
        }
    }
    
    [ContextMenu("Create TerrainSettings Asset")]
    public void CreateTerrainSettings()
    {
        #if UNITY_EDITOR
        // Create the TerrainSettings asset
        TerrainSettings terrainSettings = ScriptableObject.CreateInstance<TerrainSettings>();
        
        // Set a default path in the Assets folder
        string path = $"Assets/{assetName}.asset";
        
        // Create the asset file
        AssetDatabase.CreateAsset(terrainSettings, path);
        AssetDatabase.SaveAssets();
        
        // Select the created asset in the Project window
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = terrainSettings;
        
        Debug.Log($"TerrainSettings asset created at: {path}");
        Debug.Log("You can now assign this asset to your ProceduralLevelManager's Terrain Settings field.");
        #else
        Debug.LogWarning("TerrainSettings can only be created in the Unity Editor.");
        #endif
    }
    
    [ContextMenu("Create TerrainSettings in ScriptableObjects Folder")]
    public void CreateTerrainSettingsInScriptableObjects()
    {
        #if UNITY_EDITOR
        // Create the TerrainSettings asset
        TerrainSettings terrainSettings = ScriptableObject.CreateInstance<TerrainSettings>();
        
        // Set path in ScriptableObjects folder
        string path = "Assets/ScriptableObjects/TerrainSettings.asset";
        
        // Ensure the ScriptableObjects folder exists
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
        {
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
        }
        
        // Create the asset file
        AssetDatabase.CreateAsset(terrainSettings, path);
        AssetDatabase.SaveAssets();
        
        // Select the created asset in the Project window
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = terrainSettings;
        
        Debug.Log($"TerrainSettings asset created at: {path}");
        Debug.Log("You can now assign this asset to your ProceduralLevelManager's Terrain Settings field.");
        #else
        Debug.LogWarning("TerrainSettings can only be created in the Unity Editor.");
        #endif
    }
}


