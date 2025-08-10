using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [Header("Health Bar Components")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image backgroundImage;
    
    [Header("Health Bar Settings")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private Color mediumHealthColor = Color.yellow;
    [SerializeField] private float lowHealthThreshold = 0.3f; // 30% health
    [SerializeField] private float mediumHealthThreshold = 0.6f; // 60% health
    
    [Header("Animation Settings")]
    [SerializeField] private bool animateHealthBar = true;
    [SerializeField] private float animationSpeed = 5f;
    
    private PlayerHealth playerHealth;
    private float targetHealthValue;
    
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
    
    void Update()
    {
        // Animate the health bar if enabled
        if (animateHealthBar && healthSlider != null)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetHealthValue, Time.deltaTime * animationSpeed);
        }
    }
    
    void InitializeHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = playerHealth.maxHealth;
            healthSlider.value = playerHealth.currentHealth;
            targetHealthValue = playerHealth.currentHealth;
        }
        
        UpdateHealthText();
        UpdateHealthBarColor();
    }
    
    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthSlider != null)
        {
            targetHealthValue = currentHealth;
            
            // If animation is disabled, update immediately
            if (!animateHealthBar)
            {
                healthSlider.value = currentHealth;
            }
        }
        
        UpdateHealthText();
        UpdateHealthBarColor();
    }
    
    void UpdateHealthText()
    {
        if (healthText != null && playerHealth != null)
        {
            healthText.text = $"{playerHealth.currentHealth} / {playerHealth.maxHealth}";
        }
    }
    
    void UpdateHealthBarColor()
    {
        if (fillImage == null || playerHealth == null) return;
        
        float healthPercentage = (float)playerHealth.currentHealth / playerHealth.maxHealth;
        
        if (healthPercentage <= lowHealthThreshold)
        {
            fillImage.color = lowHealthColor;
        }
        else if (healthPercentage <= mediumHealthThreshold)
        {
            fillImage.color = mediumHealthColor;
        }
        else
        {
            fillImage.color = fullHealthColor;
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from health changes to prevent memory leaks
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }
} 