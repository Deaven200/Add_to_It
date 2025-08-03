using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrades/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    [TextArea] public string description;
    public Sprite icon;

    // This enum defines what kind of upgrade this is
    public UpgradeType upgradeType;

    // Generic numeric value (like +1 damage, +10% speed, etc.)
    public float value;

    public enum UpgradeType
    {
        Damage,
        MoveSpeed,
        CoinMagnetRange,
        DamageResistance,
        LifeOnKill,
        FireRate,
        ExplosionOnKill,
        ChestDropRate,
        Shield,
        Health,
        // You can add more types later
    }
}
