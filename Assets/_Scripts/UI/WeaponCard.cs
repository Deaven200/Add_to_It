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
        if (weaponData == null) return;
        
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
        
        // Set weapon description
        if (weaponDescriptionText != null)
        {
            weaponDescriptionText.text = weaponData.description;
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
        
        // Update selection visual
        UpdateSelectionVisual();
    }
    
    void UpdateSelectionVisual()
    {
        if (backgroundImage == null || weaponManager == null) return;
        
        bool isSelected = weaponManager.GetCurrentWeapon() == weaponData;
        backgroundImage.color = isSelected ? selectedColor : normalColor;
    }
    
    void OnWeaponSelected()
    {
        if (weaponData != null && weaponManager != null)
        {
            weaponManager.EquipWeapon(weaponData);
            weaponManager.HideWeaponSelection();
            
            Debug.Log($"Selected weapon: {weaponData.weaponName}");
        }
    }
} 