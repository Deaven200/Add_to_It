using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponManager : MonoBehaviour
{
    // Singleton pattern for persistence across scenes
    public static WeaponManager Instance { get; private set; }
    
    [Header("Weapon Settings")]
    [SerializeField] private List<WeaponData> availableWeapons = new List<WeaponData>();
    [SerializeField] private PlayerWeaponData currentPlayerWeapon;
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
    public event Action<PlayerWeaponData> OnWeaponChanged;
    public event Action<int, int> OnAmmoChanged; // current, max
    
    void Awake()
    {
        // Singleton pattern - ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // Load the previously equipped weapon from PlayerPrefs
        LoadEquippedWeapon();
        
        // Hide weapon selection UI initially
        if (weaponSelectionUI != null)
        {
            weaponSelectionUI.SetActive(false);
        }
        else
        {
            Debug.LogError("WeaponManager: weaponSelectionUI is null! Please assign it in the inspector.");
        }
        
        // Check UI references
        if (weaponCardPrefab == null)
        {
            Debug.LogError("WeaponManager: weaponCardPrefab is null! Please assign the weapon card prefab in the inspector.");
        }
        
        if (weaponCardContainer == null)
        {
            Debug.LogError("WeaponManager: weaponCardContainer is null! Please assign the weapon card container in the inspector.");
        }
    }
    
    // Called when a new scene is loaded
    void OnEnable()
    {
        // Subscribe to scene load events
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        // Unsubscribe from scene load events
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Re-find UI references in the new scene
        FindUIReferencesInNewScene();
        
        // Re-spawn weapon if player has one equipped
        if (currentPlayerWeapon != null)
        {
            SpawnCurrentWeapon();
        }
        else
        {
            // Player starts with no weapon - this is normal
            Debug.Log("WeaponManager: Player starts with no weapon (saved weapon loading disabled)");
        }
    }
    
    private void FindUIReferencesInNewScene()
    {
        // Try to find weapon selection UI
        if (weaponSelectionUI == null)
        {
            weaponSelectionUI = GameObject.Find("WeaponSelectionUI");
            if (weaponSelectionUI == null)
            {
                Debug.LogWarning("WeaponManager: Could not find weapon selection UI in new scene");
            }
        }
        
        // Try to find weapon card container
        if (weaponCardContainer == null)
        {
            weaponCardContainer = GameObject.Find("WeaponCardContainer")?.transform;
            if (weaponCardContainer == null)
            {
                Debug.LogWarning("WeaponManager: Could not find weapon card container in new scene");
            }
        }
        
        // Try to find weapon holder
        if (weaponHolder == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                weaponHolder = player.transform.Find("WeaponHolder");
                if (weaponHolder == null)
                {
                    Debug.LogWarning("WeaponManager: Could not find weapon holder in new scene");
                }
            }
        }
        
        // Try to find spawn point
        if (spawnPoint == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                spawnPoint = player.transform.Find("SpawnPoint");
                if (spawnPoint == null)
                {
                    Debug.LogWarning("WeaponManager: Could not find spawn point in new scene");
                }
            }
        }
    }
    
    private void SpawnCurrentWeapon()
    {
        // Early return if no weapon is equipped
        if (currentPlayerWeapon == null)
        {
            return;
        }
        
        // Early return if no weapon holder found
        if (weaponHolder == null)
        {
            Debug.LogWarning("WeaponManager: No weapon holder found, cannot spawn weapon");
            return;
        }
        
        // Remove current weapon instance if it exists
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
            currentWeaponInstance = null;
        }
        
        // Spawn new weapon instance only if prefab exists
        if (currentPlayerWeapon?.weaponPrefab != null)
        {
            currentWeaponInstance = Instantiate(currentPlayerWeapon.weaponPrefab, weaponHolder);
        }
        else
        {
            string weaponName = currentPlayerWeapon?.weaponName ?? "Unknown Weapon";
            Debug.LogWarning($"WeaponManager: No weapon prefab found for {weaponName}");
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
        
        // Create new player weapon data from base weapon
        currentPlayerWeapon = new PlayerWeaponData(weaponData);
        
        // Spawn weapon model if prefab exists
        if (currentPlayerWeapon.weaponPrefab != null && weaponHolder != null)
        {
            currentWeaponInstance = Instantiate(currentPlayerWeapon.weaponPrefab, weaponHolder);
        }
        
        // Set ammo
        currentAmmo = currentPlayerWeapon.maxAmmo;
        
        // Save the equipped weapon for persistence across scenes
        SaveEquippedWeapon();
        
        // Notify listeners
        OnWeaponChanged?.Invoke(currentPlayerWeapon);
        OnAmmoChanged?.Invoke(currentAmmo, currentPlayerWeapon.maxAmmo);
        
        string weaponName = currentPlayerWeapon?.weaponName ?? "Unknown Weapon";
        Debug.Log($"Equipped weapon: {weaponName}");
    }
    
    public void Shoot()
    {
        // Early return if no weapon or reloading
        if (currentPlayerWeapon == null || isReloading) 
        {
            return;
        }
        
        // Additional safety check - if weapon is null after the first check, return
        if (currentPlayerWeapon == null)
        {
            return;
        }
        
        // Check fire rate
        if (Time.time < nextFireTime)
        {
            return;
        }
        nextFireTime = Time.time + currentPlayerWeapon.fireRate;
        
        // Check ammo
        if (currentPlayerWeapon.maxAmmo > 0 && currentAmmo <= 0)
        {
            Reload();
            return;
        }
        
        // Create bullet only if we have required components
        if (currentPlayerWeapon?.weaponPrefab != null && spawnPoint != null)
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
            
            GameObject bullet = Instantiate(currentPlayerWeapon.weaponPrefab, spawnPoint.position, Quaternion.LookRotation(shootDirection));
            
            // Set bullet properties
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.damage = currentPlayerWeapon.damage;
            }
            
            // Set velocity toward camera direction
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = shootDirection * currentPlayerWeapon.bulletSpeed;
            }
            
            // Play sound
            if (audioSource != null && currentPlayerWeapon.shootSound != null)
            {
                audioSource.PlayOneShot(currentPlayerWeapon.shootSound);
            }
        }
        else
        {
            // Missing required components for shooting
            if (currentPlayerWeapon?.weaponPrefab == null)
            {
                Debug.LogWarning("WeaponManager: Cannot shoot - weapon prefab is null");
            }
            if (spawnPoint == null)
            {
                Debug.LogWarning("WeaponManager: Cannot shoot - spawn point is null");
            }
            return;
        }
        
        // Reduce ammo
        if (currentPlayerWeapon?.maxAmmo > 0)
        {
            currentAmmo--;
            OnAmmoChanged?.Invoke(currentAmmo, currentPlayerWeapon.maxAmmo);
        }
    }
    
    public void Reload()
    {
        if (currentPlayerWeapon == null || isReloading || currentPlayerWeapon.maxAmmo <= 0) return;
        
        isReloading = true;
        reloadTimer = currentPlayerWeapon.reloadTime;
        
        // Play reload sound
        if (audioSource != null && currentPlayerWeapon.reloadSound != null)
        {
            audioSource.PlayOneShot(currentPlayerWeapon.reloadSound);
        }
        
        string weaponName = currentPlayerWeapon?.weaponName ?? "Unknown Weapon";
        Debug.Log($"Reloading {weaponName}...");
    }
    
    private void FinishReload()
    {
        if (currentPlayerWeapon == null)
        {
            isReloading = false;
            return;
        }
        
        isReloading = false;
        currentAmmo = currentPlayerWeapon.maxAmmo;
        OnAmmoChanged?.Invoke(currentAmmo, currentPlayerWeapon.maxAmmo);
        
        string weaponName = currentPlayerWeapon.weaponName;
        Debug.Log($"Reloaded {weaponName}");
    }
    
    public void ShowWeaponSelection(WeaponStation station = null)
    {
        Debug.Log($"WeaponManager: ShowWeaponSelection called. weaponSelectionUI null? {(weaponSelectionUI == null ? "YES" : "NO")}");
        
        if (weaponSelectionUI != null)
        {
            weaponSelectionUI.SetActive(true);
            Debug.Log("WeaponManager: Weapon selection UI activated");
            
            PopulateWeaponList();
            Debug.Log($"WeaponManager: Populated weapon list with {availableWeapons.Count} weapons");
            
            // Store reference to the station that opened this menu
            _currentStation = station;
            
            // Pause game and show cursor
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            Debug.Log("WeaponManager: Game paused, cursor unlocked and visible");
        }
        else
        {
            Debug.LogError("WeaponManager: weaponSelectionUI is null! Please assign the weapon selection UI panel in the inspector.");
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
        Debug.Log($"WeaponManager: PopulateWeaponList called. weaponCardContainer null? {(weaponCardContainer == null ? "YES" : "NO")}, weaponCardPrefab null? {(weaponCardPrefab == null ? "YES" : "NO")}");
        
        if (weaponCardContainer == null || weaponCardPrefab == null) 
        {
            Debug.LogError("WeaponManager: Missing UI references! weaponCardContainer or weaponCardPrefab is null.");
            return;
        }
        
        // Clear existing cards
        foreach (Transform child in weaponCardContainer)
        {
            Destroy(child.gameObject);
        }
        
        Debug.Log($"WeaponManager: Creating weapon cards for {availableWeapons.Count} weapons");
        
        // Create cards for each weapon
        foreach (WeaponData weapon in availableWeapons)
        {
            GameObject cardGO = Instantiate(weaponCardPrefab, weaponCardContainer);
            WeaponCard weaponCard = cardGO.GetComponent<WeaponCard>();
            if (weaponCard != null)
            {
                weaponCard.SetWeaponData(weapon, this);
                Debug.Log($"WeaponManager: Created weapon card for {weapon.weaponName}");
            }
            else
            {
                Debug.LogError($"WeaponManager: WeaponCard component not found on prefab for {weapon.weaponName}");
            }
        }
    }
    
    public void AddWeapon(WeaponData weaponData)
    {
        if (weaponData == null)
        {
            Debug.LogError("WeaponManager: Cannot add null weapon data!");
            return;
        }
        
        if (!availableWeapons.Contains(weaponData))
        {
            availableWeapons.Add(weaponData);
            Debug.Log($"WeaponManager: Added weapon: {weaponData.weaponName}. Total weapons: {availableWeapons.Count}");
        }
        else
        {
            Debug.Log($"WeaponManager: Weapon {weaponData.weaponName} already exists in available weapons.");
        }
    }
    
    // Debug method to add weapons for testing
    [ContextMenu("Add Test Weapons")]
    public void AddTestWeapons()
    {
        Debug.Log("WeaponManager: Adding test weapons...");
        
        // Try to find weapon data assets
        WeaponData[] allWeaponData = Resources.FindObjectsOfTypeAll<WeaponData>();
        Debug.Log($"WeaponManager: Found {allWeaponData.Length} weapon data assets");
        
        foreach (WeaponData weapon in allWeaponData)
        {
            AddWeapon(weapon);
        }
        
        Debug.Log($"WeaponManager: Test weapons added. Total available weapons: {availableWeapons.Count}");
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
        if (currentPlayerWeapon != null)
        {
            PlayerPrefs.SetString("EquippedWeapon", currentPlayerWeapon.baseWeapon.weaponName);
            PlayerPrefs.SetInt("CurrentAmmo", currentAmmo);
            
            // Save upgrade modifiers
            PlayerPrefs.SetFloat("DamageModifier", currentPlayerWeapon.damageModifier);
            PlayerPrefs.SetFloat("FireRateModifier", currentPlayerWeapon.fireRateModifier);
            PlayerPrefs.SetFloat("BulletSpeedModifier", currentPlayerWeapon.bulletSpeedModifier);
            PlayerPrefs.SetInt("AmmoModifier", currentPlayerWeapon.ammoModifier);
            PlayerPrefs.SetFloat("ReloadTimeModifier", currentPlayerWeapon.reloadTimeModifier);
            
            PlayerPrefs.Save();
            Debug.Log($"Saved equipped weapon: {currentPlayerWeapon.weaponName}");
        }
        else
        {
            PlayerPrefs.DeleteKey("EquippedWeapon");
            PlayerPrefs.DeleteKey("CurrentAmmo");
            PlayerPrefs.DeleteKey("DamageModifier");
            PlayerPrefs.DeleteKey("FireRateModifier");
            PlayerPrefs.DeleteKey("BulletSpeedModifier");
            PlayerPrefs.DeleteKey("AmmoModifier");
            PlayerPrefs.DeleteKey("ReloadTimeModifier");
            PlayerPrefs.Save();
        }
        
        // Save available weapons list
        SaveAvailableWeapons();
    }
    
    private void SaveAvailableWeapons()
    {
        // Save the count of available weapons
        PlayerPrefs.SetInt("AvailableWeaponsCount", availableWeapons.Count);
        
        // Save each weapon by name
        for (int i = 0; i < availableWeapons.Count; i++)
        {
            if (availableWeapons[i] != null)
            {
                PlayerPrefs.SetString($"AvailableWeapon_{i}", availableWeapons[i].weaponName);
            }
        }
        
        PlayerPrefs.Save();
        Debug.Log($"Saved {availableWeapons.Count} available weapons");
    }
    
    private void LoadEquippedWeapon()
    {
        // Load available weapons first
        LoadAvailableWeapons();
        
        // Don't automatically load equipped weapon on start - player should start with no weapon
        // Only load if explicitly requested (e.g., for save/load system)
        currentPlayerWeapon = null;
        currentAmmo = 0;
        
    }
    
    private void LoadAvailableWeapons()
    {
        // Only load if we don't already have weapons (to avoid duplicates)
        if (availableWeapons.Count > 0)
        {
            return;
        }
        
        int weaponCount = PlayerPrefs.GetInt("AvailableWeaponsCount", 0);
        
        if (weaponCount > 0)
        {
            // Find all weapon data assets
            WeaponData[] allWeaponData = Resources.FindObjectsOfTypeAll<WeaponData>();
            
            // Load each saved weapon by name
            for (int i = 0; i < weaponCount; i++)
            {
                string weaponName = PlayerPrefs.GetString($"AvailableWeapon_{i}", "");
                if (!string.IsNullOrEmpty(weaponName))
                {
                    // Find the weapon data by name
                    foreach (WeaponData weapon in allWeaponData)
                    {
                        if (weapon.weaponName == weaponName)
                        {
                            availableWeapons.Add(weapon);
                            break;
                        }
                    }
                }
            }
            
        }
    }
    
    // Method to clear weapon (for testing or game reset)
    public void ClearEquippedWeapon()
    {
        currentPlayerWeapon = null;
        currentAmmo = 0;
        
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
            currentWeaponInstance = null;
        }
        
        SaveEquippedWeapon(); // This will delete the saved data
        
        OnWeaponChanged?.Invoke(null);
        OnAmmoChanged?.Invoke(0, 0);
    }
    
    // Method to clear all saved weapon data
    [ContextMenu("Clear All Saved Weapon Data")]
    public void ClearAllSavedWeaponData()
    {
        // Clear equipped weapon data
        PlayerPrefs.DeleteKey("EquippedWeapon");
        PlayerPrefs.DeleteKey("CurrentAmmo");
        
        // Clear available weapons data
        PlayerPrefs.DeleteKey("AvailableWeaponsCount");
        
        // Clear any individual weapon saves
        int weaponCount = PlayerPrefs.GetInt("AvailableWeaponsCount", 0);
        for (int i = 0; i < weaponCount; i++)
        {
            PlayerPrefs.DeleteKey($"AvailableWeapon_{i}");
        }
        
        PlayerPrefs.Save();
        
        // Clear current weapon
        currentPlayerWeapon = null;
        currentAmmo = 0;
        availableWeapons.Clear();
        
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
            currentWeaponInstance = null;
        }
        
        OnWeaponChanged?.Invoke(null);
        OnAmmoChanged?.Invoke(0, 0);
        
        Debug.Log("WeaponManager: Cleared all saved weapon data - player will start fresh");
    }
    
    // Method to reset weapon system completely
    [ContextMenu("Reset Weapon System")]
    public void ResetWeaponSystem()
    {
        ClearAllSavedWeaponData();
        Debug.Log("WeaponManager: Weapon system reset - restart the game to see changes");
    }
    
    // Getters
    public PlayerWeaponData GetCurrentWeapon() => currentPlayerWeapon;
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => currentPlayerWeapon?.maxAmmo ?? 0;
    public bool IsReloading() => isReloading;
    public List<WeaponData> GetAvailableWeapons() => availableWeapons;
    public bool HasWeapon() => currentPlayerWeapon != null;
    
    // Debug method to print current weapon stats
    [ContextMenu("Print Current Weapon Stats")]
    public void PrintCurrentWeaponStats()
    {
        if (currentPlayerWeapon != null)
        {
            Debug.Log($"=== Current Weapon Stats ===");
            Debug.Log($"Weapon: {currentPlayerWeapon.weaponName}");
            Debug.Log($"Base Damage: {currentPlayerWeapon.baseWeapon.damage}");
            Debug.Log($"Current Damage: {currentPlayerWeapon.damage}");
            Debug.Log($"Damage Modifier: +{currentPlayerWeapon.damageModifier}");
            Debug.Log($"Base Fire Rate: {currentPlayerWeapon.baseWeapon.fireRate}s");
            Debug.Log($"Current Fire Rate: {currentPlayerWeapon.fireRate}s");
            Debug.Log($"Fire Rate Modifier: +{currentPlayerWeapon.fireRateModifier}%");
            Debug.Log($"Base Bullet Speed: {currentPlayerWeapon.baseWeapon.bulletSpeed}");
            Debug.Log($"Current Bullet Speed: {currentPlayerWeapon.bulletSpeed}");
            Debug.Log($"Bullet Speed Modifier: +{currentPlayerWeapon.bulletSpeedModifier}");
            Debug.Log($"Base Ammo: {currentPlayerWeapon.baseWeapon.maxAmmo}");
            Debug.Log($"Current Ammo: {currentPlayerWeapon.maxAmmo}");
            Debug.Log($"Ammo Modifier: +{currentPlayerWeapon.ammoModifier}");
            Debug.Log($"Base Reload Time: {currentPlayerWeapon.baseWeapon.reloadTime}s");
            Debug.Log($"Current Reload Time: {currentPlayerWeapon.reloadTime}s");
            Debug.Log($"Reload Time Modifier: -{currentPlayerWeapon.reloadTimeModifier}s");
            Debug.Log($"=============================");
        }
        else
        {
            Debug.Log("No weapon currently equipped!");
        }
    }

    /// <summary>
    /// Initialize the weapon manager (called by PlayerSpawner)
    /// </summary>
    public void Initialize()
    {
        // Reload weapon if one is equipped
        if (currentPlayerWeapon != null && currentPlayerWeapon.maxAmmo > 0)
        {
            currentAmmo = currentPlayerWeapon.maxAmmo;
            OnAmmoChanged?.Invoke(currentAmmo, currentPlayerWeapon.maxAmmo);
        }
        
        // Reset fire rate timer
        nextFireTime = 0f;
        isReloading = false;
        reloadTimer = 0f;
    }
} 