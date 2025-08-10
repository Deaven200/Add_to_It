using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Info")]
    public string weaponName;
    [TextArea] public string description;
    public Sprite weaponIcon;
    public GameObject weaponPrefab;
    
    [Header("Weapon Stats")]
    public float damage = 10f;
    public float fireRate = 0.5f;
    public float bulletSpeed = 20f;
    public int maxAmmo = -1; // -1 for infinite ammo
    public float reloadTime = 1f;
    
    [Header("Weapon Type")]
    public WeaponType weaponType;
    
    [Header("Special Effects")]
    public bool hasExplosion = false;
    public float explosionRadius = 2f;
    public bool hasPiercing = false;
    public int pierceCount = 1;
    
    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    
    public enum WeaponType
    {
        Pistol,
        Rifle,
        Shotgun,
        Sniper,
        Rocket,
        Laser,
        Grenade,
        Melee,
        Special
    }
} 