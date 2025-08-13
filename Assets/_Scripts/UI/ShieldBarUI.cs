using UnityEngine;
using TMPro;

/// <summary>
/// Shield-specific wrapper for the BarUI component.
/// Handles shield-specific logic and connects to PlayerShield component.
/// </summary>
public class ShieldBarUI : MonoBehaviour
{
    [Header("Shield Bar")]
    [SerializeField] private BarUI shieldBar;
    [SerializeField] private TextMeshProUGUI shieldText; // Optional additional text display
    
    private PlayerShield playerShield;
    
    void Start()
    {
        // Find the player shield component
        playerShield = FindObjectOfType<PlayerShield>();
        
        if (playerShield == null)
        {
            Debug.LogError("PlayerShield component not found! Make sure there's a PlayerShield component in the scene.");
            return;
        }
        
        // Subscribe to shield changes
        playerShield.OnShieldChanged += UpdateShieldBar;
        
        // Initialize the shield bar
        InitializeShieldBar();
    }
    
    void InitializeShieldBar()
    {
        if (shieldBar != null && playerShield != null)
        {
            // Set initial values
            shieldBar.SetValue(playerShield.currentShield, playerShield.maxShield);
            
            // Subscribe to bar value changes for additional text updates
            shieldBar.OnValueChanged += OnShieldBarValueChanged;
        }
        
        UpdateShieldText();
    }
    
    public void UpdateShieldBar(int currentShield, int maxShield)
    {
        if (shieldBar != null)
        {
            shieldBar.SetValue(currentShield, maxShield);
        }
        
        UpdateShieldText();
    }
    
    private void OnShieldBarValueChanged(int current, int max)
    {
        // This is called when the bar value changes (useful for additional UI updates)
        UpdateShieldText();
    }
    
    void UpdateShieldText()
    {
        if (shieldText != null && playerShield != null)
        {
            shieldText.text = $"{playerShield.currentShield} / {playerShield.maxShield}";
            Debug.Log($"ShieldBarUI: Updated text to '{shieldText.text}'");
        }
        else if (shieldText == null)
        {
            Debug.LogWarning("ShieldBarUI: shieldText is null - make sure to assign a TextMeshProUGUI component in the inspector");
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from shield changes to prevent memory leaks
        if (playerShield != null)
        {
            playerShield.OnShieldChanged -= UpdateShieldBar;
        }
        
        if (shieldBar != null)
        {
            shieldBar.OnValueChanged -= OnShieldBarValueChanged;
        }
    }
}
