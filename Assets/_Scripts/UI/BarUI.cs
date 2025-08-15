using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Generic bar UI component that can be used for health, stamina, or any other bar-based UI.
/// Uses a container and fill approach where the fill image shrinks/grows based on the current value.
/// </summary>
public class BarUI : MonoBehaviour
{
    [Header("Bar Components")]
    [SerializeField] private Image barContainer; // The background/container image
    [SerializeField] private Image barFill; // The fill image that shows current value
    [SerializeField] private TextMeshProUGUI valueText; // Optional text display
    [SerializeField] private bool useInternalText = true; // Whether to use the internal text component
    
    [Header("Bar Settings")]
    [SerializeField] private int maxValue = 100;
    [SerializeField] private int currentValue = 100;
    [SerializeField] private string valueFormat = "{0} / {1}"; // Format for text display
    
    [Header("Bar Scaling")]
    [SerializeField] private bool useDynamicScaling = true;
    [SerializeField] private float baseBarWidth = 200f; // Base width for 100 health
    [SerializeField] private float maxBarWidth = 800f; // Maximum bar width (can be very high)
    [SerializeField] private int scalingInterval = 100; // Health interval for scaling changes
    [SerializeField] private float baseScalingFactor = 1f; // Starting scaling factor (1:1)
    
    [Header("Animation Settings")]
    [SerializeField] private bool animateBar = true;
    [SerializeField] private float animationSpeed = 5f;
    
    [Header("Fill Direction")]
    [SerializeField] private FillDirection fillDirection = FillDirection.LeftToRight;
    
    private float targetFillAmount;
    private Vector2 originalFillSize;
    private Vector2 originalFillPosition;
    private Vector2 originalContainerSize;
    
    public enum FillDirection
    {
        LeftToRight,
        RightToLeft,
        TopToBottom,
        BottomToTop
    }
    
    // Events
    public System.Action<int, int> OnValueChanged;
    
    void Start()
    {
        InitializeBar();
    }
    
    void Update()
    {
        // Animate the bar if enabled
        if (animateBar && barFill != null)
        {
            float currentFillAmount = GetCurrentFillAmount();
            float newFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * animationSpeed);
            
            UpdateFillVisual(newFillAmount);
        }
    }
    
    void InitializeBar()
    {
        if (barFill != null)
        {
            // Store original size and position
            originalFillSize = barFill.rectTransform.sizeDelta;
            originalFillPosition = barFill.rectTransform.anchoredPosition;
        }
        
        if (barContainer != null)
        {
            // Store original container size
            originalContainerSize = barContainer.rectTransform.sizeDelta;
        }
        
        // Set initial values
        SetValue(currentValue, maxValue);
    }
    
    /// <summary>
    /// Sets the current and maximum values for the bar
    /// </summary>
    public void SetValue(int current, int max)
    {
        currentValue = Mathf.Clamp(current, 0, max);
        maxValue = max;
        
        // Update bar size based on max value if dynamic scaling is enabled
        if (useDynamicScaling)
        {
            UpdateBarSize();
        }
        
        float fillPercentage = (float)currentValue / maxValue;
        targetFillAmount = fillPercentage;
        
        // Update immediately if animation is disabled
        if (!animateBar)
        {
            UpdateFillVisual(fillPercentage);
        }
        
        UpdateValueText();
        
        // Trigger event
        OnValueChanged?.Invoke(currentValue, maxValue);
    }
    
    /// <summary>
    /// Updates the bar's visual size based on the max value using exponential scaling system
    /// </summary>
    private void UpdateBarSize()
    {
        if (barContainer == null || barFill == null) return;
        
        float newWidth = CalculateBarWidth(maxValue);
        
        // Update container size
        Vector2 containerSize = barContainer.rectTransform.sizeDelta;
        containerSize.x = newWidth;
        barContainer.rectTransform.sizeDelta = containerSize;
        
        // Update fill size to match new container
        Vector2 fillSize = barFill.rectTransform.sizeDelta;
        fillSize.x = newWidth;
        barFill.rectTransform.sizeDelta = fillSize;
        
        // Store new original sizes for calculations
        originalFillSize = fillSize;
        originalContainerSize = containerSize;
    }
    
    /// <summary>
    /// Calculates the visual width of the bar based on max health using exponential scaling system
    /// </summary>
    private float CalculateBarWidth(int health)
    {
        if (health <= 0) return 0f;
        
        float totalWidth = 0f;
        int remainingHealth = health;
        int currentInterval = 0;
        float currentScalingFactor = baseScalingFactor;
        
        // Calculate width for each scaling interval
        while (remainingHealth > 0)
        {
            int healthInThisInterval = Mathf.Min(remainingHealth, scalingInterval);
            
            // Calculate width for this interval
            float intervalWidth = (healthInThisInterval / (float)scalingInterval) * baseBarWidth / currentScalingFactor;
            totalWidth += intervalWidth;
            
            remainingHealth -= healthInThisInterval;
            currentInterval++;
            currentScalingFactor *= 2f; // Double the scaling factor for next interval
        }
        
        return Mathf.Min(totalWidth, maxBarWidth);
    }
    
    /// <summary>
    /// Gets the scaling factor for a given health value
    /// </summary>
    public float GetScalingFactor(int health)
    {
        int interval = health / scalingInterval;
        return baseScalingFactor * Mathf.Pow(2f, interval);
    }
    
    /// <summary>
    /// Gets which scaling interval a health value falls into
    /// </summary>
    public int GetScalingInterval(int health)
    {
        return health / scalingInterval;
    }
    
    /// <summary>
    /// Updates only the current value (keeps the same max value)
    /// </summary>
    public void SetCurrentValue(int value)
    {
        SetValue(value, maxValue);
    }
    
    /// <summary>
    /// Updates only the maximum value (keeps the same current value)
    /// </summary>
    public void SetMaxValue(int max)
    {
        SetValue(currentValue, max);
    }
    
    /// <summary>
    /// Adds or subtracts from the current value
    /// </summary>
    public void ModifyValue(int amount)
    {
        SetCurrentValue(currentValue + amount);
    }
    
    private float GetCurrentFillAmount()
    {
        if (barFill == null) return 0f;
        
        switch (fillDirection)
        {
            case FillDirection.LeftToRight:
            case FillDirection.RightToLeft:
                return barFill.rectTransform.sizeDelta.x / originalFillSize.x;
            case FillDirection.TopToBottom:
            case FillDirection.BottomToTop:
                return barFill.rectTransform.sizeDelta.y / originalFillSize.y;
            default:
                return 0f;
        }
    }
    
    private void UpdateFillVisual(float fillAmount)
    {
        if (barFill == null) return;
        
        Vector2 newSize = barFill.rectTransform.sizeDelta;
        Vector2 newPosition = barFill.rectTransform.anchoredPosition;
        
        switch (fillDirection)
        {
            case FillDirection.LeftToRight:
                newSize.x = fillAmount * originalFillSize.x;
                barFill.rectTransform.sizeDelta = newSize;
                break;
                
            case FillDirection.RightToLeft:
                newSize.x = fillAmount * originalFillSize.x;
                newPosition.x = originalFillPosition.x - (originalFillSize.x - newSize.x) * 0.5f;
                barFill.rectTransform.sizeDelta = newSize;
                barFill.rectTransform.anchoredPosition = newPosition;
                break;
                
            case FillDirection.TopToBottom:
                newSize.y = fillAmount * originalFillSize.y;
                barFill.rectTransform.sizeDelta = newSize;
                break;
                
            case FillDirection.BottomToTop:
                newSize.y = fillAmount * originalFillSize.y;
                newPosition.y = originalFillPosition.y + (originalFillSize.y - newSize.y) * 0.5f;
                barFill.rectTransform.sizeDelta = newSize;
                barFill.rectTransform.anchoredPosition = newPosition;
                break;
        }
    }
    
    private void UpdateValueText()
    {
        if (useInternalText && valueText != null)
        {
            string newText = string.Format(valueFormat, currentValue, maxValue);
            valueText.text = newText;
            
            // Debug log to help troubleshoot text issues
    
        }
        else if (useInternalText && valueText == null)
        {
            Debug.LogWarning("BarUI: valueText is null - make sure to assign a TextMeshProUGUI component in the inspector");
        }
    }
    
    // Public getters
    public int GetCurrentValue() => currentValue;
    public int GetMaxValue() => maxValue;
    public float GetFillPercentage() => (float)currentValue / maxValue;
    
    /// <summary>
    /// Gets the current visual width of the bar
    /// </summary>
    public float GetBarWidth()
    {
        if (barContainer != null)
        {
            return barContainer.rectTransform.sizeDelta.x;
        }
        return 0f;
    }
    
    /// <summary>
    /// Gets the calculated width for a given health value
    /// </summary>
    public float GetCalculatedWidth(int health)
    {
        return CalculateBarWidth(health);
    }
}
