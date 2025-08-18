using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI component to display FPS optimization status and provide controls
/// </summary>
public class FPSOptimizationUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI thresholdText;
    [SerializeField] private Slider fpsSlider;
    [SerializeField] private Toggle optimizationToggle;
    [SerializeField] private Button emergencyButton;
    [SerializeField] private Button resumeButton;
    
    [Header("Display Settings")]
    [SerializeField] private bool showUI = true;
    [SerializeField] private Color goodFPSColor = Color.green;
    [SerializeField] private Color warningFPSColor = Color.yellow;
    [SerializeField] private Color criticalFPSColor = Color.red;
    
    private FPSOptimizer fpsOptimizer;
    private ProceduralLevelManager levelManager;
    
    void Start()
    {
        FindReferences();
        SetupUI();
        SubscribeToEvents();
    }
    
    void FindReferences()
    {
        fpsOptimizer = FindObjectOfType<FPSOptimizer>();
        levelManager = FindObjectOfType<ProceduralLevelManager>();
        
        if (fpsOptimizer == null)
        {
            Debug.LogWarning("FPSOptimizationUI: No FPSOptimizer found!");
        }
    }
    
    void SetupUI()
    {
        if (fpsOptimizer != null)
        {
            // Setup toggle
            if (optimizationToggle != null)
            {
                optimizationToggle.isOn = fpsOptimizer.IsOptimizationEnabled;
                optimizationToggle.onValueChanged.AddListener(OnOptimizationToggled);
            }
            
            // Setup buttons
            if (emergencyButton != null)
            {
                emergencyButton.onClick.AddListener(OnEmergencyButtonClicked);
            }
            
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(OnResumeButtonClicked);
            }
            
            // Setup slider
            if (fpsSlider != null)
            {
                fpsSlider.minValue = 15f;
                fpsSlider.maxValue = 60f;
                fpsSlider.value = fpsOptimizer.CurrentFPS;
                fpsSlider.onValueChanged.AddListener(OnFPSThresholdChanged);
            }
        }
    }
    
    void SubscribeToEvents()
    {
        if (fpsOptimizer != null)
        {
            fpsOptimizer.OnFPSChanged += OnFPSChanged;
            fpsOptimizer.OnGenerationPausedChanged += OnGenerationPausedChanged;
            fpsOptimizer.OnEmergencyModeChanged += OnEmergencyModeChanged;
        }
    }
    
    void Update()
    {
        if (!showUI) return;
        
        UpdateUI();
    }
    
    void UpdateUI()
    {
        if (fpsOptimizer == null) return;
        
        // Update FPS text
        if (fpsText != null)
        {
            float fps = fpsOptimizer.CurrentFPS;
            fpsText.text = $"FPS: {fps:F1}";
            
            // Color based on FPS
            if (fps >= 50f)
            {
                fpsText.color = goodFPSColor;
            }
            else if (fps >= 30f)
            {
                fpsText.color = warningFPSColor;
            }
            else
            {
                fpsText.color = criticalFPSColor;
            }
        }
        
        // Update status text
        if (statusText != null)
        {
            string status = "Normal";
            if (fpsOptimizer.IsEmergencyMode)
            {
                status = "EMERGENCY MODE";
            }
            else if (fpsOptimizer.IsGenerationPaused)
            {
                status = "Generation Paused";
            }
            
            statusText.text = $"Status: {status}";
        }
        
        // Update threshold text
        if (thresholdText != null)
        {
            thresholdText.text = $"Thresholds: {fpsOptimizer.CurrentFPS:F1}";
        }
        
        // Update button states
        if (emergencyButton != null)
        {
            emergencyButton.interactable = !fpsOptimizer.IsEmergencyMode;
        }
        
        if (resumeButton != null)
        {
            resumeButton.interactable = fpsOptimizer.IsGenerationPaused || fpsOptimizer.IsEmergencyMode;
        }
    }
    
    void OnFPSChanged(float fps)
    {
        if (fpsSlider != null)
        {
            fpsSlider.value = fps;
        }
    }
    
    void OnGenerationPausedChanged(bool paused)
    {
        UpdateUI();
    }
    
    void OnEmergencyModeChanged(bool emergencyMode)
    {
        UpdateUI();
    }
    
    void OnOptimizationToggled(bool enabled)
    {
        if (fpsOptimizer != null)
        {
            fpsOptimizer.SetOptimizationEnabled(enabled);
        }
    }
    
    void OnEmergencyButtonClicked()
    {
        if (fpsOptimizer != null)
        {
            fpsOptimizer.ForceEmergencyMode();
        }
    }
    
    void OnResumeButtonClicked()
    {
        if (fpsOptimizer != null)
        {
            fpsOptimizer.ForceResumeGeneration();
        }
    }
    
    void OnFPSThresholdChanged(float value)
    {
        // This could be used to adjust thresholds dynamically
        // For now, just update the display
    }
    
    void OnDestroy()
    {
        if (fpsOptimizer != null)
        {
            fpsOptimizer.OnFPSChanged -= OnFPSChanged;
            fpsOptimizer.OnGenerationPausedChanged -= OnGenerationPausedChanged;
            fpsOptimizer.OnEmergencyModeChanged -= OnEmergencyModeChanged;
        }
    }
    
    // Public methods for external control
    public void SetUIVisibility(bool visible)
    {
        showUI = visible;
        gameObject.SetActive(visible);
    }
    
    public void ToggleUI()
    {
        showUI = !showUI;
        gameObject.SetActive(showUI);
    }
}

