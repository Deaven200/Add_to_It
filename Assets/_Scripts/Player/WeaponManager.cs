using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponManager : MonoBehaviour
{
    // Singleton pattern for persistence across scenes
    public static WeaponManager Instance { get; private set; }
    
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
    
    void Awake()
    {
        // Singleton pattern - ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("WeaponManager: Created persistent instance");
        }
        else
        {
            Debug.Log("WeaponManager: Another instance found, destroying this one");
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        Debug.Log($"WeaponManager: Start called. Available weapons count: {availableWeapons.Count}");
        
        // Load the previously equipped weapon from PlayerPrefs
        LoadEquippedWeapon();
        
        // Hide weapon selection UI initially
        if (weaponSelectionUI != null)
        {
            weaponSelectionUI.SetActive(false);
            Debug.Log("WeaponManager: Weapon selection UI hidden initially");
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
        else
        {
            Debug.Log($"WeaponManager: weaponCardPrefab assigned: {weaponCardPrefab.name}");
        }
        
        if (weaponCardContainer == null)
        {
            Debug.LogError("WeaponManager: weaponCardContainer is null! Please assign the weapon card container in the inspector.");
        }
        else
        {
            Debug.Log($"WeaponManager: weaponCardContainer assigned: {weaponCardContainer.name}");
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
        Debug.Log($"WeaponManager: Scene loaded: {scene.name}");
        
        // Re-find UI references in the new scene
        FindUIReferencesInNewScene();
        
        // Re-spawn weapon if player has one equipped
        if (currentWeapon != null)
        {
            SpawnCurrentWeapon();
        }
    }
    
    private void FindUIReferencesInNewScene()
    {
        Debug.Log("WeaponManager: Finding UI references in new scene...");
        
        // Try to find weapon selection UI
        if (weaponSelectionUI == null)
        {
            weaponSelectionUI = GameObject.Find("WeaponSelectionUI");
            if (weaponSelectionUI != null)
            {
                Debug.Log("WeaponManager: Found weapon selection UI in new scene");
            }
            else
            {
                Debug.LogWarning("WeaponManager: Could not find weapon selection UI in new scene");
            }
        }
        
        // Try to find weapon card container
        if (weaponCardContainer == null)
        {
            weaponCardContainer = GameObject.Find("WeaponCardContainer")?.transform;
            if (weaponCardContainer != null)
            {
                Debug.Log("WeaponManager: Found weapon card container in new scene");
            }
            else
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
                if (weaponHolder != null)
                {
                    Debug.Log("WeaponManager: Found weapon holder in new scene");
                }
                else
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
                if (spawnPoint != null)
                {
                    Debug.Log("WeaponManager: Found spawn point in new scene");
                }
                else
                {
                    Debug.LogWarning("WeaponManager: Could not find spawn point in new scene");
                }
            }
        }
    }
    
    private void SpawnCurrentWeapon()
    {
        if (currentWeapon == null || weaponHolder == null) return;
        
        // Remove current weapon instance if it exists
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
        }
        
        // Spawn new weapon instance
        if (currentWeapon.weaponPrefab != null)
        {
            currentWeaponInstance = Instantiate(currentWeapon.weaponPrefab, weaponHolder);
            Debug.Log($"WeaponManager: Spawned weapon in new scene: {currentWeapon.weaponName}");
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
        currentWeapon = null;
        currentAmmo = 0;
        
        Debug.Log("WeaponManager: Player starts with no weapon (saved weapon loading disabled)");
    }
    
    private void LoadAvailableWeapons()
    {
        // Only load if we don't already have weapons (to avoid duplicates)
        if (availableWeapons.Count > 0)
        {
            Debug.Log("WeaponManager: Available weapons already loaded, skipping load");
            return;
        }
        
        int weaponCount = PlayerPrefs.GetInt("AvailableWeaponsCount", 0);
        Debug.Log($"Loading {weaponCount} available weapons from PlayerPrefs");
        
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
                            Debug.Log($"Loaded weapon: {weapon.weaponName}");
                            break;
                        }
                    }
                }
            }
            
            Debug.Log($"Successfully loaded {availableWeapons.Count} weapons from PlayerPrefs");
        }
        else
        {
            Debug.Log("No saved weapons found in PlayerPrefs");
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
        currentWeapon = null;
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
    public WeaponData GetCurrentWeapon() => currentWeapon;
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => currentWeapon?.maxAmmo ?? 0;
    public bool IsReloading() => isReloading;
    public List<WeaponData> GetAvailableWeapons() => availableWeapons;
    public bool HasWeapon() => currentWeapon != null;
} 