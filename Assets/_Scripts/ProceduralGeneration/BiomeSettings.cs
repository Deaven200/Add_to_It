using UnityEngine;

[System.Serializable]
public class BiomeSettings
{
    [SerializeField] private string biomeName;
    [SerializeField] private float minHeight;
    [SerializeField] private float maxHeight;
    [SerializeField] private Color groundColor;
    [SerializeField] private float roughness;
    
    public BiomeSettings(string name, float min, float max, Color color, float rough)
    {
        biomeName = name;
        minHeight = min;
        maxHeight = max;
        groundColor = color;
        roughness = rough;
    }
    
    public string BiomeName => biomeName;
    public float MinHeight => minHeight;
    public float MaxHeight => maxHeight;
    public Color GroundColor => groundColor;
    public float Roughness => roughness;
}
