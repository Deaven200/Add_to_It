using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
    
    // Cached references for performance
    private UpgradeManager _cachedUpgradeManager;
    
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
        
        // Cache references for performance
        _cachedUpgradeManager = FindObjectOfType<UpgradeManager>();
        
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
        Debug.Log($"UIManager: Scene loaded - {scene.name}");
        
        // Ensure EventSystem exists first
        EnsureEventSystemExists();
        
        // Re-find UI references in the new scene
        FindUIReferencesInNewScene();
        
        // Reset UI state for new scene
        ResetUIState();
        
        // Update UI with current player data
        UpdateHealthUI();
        UpdateMoneyUI();
        
        // Verify pause menu setup
        VerifyPauseMenuSetup();
    }
    
    private void EnsureEventSystemExists()
    {
        if (EventSystem.current == null)
        {
            EventSystem existingEventSystem = FindObjectOfType<EventSystem>();
            if (existingEventSystem != null)
            {
                EventSystem.current = existingEventSystem;
                Debug.Log($"UIManager: Set EventSystem.current to {existingEventSystem.name}");
            }
            else
            {
                Debug.LogWarning("UIManager: No EventSystem found in scene! Creating one automatically...");
                
                // Create EventSystem automatically
                GameObject eventSystemGO = new GameObject("EventSystem");
                EventSystem newEventSystem = eventSystemGO.AddComponent<EventSystem>();
                eventSystemGO.AddComponent<StandaloneInputModule>();
                
                // Set it as current
                EventSystem.current = newEventSystem;
                
                Debug.Log($"UIManager: Created EventSystem: {eventSystemGO.name}");
            }
        }
    }
    
    public void VerifyPauseMenuSetup()
    {
        Debug.Log("=== UIManager: Verifying Pause Menu Setup ===");
        
        if (pauseMenuPanel == null)
        {
            Debug.LogError("❌ UIManager: PauseMenuPanel is null after scene load!");
            Debug.LogError("This means the pause menu won't work in this scene.");
        }
        else
        {
            Debug.Log($"✅ UIManager: PauseMenuPanel found: {pauseMenuPanel.name}");
            
            // Check if PauseManager is attached
            PauseManager pauseManager = pauseMenuPanel.GetComponent<PauseManager>();
            if (pauseManager == null)
            {
                Debug.LogError("❌ UIManager: PauseManager component not found on PauseMenuPanel!");
            }
            else
            {
                Debug.Log("✅ UIManager: PauseManager component found");
                
                // Force PauseManager to reconnect to UIManager
                pauseManager.SendMessage("ForceReconnectToUIManager", SendMessageOptions.DontRequireReceiver);
            }
        }
        
        Debug.Log("=== UIManager: Pause Menu Verification Complete ===");
    }
    
    public void FindUIReferencesInNewScene()
    {
        // Try to find pause menu panel - search more thoroughly
        if (pauseMenuPanel == null)
        {
            // First try to find by exact name
            pauseMenuPanel = GameObject.Find("PauseMenuPanel");
            
            // If not found, try to find any GameObject with PauseManager component
            if (pauseMenuPanel == null)
            {
                PauseManager pauseManager = FindObjectOfType<PauseManager>();
                if (pauseManager != null)
                {
                    pauseMenuPanel = pauseManager.gameObject;
                    Debug.Log("UIManager: Found PauseMenuPanel via PauseManager component");
                }
            }
            
            // If still not found, try to find by partial name match
            if (pauseMenuPanel == null)
            {
                GameObject[] allObjects = FindObjectsOfType<GameObject>();
                foreach (GameObject obj in allObjects)
                {
                    if (obj.name.ToLower().Contains("pause") && obj.name.ToLower().Contains("menu"))
                    {
                        pauseMenuPanel = obj;
                        Debug.Log($"UIManager: Found PauseMenuPanel via partial name match: {obj.name}");
                        break;
                    }
                }
            }
        }
        
        // Try to find death screen panel
        if (deathScreenPanel == null)
        {
            deathScreenPanel = GameObject.Find("DeathScreenPanel");
            
            // If not found, try to find by partial name match
            if (deathScreenPanel == null)
            {
                GameObject[] allObjects = FindObjectsOfType<GameObject>();
                foreach (GameObject obj in allObjects)
                {
                    if (obj.name.ToLower().Contains("death") && obj.name.ToLower().Contains("screen"))
                    {
                        deathScreenPanel = obj;
                        Debug.Log($"UIManager: Found DeathScreenPanel via partial name match: {obj.name}");
                        break;
                    }
                }
            }
        }
        
        // Try to find canvas
        if (canvas == null)
        {
            canvas = GameObject.Find("Canvas");
            
            // If not found, try to find any Canvas in the scene
            if (canvas == null)
            {
                Canvas[] canvases = FindObjectsOfType<Canvas>();
                if (canvases.Length > 0)
                {
                    canvas = canvases[0].gameObject;
                    Debug.Log($"UIManager: Found Canvas: {canvas.name}");
                }
            }
        }
        
        // Try to find health bar components
        if (healthSlider == null)
        {
            healthSlider = GameObject.Find("HealthSlider")?.GetComponent<Slider>();
            
            // If not found, try to find any Slider with health-related name
            if (healthSlider == null)
            {
                Slider[] sliders = FindObjectsOfType<Slider>();
                foreach (Slider slider in sliders)
                {
                    if (slider.name.ToLower().Contains("health"))
                    {
                        healthSlider = slider;
                        Debug.Log($"UIManager: Found HealthSlider: {slider.name}");
                        break;
                    }
                }
            }
        }
        
        if (healthFillImage == null)
        {
            healthFillImage = GameObject.Find("HealthFill")?.GetComponent<Image>();
            
            // If not found, try to find any Image with health-related name
            if (healthFillImage == null)
            {
                Image[] images = FindObjectsOfType<Image>();
                foreach (Image image in images)
                {
                    if (image.name.ToLower().Contains("health") && image.name.ToLower().Contains("fill"))
                    {
                        healthFillImage = image;
                        Debug.Log($"UIManager: Found HealthFill: {image.name}");
                        break;
                    }
                }
            }
        }
        
        if (healthText == null)
        {
            healthText = GameObject.Find("HealthText")?.GetComponent<TextMeshProUGUI>();
            
            // If not found, try to find any TextMeshProUGUI with health-related name
            if (healthText == null)
            {
                TextMeshProUGUI[] texts = FindObjectsOfType<TextMeshProUGUI>();
                foreach (TextMeshProUGUI text in texts)
                {
                    if (text.name.ToLower().Contains("health"))
                    {
                        healthText = text;
                        Debug.Log($"UIManager: Found HealthText: {text.name}");
                        break;
                    }
                }
            }
        }
        
        // Try to find money text
        if (moneyText == null)
        {
            moneyText = GameObject.Find("MoneyText")?.GetComponent<TextMeshProUGUI>();
            
            // If not found, try to find any TextMeshProUGUI with money-related name
            if (moneyText == null)
            {
                TextMeshProUGUI[] texts = FindObjectsOfType<TextMeshProUGUI>();
                foreach (TextMeshProUGUI text in texts)
                {
                    if (text.name.ToLower().Contains("money") || text.name.ToLower().Contains("currency"))
                    {
                        moneyText = text;
                        Debug.Log($"UIManager: Found MoneyText: {text.name}");
                        break;
                    }
                }
            }
        }
        
        // Log what we found for debugging
        Debug.Log($"UIManager: Scene loaded - Found UI elements: PauseMenuPanel={pauseMenuPanel != null}, DeathScreenPanel={deathScreenPanel != null}, Canvas={canvas != null}");
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
        // Only handle pause input if death screen is not active and upgrade menu is not active
        if (!isDeathScreenActive && Input.GetKeyDown(KeyCode.Escape))
        {
            // Check if upgrade menu is active (use cached reference for performance)
            bool upgradeMenuActive = _cachedUpgradeManager != null && _cachedUpgradeManager.IsUpgradeMenuActive();
            
            if (!upgradeMenuActive)
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