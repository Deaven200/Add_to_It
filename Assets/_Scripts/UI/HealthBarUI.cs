using UnityEngine;
using TMPro;

/// <summary>
/// Health-specific wrapper for the BarUI component.
/// Handles health-specific logic and connects to PlayerHealth component.
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private BarUI healthBar;
    [SerializeField] private TextMeshProUGUI healthText; // Optional additional text display
    
    private PlayerHealth playerHealth;
    
    void Start()
    {
        // Find the player health component
        playerHealth = FindObjectOfType<PlayerHealth>();
        
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth component not found! Make sure there's a PlayerHealth component in the scene.");
            return;
        }
        
        // Subscribe to health changes
        playerHealth.OnHealthChanged += UpdateHealthBar;
        
        // Initialize the health bar
        InitializeHealthBar();
    }
    
    void InitializeHealthBar()
    {
        if (healthBar != null && playerHealth != null)
        {
            // Set initial values
            healthBar.SetValue(playerHealth.currentHealth, playerHealth.maxHealth);
            
            // Subscribe to bar value changes for additional text updates
            healthBar.OnValueChanged += OnHealthBarValueChanged;
        }
        
        UpdateHealthText();
    }
    
    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.SetValue(currentHealth, maxHealth);
        }
        
        UpdateHealthText();
    }
    
    private void OnHealthBarValueChanged(int current, int max)
    {
        // This is called when the bar value changes (useful for additional UI updates)
        UpdateHealthText();
    }
    
    void UpdateHealthText()
    {
        if (healthText != null && playerHealth != null)
        {
            healthText.text = $"{playerHealth.currentHealth} / {playerHealth.maxHealth}";
    
        }
        else if (healthText == null)
        {
            Debug.LogWarning("HealthBarUI: healthText is null - make sure to assign a TextMeshProUGUI component in the inspector");
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from health changes to prevent memory leaks
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthBar;
        }
        
        if (healthBar != null)
        {
            healthBar.OnValueChanged -= OnHealthBarValueChanged;
        }
    }
} 