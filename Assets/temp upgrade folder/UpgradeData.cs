using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrades/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public string description;
    public Sprite icon;

    // You can expand this with different upgrade effects
    public enum UpgradeType { Health, Speed, Damage }
    public UpgradeType type;
    public float value;
}
