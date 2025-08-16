using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Script to check if UI is properly set up for interactions
/// </summary>
public class UISetupChecker : MonoBehaviour
{
    [Header("Check Settings")]
    [SerializeField] private bool checkOnStart = true;
    [SerializeField] private bool checkOnDemand = false;
    
    void Start()
    {
        if (checkOnStart)
        {
            CheckUISetup();
        }
    }
    
    void Update()
    {
        if (checkOnDemand && Input.GetKeyDown(KeyCode.F1))
        {
            CheckUISetup();
        }
    }
    
    [ContextMenu("Check UI Setup")]
    public void CheckUISetup()
    {
        Debug.Log("=== UI SETUP CHECKER ===");
        
        // Check EventSystem
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem != null)
        {
            Debug.Log($"✓ EventSystem found: {eventSystem.name}");
            Debug.Log($"  - Enabled: {eventSystem.enabled}");
            Debug.Log($"  - Current: {eventSystem == EventSystem.current}");
        }
        else
        {
            Debug.LogError("✗ No EventSystem found in scene! This is required for UI interactions.");
        }
        
        // Check Canvases
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        Debug.Log($"Found {canvases.Length} canvas(es) in scene:");
        
        foreach (Canvas canvas in canvases)
        {
            Debug.Log($"Canvas: {canvas.name}");
            Debug.Log($"  - Enabled: {canvas.enabled}");
            Debug.Log($"  - RenderMode: {canvas.renderMode}");
            Debug.Log($"  - SortOrder: {canvas.sortingOrder}");
            
            // Check GraphicRaycaster
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster != null)
            {
                Debug.Log($"  ✓ GraphicRaycaster found - Enabled: {raycaster.enabled}");
            }
            else
            {
                Debug.LogError($"  ✗ Missing GraphicRaycaster on canvas {canvas.name}!");
            }
            
            // Check CanvasScaler
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                Debug.Log($"  ✓ CanvasScaler found - UI Scale Mode: {scaler.uiScaleMode}");
            }
            else
            {
                Debug.LogWarning($"  ⚠ No CanvasScaler on canvas {canvas.name}");
            }
        }
        
        // Check Buttons
        Button[] buttons = FindObjectsOfType<Button>();
        Debug.Log($"Found {buttons.Length} button(s) in scene:");
        
        foreach (Button button in buttons)
        {
            Debug.Log($"Button: {button.name}");
            Debug.Log($"  - Enabled: {button.enabled}");
            Debug.Log($"  - Interactable: {button.interactable}");
            Debug.Log($"  - OnClick listeners: {button.onClick.GetPersistentEventCount()}");
            
            // Check if button has Image component
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                Debug.Log($"  ✓ Image component found - Enabled: {buttonImage.enabled}");
            }
            else
            {
                Debug.LogWarning($"  ⚠ No Image component on button {button.name}");
            }
        }
        
        // Check Input System
        Debug.Log("Input System Check:");
        Debug.Log($"  - Mouse position: {Input.mousePosition}");
        Debug.Log($"  - Cursor visible: {Cursor.visible}");
        Debug.Log($"  - Cursor lock state: {Cursor.lockState}");
        
        Debug.Log("=== END UI SETUP CHECK ===");
    }
    
    [ContextMenu("Fix Common UI Issues")]
    public void FixCommonUIIssues()
    {
        Debug.Log("=== FIXING COMMON UI ISSUES ===");
        
        // Ensure EventSystem exists
        if (EventSystem.current == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
            Debug.Log("✓ Created missing EventSystem");
        }
        
        // Check and fix canvases
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            // Add GraphicRaycaster if missing
            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
                Debug.Log($"✓ Added GraphicRaycaster to canvas {canvas.name}");
            }
            
            // Add CanvasScaler if missing
            if (canvas.GetComponent<CanvasScaler>() == null)
            {
                CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                Debug.Log($"✓ Added CanvasScaler to canvas {canvas.name}");
            }
        }
        
        Debug.Log("=== END FIXING UI ISSUES ===");
    }
}
