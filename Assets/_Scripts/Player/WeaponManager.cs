using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private List<WeaponData> availableWeapons = new List<WeaponData>();
    [SerializeField] private WeaponData currentWeapon;
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Transform spawnPoint;
    
    [Header("UI References")]
    [SerializeField] private GameObject weaponSelectionUI;
    [SerializeField] private GameObject weaponCardPrefab;
    [SerializeField] private Transform weaponCardContainer;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    
    private GameObject currentWeaponInstance;
    private int currentAmmo;
    private bool isReloading = false;
    private float reloadTimer = 0f;
    private float nextFireTime = 0f; // Fire rate control
    private WeaponStation _currentStation; // Reference to the station that opened this menu
    
    // Events
    public event Action<WeaponData> OnWeaponChanged;
    public event Action<int, int> OnAmmoChanged; // current, max
    
    void Start()
    {
        // Load the previously equipped weapon from PlayerPrefs
        LoadEquippedWeapon();
        
        // Hide weapon selection UI initially
        if (weaponSelectionUI != null)
        {
            weaponSelectionUI.SetActive(false);
        }
        
        if (currentWeapon == null)
        {
            Debug.Log("WeaponManager initialized. Player starts with no weapon. Visit weapon station to get a weapon.");
        }
        else
        {
            Debug.Log($"WeaponManager initialized. Equipped weapon: {currentWeapon.weaponName}");
        }
    }
    
    void Update()
    {
        // Handle reloading
        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0f)
            {
                FinishReload();
            }
        }
        
        // Quick weapon switching with number keys (1-9) - only if player has weapons
        if (availableWeapons.Count > 0)
        {
            for (int i = 0; i < Mathf.Min(availableWeapons.Count, 9); i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    EquipWeapon(availableWeapons[i]);
                }
            }
        }
    }
    
    public void EquipWeapon(WeaponData weaponData)
    {
        if (weaponData == null) return;
        
        // Remove current weapon
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
        }
        
        // Set new weapon
        currentWeapon = weaponData;
        
        // Spawn weapon model if prefab exists
        if (weaponData.weaponPrefab != null && weaponHolder != null)
        {
            currentWeaponInstance = Instantiate(weaponData.weaponPrefab, weaponHolder);
        }
        
        // Set ammo
        currentAmmo = weaponData.maxAmmo;
        
        // Save the equipped weapon for persistence across scenes
        SaveEquippedWeapon();
        
        // Notify listeners
        OnWeaponChanged?.Invoke(currentWeapon);
        OnAmmoChanged?.Invoke(currentAmmo, weaponData.maxAmmo);
        
        Debug.Log($"Equipped weapon: {weaponData.weaponName}");
    }
    
    public void Shoot()
    {
        if (currentWeapon == null || isReloading) 
        {
            Debug.Log("No weapon equipped or currently reloading!");
            return;
        }
        
        // Check fire rate
        if (Time.time < nextFireTime)
        {
            return;
        }
        nextFireTime = Time.time + currentWeapon.fireRate;
        
        // Check ammo
        if (currentWeapon.maxAmmo > 0 && currentAmmo <= 0)
        {
            Reload();
            return;
        }
        
        // Create bullet
        if (currentWeapon.weaponPrefab != null && spawnPoint != null)
        {
            // Get camera direction for accurate shooting
            Camera playerCamera = Camera.main;
            if (playerCamera == null)
            {
                playerCamera = FindObjectOfType<Camera>();
            }
            
            Vector3 shootDirection = spawnPoint.forward; // Default direction
            if (playerCamera != null)
            {
                shootDirection = playerCamera.transform.forward;
            }
            
            GameObject bullet = Instantiate(currentWeapon.weaponPrefab, spawnPoint.position, Quaternion.LookRotation(shootDirection));
            
            // Set bullet properties
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.damage = currentWeapon.damage;
            }
            
            // Set velocity toward camera direction
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = shootDirection * currentWeapon.bulletSpeed;
            }
            
            // Play sound
            if (audioSource != null && currentWeapon.shootSound != null)
            {
                audioSource.PlayOneShot(currentWeapon.shootSound);
            }
            
            Debug.Log($"Shot fired! Direction: {shootDirection}");
        }
        
        // Reduce ammo
        if (currentWeapon.maxAmmo > 0)
        {
            currentAmmo--;
            OnAmmoChanged?.Invoke(currentAmmo, currentWeapon.maxAmmo);
        }
    }
    
    public void Reload()
    {
        if (currentWeapon == null || isReloading || currentWeapon.maxAmmo <= 0) return;
        
        isReloading = true;
        reloadTimer = currentWeapon.reloadTime;
        
        // Play reload sound
        if (audioSource != null && currentWeapon.reloadSound != null)
        {
            audioSource.PlayOneShot(currentWeapon.reloadSound);
        }
        
        Debug.Log($"Reloading {currentWeapon.weaponName}...");
    }
    
    private void FinishReload()
    {
        isReloading = false;
        currentAmmo = currentWeapon.maxAmmo;
        OnAmmoChanged?.Invoke(currentAmmo, currentWeapon.maxAmmo);
        Debug.Log($"Reloaded {currentWeapon.weaponName}");
    }
    
    public void ShowWeaponSelection(WeaponStation station = null)
    {
        if (weaponSelectionUI != null)
        {
            weaponSelectionUI.SetActive(true);
            PopulateWeaponList();
            
            // Store reference to the station that opened this menu
            _currentStation = station;
            
            // Pause game and show cursor
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    public void HideWeaponSelection()
    {
        if (weaponSelectionUI != null)
        {
            weaponSelectionUI.SetActive(false);
            
            // Resume game and hide cursor
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    private void PopulateWeaponList()
    {
        if (weaponCardContainer == null || weaponCardPrefab == null) return;
        
        // Clear existing cards
        foreach (Transform child in weaponCardContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create cards for each weapon
        foreach (WeaponData weapon in availableWeapons)
        {
            GameObject cardGO = Instantiate(weaponCardPrefab, weaponCardContainer);
            WeaponCard weaponCard = cardGO.GetComponent<WeaponCard>();
            if (weaponCard != null)
            {
                weaponCard.SetWeaponData(weapon, this);
            }
        }
    }
    
    public void AddWeapon(WeaponData weaponData)
    {
        if (!availableWeapons.Contains(weaponData))
        {
            availableWeapons.Add(weaponData);
            Debug.Log($"Added weapon: {weaponData.weaponName}");
        }
    }
    
    public void RemoveWeapon(WeaponData weaponData)
    {
        if (availableWeapons.Contains(weaponData))
        {
            availableWeapons.Remove(weaponData);
            Debug.Log($"Removed weapon: {weaponData.weaponName}");
        }
    }
    
    // Save/Load weapon persistence
    private void SaveEquippedWeapon()
    {
        if (currentWeapon != null)
        {
            PlayerPrefs.SetString("EquippedWeapon", currentWeapon.weaponName);
            PlayerPrefs.SetInt("CurrentAmmo", currentAmmo);
            PlayerPrefs.Save();
            Debug.Log($"Saved equipped weapon: {currentWeapon.weaponName}");
        }
        else
        {
            PlayerPrefs.DeleteKey("EquippedWeapon");
            PlayerPrefs.DeleteKey("CurrentAmmo");
            PlayerPrefs.Save();
        }
    }
    
    private void LoadEquippedWeapon()
    {
        string savedWeaponName = PlayerPrefs.GetString("EquippedWeapon", "");
        int savedAmmo = PlayerPrefs.GetInt("CurrentAmmo", 0);
        
        if (!string.IsNullOrEmpty(savedWeaponName))
        {
            // Find the weapon data by name
            WeaponData weaponToEquip = null;
            foreach (WeaponData weapon in availableWeapons)
            {
                if (weapon.weaponName == savedWeaponName)
                {
                    weaponToEquip = weapon;
                    break;
                }
            }
            
            if (weaponToEquip != null)
            {
                // Set weapon without calling SaveEquippedWeapon to avoid infinite loop
                currentWeapon = weaponToEquip;
                
                // Spawn weapon model if prefab exists
                if (weaponToEquip.weaponPrefab != null && weaponHolder != null)
                {
                    currentWeaponInstance = Instantiate(weaponToEquip.weaponPrefab, weaponHolder);
                }
                
                // Restore saved ammo or use max ammo
                currentAmmo = (savedAmmo > 0) ? savedAmmo : weaponToEquip.maxAmmo;
                
                // Notify listeners
                OnWeaponChanged?.Invoke(currentWeapon);
                OnAmmoChanged?.Invoke(currentAmmo, weaponToEquip.maxAmmo);
                
                Debug.Log($"Loaded equipped weapon: {weaponToEquip.weaponName} with {currentAmmo} ammo");
            }
            else
            {
                Debug.LogWarning($"Could not find saved weapon: {savedWeaponName}");
                // Clear invalid save data
                PlayerPrefs.DeleteKey("EquippedWeapon");
                PlayerPrefs.DeleteKey("CurrentAmmo");
                currentWeapon = null;
                currentAmmo = 0;
            }
        }
        else
        {
            // No saved weapon
            currentWeapon = null;
            currentAmmo = 0;
        }
    }
    
    // Method to clear weapon (for testing or game reset)
    public void ClearEquippedWeapon()
    {
        currentWeapon = null;
        currentAmmo = 0;
        
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
            currentWeaponInstance = null;
        }
        
        SaveEquippedWeapon(); // This will delete the saved data
        
        OnWeaponChanged?.Invoke(null);
        OnAmmoChanged?.Invoke(0, 0);
        
        Debug.Log("Cleared equipped weapon");
    }
    
    // Getters
    public WeaponData GetCurrentWeapon() => currentWeapon;
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => currentWeapon?.maxAmmo ?? 0;
    public bool IsReloading() => isReloading;
    public List<WeaponData> GetAvailableWeapons() => availableWeapons;
    public bool HasWeapon() => currentWeapon != null;
} 