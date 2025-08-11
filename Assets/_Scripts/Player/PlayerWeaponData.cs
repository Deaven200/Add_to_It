using UnityEngine;

[System.Serializable]
public class PlayerWeaponData
{
    [Header("Base Weapon Reference")]
    public WeaponData baseWeapon;
    
    [Header("Modified Stats")]
    public float damage;
    public float fireRate;
    public float bulletSpeed;
    public int maxAmmo;
    public float reloadTime;
    
    [Header("Special Effects")]
    public bool hasExplosion;
    public float explosionRadius;
    public bool hasPiercing;
    public int pierceCount;
    
    [Header("Upgrade Modifiers")]
    public float damageModifier = 0f;
    public float fireRateModifier = 0f;
    public float bulletSpeedModifier = 0f;
    public int ammoModifier = 0;
    public float reloadTimeModifier = 0f;
    
    public PlayerWeaponData(WeaponData baseWeaponData)
    {
        if (baseWeaponData == null)
        {
            Debug.LogError("PlayerWeaponData: Cannot create player weapon data with null base weapon!");
            return;
        }
        
        baseWeapon = baseWeaponData;
        
        // Copy base stats
        damage = baseWeaponData.damage;
        fireRate = baseWeaponData.fireRate;
        bulletSpeed = baseWeaponData.bulletSpeed;
        maxAmmo = baseWeaponData.maxAmmo;
        reloadTime = baseWeaponData.reloadTime;
        hasExplosion = baseWeaponData.hasExplosion;
        explosionRadius = baseWeaponData.explosionRadius;
        hasPiercing = baseWeaponData.hasPiercing;
        pierceCount = baseWeaponData.pierceCount;
    }
    
    // Apply damage upgrade
    public void AddDamageModifier(float amount)
    {
        damageModifier += amount;
        damage = baseWeapon.damage + damageModifier;
    }
    
    // Apply fire rate upgrade (lower fire rate = faster shooting)
    public void AddFireRateModifier(float percentageReduction)
    {
        fireRateModifier += percentageReduction;
        float reduction = baseWeapon.fireRate * (fireRateModifier / 100f);
        fireRate = Mathf.Max(0.1f, baseWeapon.fireRate - reduction);
    }
    
    // Apply bullet speed upgrade
    public void AddBulletSpeedModifier(float amount)
    {
        bulletSpeedModifier += amount;
        bulletSpeed = baseWeapon.bulletSpeed + bulletSpeedModifier;
    }
    
    // Apply ammo upgrade
    public void AddAmmoModifier(int amount)
    {
        ammoModifier += amount;
        maxAmmo = baseWeapon.maxAmmo + ammoModifier;
    }
    
    // Apply reload time upgrade (lower reload time = faster reloading)
    public void AddReloadTimeModifier(float amount)
    {
        reloadTimeModifier += amount;
        reloadTime = Mathf.Max(0.1f, baseWeapon.reloadTime - reloadTimeModifier);
    }
    
    // Get weapon name (delegates to base weapon)
    public string weaponName => baseWeapon.weaponName;
    
    // Get weapon description (delegates to base weapon)
    public string description => baseWeapon.description;
    
    // Get weapon icon (delegates to base weapon)
    public Sprite weaponIcon => baseWeapon.weaponIcon;
    
    // Get weapon prefab (delegates to base weapon)
    public GameObject weaponPrefab => baseWeapon.weaponPrefab;
    
    // Get weapon type (delegates to base weapon)
    public WeaponData.WeaponType weaponType => baseWeapon.weaponType;
    
    // Get audio clips (delegates to base weapon)
    public AudioClip shootSound => baseWeapon.shootSound;
    public AudioClip reloadSound => baseWeapon.reloadSound;
}
