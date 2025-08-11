using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponCard : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI weaponDescriptionText;
    [SerializeField] private TextMeshProUGUI weaponStatsText;
    [SerializeField] private Button selectButton;
    [SerializeField] private Image backgroundImage;
    
    [Header("Visual Feedback")]
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color normalColor = Color.white;
    
    private WeaponData weaponData;
    private WeaponManager weaponManager;
    
    void Start()
    {
        // Set up button click event
        if (selectButton != null)
        {
            selectButton.onClick.AddListener(OnWeaponSelected);
        }
        else
        {
            // Try to get button component if not assigned
            selectButton = GetComponent<Button>();
            if (selectButton != null)
            {
                selectButton.onClick.AddListener(OnWeaponSelected);
            }
            else
            {
                Debug.LogError($"WeaponCard: No Button component found on {gameObject.name}! Make sure the weapon card prefab has a Button component.");
            }
        }
    }
    
    public void SetWeaponData(WeaponData data, WeaponManager manager)
    {
        weaponData = data;
        weaponManager = manager;
        
        UpdateUI();
    }
    
    void UpdateUI()
    {
        if (weaponData == null) 
        {
            Debug.LogError($"WeaponCard: weaponData is null for {gameObject.name}");
            return;
        }
        
        // Set weapon icon
        if (weaponIcon != null && weaponData.weaponIcon != null)
        {
            weaponIcon.sprite = weaponData.weaponIcon;
            weaponIcon.enabled = true;
        }
        else if (weaponIcon != null)
        {
            weaponIcon.enabled = false;
        }
        
        // Set weapon name
        if (weaponNameText != null)
        {
            weaponNameText.text = weaponData.weaponName;
        }
        else
        {
            Debug.LogWarning($"WeaponCard: weaponNameText is null for {gameObject.name}");
        }
        
        // Set weapon description
        if (weaponDescriptionText != null)
        {
            weaponDescriptionText.text = weaponData.description;
        }
        else
        {
            Debug.LogWarning($"WeaponCard: weaponDescriptionText is null for {gameObject.name}");
        }
        
        // Set weapon stats
        if (weaponStatsText != null)
        {
            string stats = $"Damage: {weaponData.damage}\n";
            stats += $"Fire Rate: {weaponData.fireRate}s\n";
            stats += $"Speed: {weaponData.bulletSpeed}\n";
            
            if (weaponData.maxAmmo > 0)
            {
                stats += $"Ammo: {weaponData.maxAmmo}\n";
                stats += $"Reload: {weaponData.reloadTime}s";
            }
            else
            {
                stats += "Ammo: Infinite";
            }
            
            if (weaponData.hasExplosion)
            {
                stats += $"\nExplosion: {weaponData.explosionRadius}m";
            }
            
            if (weaponData.hasPiercing)
            {
                stats += $"\nPierce: {weaponData.pierceCount}";
            }
            
            weaponStatsText.text = stats;
        }
        else
        {
            Debug.LogWarning($"WeaponCard: weaponStatsText is null for {gameObject.name}");
        }
        
        // Update selection visual
        UpdateSelectionVisual();
    }
    
    void UpdateSelectionVisual()
    {
        if (backgroundImage == null || weaponManager == null) return;
        
        PlayerWeaponData currentWeapon = weaponManager.GetCurrentWeapon();
        bool isSelected = currentWeapon != null && currentWeapon.baseWeapon == weaponData;
        backgroundImage.color = isSelected ? selectedColor : normalColor;
    }
    
    void OnWeaponSelected()
    {
        if (weaponData != null && weaponManager != null)
        {
            weaponManager.EquipWeapon(weaponData);
            weaponManager.HideWeaponSelection();
        }
        else
        {
            Debug.LogError($"WeaponCard: Cannot select weapon - weaponData or weaponManager is null for {gameObject.name}");
        }
    }
    
    // Debug method to test button functionality
    [ContextMenu("Test Button Click")]
    public void TestButtonClick()
    {
        OnWeaponSelected();
    }
} 