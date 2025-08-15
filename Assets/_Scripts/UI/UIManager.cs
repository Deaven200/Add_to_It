using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Central UI Manager that handles all UI elements across scenes.
/// Uses singleton pattern for persistence and scene management.
/// </summary>
public class UIManager : MonoBehaviour
{
    // Singleton pattern
    public static UIManager Instance { get; private set; }
    
    [Header("UI Panels")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject deathScreenPanel;
    [SerializeField] private GameObject settingsPanelPrefab;
    [SerializeField] private GameObject canvas;
    
    [Header("Health Bar UI")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image healthFillImage;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private Color mediumHealthColor = Color.yellow;
    
    [Header("Money UI")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private string currencyName = "Money";
    [SerializeField] private string moneyDisplayFormat = "{0}: {1}"; // {0} = currency name, {1} = amount
    
    [Header("Scene Management")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string playerRoomSceneName = "PlayerRoom";
    
    private GameObject _activeSettingsInstance;
    private bool isPaused = false;
    private bool isDeathScreenActive = false;
    
    // Player data persistence
    private int currentPlayerHealth = 100;
    private int maxPlayerHealth = 100;
    private int currentPlayerMoney = 0;
    
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
        // Hide all UI panels initially
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (deathScreenPanel != null) deathScreenPanel.SetActive(false);
        
        // Load saved player data
        LoadPlayerData();
    }
    
    // Called when a new scene is loaded
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-find UI references in the new scene
        FindUIReferencesInNewScene();
        
        // Reset UI state for new scene
        ResetUIState();
        
        // Update UI with current player data
        UpdateHealthUI();
        UpdateMoneyUI();
    }
    
    private void FindUIReferencesInNewScene()
    {
        // Try to find pause menu panel
        if (pauseMenuPanel == null)
        {
            pauseMenuPanel = GameObject.Find("PauseMenuPanel");
        }
        
        // Try to find death screen panel
        if (deathScreenPanel == null)
        {
            deathScreenPanel = GameObject.Find("DeathScreenPanel");
        }
        
        // Try to find canvas
        if (canvas == null)
        {
            canvas = GameObject.Find("Canvas");
        }
        
        // Try to find health bar components
        if (healthSlider == null)
        {
            healthSlider = GameObject.Find("HealthSlider")?.GetComponent<Slider>();
        }
        
        if (healthFillImage == null)
        {
            healthFillImage = GameObject.Find("HealthFill")?.GetComponent<Image>();
        }
        
        if (healthText == null)
        {
            healthText = GameObject.Find("HealthText")?.GetComponent<TextMeshProUGUI>();
        }
        
        // Try to find money text
        if (moneyText == null)
        {
            moneyText = GameObject.Find("MoneyText")?.GetComponent<TextMeshProUGUI>();
        }
    }
    
    private void ResetUIState()
    {
        // Hide all UI panels
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (deathScreenPanel != null) deathScreenPanel.SetActive(false);
        
        // Reset pause state
        isPaused = false;
        isDeathScreenActive = false;
        
        // Ensure game is running
        Time.timeScale = 1f;
    }
    
    void Update()
    {
        // Only handle pause input if death screen is not active
        if (!isDeathScreenActive && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    #region Player Data Management
    
    public void SetPlayerHealth(int currentHealth, int maxHealth)
    {
        currentPlayerHealth = currentHealth;
        maxPlayerHealth = maxHealth;
        SavePlayerData();
        UpdateHealthUI();
    }
    
    public void SetPlayerMoney(int money)
    {
        currentPlayerMoney = money;
        SavePlayerData();
        UpdateMoneyUI();
    }
    
    public void AddPlayerMoney(int amount)
    {
        currentPlayerMoney += amount;
        SavePlayerData();
        UpdateMoneyUI();
    }
    
    public int GetPlayerHealth() => currentPlayerHealth;
    public int GetMaxPlayerHealth() => maxPlayerHealth;
    public int GetPlayerMoney() => currentPlayerMoney;
    
    public void SetCurrencyName(string name)
    {
        currencyName = name;
        UpdateMoneyUI();
    }
    
    public void SetMoneyDisplayFormat(string format)
    {
        moneyDisplayFormat = format;
        UpdateMoneyUI();
    }
    
    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxPlayerHealth;
            healthSlider.value = currentPlayerHealth;
        }
        
        if (healthText != null)
        {
            healthText.text = $"{currentPlayerHealth} / {maxPlayerHealth}";
        }
        
        if (healthFillImage != null)
        {
            float healthPercentage = (float)currentPlayerHealth / maxPlayerHealth;
            
            if (healthPercentage <= 0.3f)
            {
                healthFillImage.color = lowHealthColor;
            }
            else if (healthPercentage <= 0.6f)
            {
                healthFillImage.color = mediumHealthColor;
            }
            else
            {
                healthFillImage.color = fullHealthColor;
            }
        }
    }
    
    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = string.Format(moneyDisplayFormat, currencyName, currentPlayerMoney);
        }
    }
    
    private void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerHealth", currentPlayerHealth);
        PlayerPrefs.SetInt("MaxPlayerHealth", maxPlayerHealth);
        PlayerPrefs.SetInt("PlayerMoney", currentPlayerMoney);
        PlayerPrefs.Save();
    }
    
    private void LoadPlayerData()
    {
        currentPlayerHealth = PlayerPrefs.GetInt("PlayerHealth", 100);
        maxPlayerHealth = PlayerPrefs.GetInt("MaxPlayerHealth", 100);
        currentPlayerMoney = PlayerPrefs.GetInt("PlayerMoney", 0);
        
    }
    
    [ContextMenu("Reset Player Data")]
    public void ResetPlayerData()
    {
        PlayerPrefs.DeleteKey("PlayerHealth");
        PlayerPrefs.DeleteKey("MaxPlayerHealth");
        PlayerPrefs.DeleteKey("PlayerMoney");
        PlayerPrefs.Save();
        
        currentPlayerHealth = 100;
        maxPlayerHealth = 100;
        currentPlayerMoney = 0;
        
        UpdateHealthUI();
        UpdateMoneyUI();
    }
    
    #endregion
    
    #region Pause Menu Functions
    
    public void PauseGame()
    {
        if (isDeathScreenActive) return; // Don't pause if death screen is active
        
        isPaused = true;
        Time.timeScale = 0f;
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("UIManager: Pause menu panel not found!");
        }
        
        // Refresh upgrade display if available
        SimpleUpgradeDisplay upgradeDisplay = FindObjectOfType<SimpleUpgradeDisplay>();
        if (upgradeDisplay != null)
        {
            upgradeDisplay.OnPauseMenuOpen();
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        // Refresh upgrade display if available
        SimpleUpgradeDisplay upgradeDisplay = FindObjectOfType<SimpleUpgradeDisplay>();
        if (upgradeDisplay != null)
        {
            upgradeDisplay.OnPauseMenuClose();
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        SceneManager.LoadScene(mainMenuSceneName);
    }
    
    public void OnSettingsButtonPressed()
    {
        if (_activeSettingsInstance == null && settingsPanelPrefab != null && canvas != null)
        {
            _activeSettingsInstance = Instantiate(settingsPanelPrefab, canvas.transform);
        }
    }
    
    #endregion
    
    #region Death Screen Functions
    
    public void ShowDeathScreen()
    {
        isDeathScreenActive = true;
        Time.timeScale = 0f;
        
        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(true);
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void OnRetryButtonPressed()
    {
        Time.timeScale = 1f;
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }
    
    public void OnQuitToMainMenuPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
    
    public void OnGoToPlayerRoomPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(playerRoomSceneName);
    }
    
    #endregion
    
    #region Utility Functions
    
    public bool IsPaused() => isPaused;
    public bool IsDeathScreenActive() => isDeathScreenActive;
    
    [ContextMenu("Test Pause Game")]
    public void TestPauseGame()
    {
        PauseGame();
    }
    
    [ContextMenu("Test Show Death Screen")]
    public void TestShowDeathScreen()
    {
        ShowDeathScreen();
    }
    
    [ContextMenu("Check UI Status")]
    public void CheckUIStatus()
    {
        Debug.Log("=== UI Status Check ===");
        Debug.Log($"Pause Menu Panel: {(pauseMenuPanel != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Death Screen Panel: {(deathScreenPanel != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Canvas: {(canvas != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Settings Panel Prefab: {(settingsPanelPrefab != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Health Slider: {(healthSlider != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Health Text: {(healthText != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Money Text: {(moneyText != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"Is Paused: {isPaused}");
        Debug.Log($"Is Death Screen Active: {isDeathScreenActive}");
        Debug.Log($"Time Scale: {Time.timeScale}");
        Debug.Log($"Player Health: {currentPlayerHealth}/{maxPlayerHealth}");
        Debug.Log($"Player Money: {currentPlayerMoney}");
        Debug.Log("=== Status Check Complete ===");
    }
    
    #endregion
} 