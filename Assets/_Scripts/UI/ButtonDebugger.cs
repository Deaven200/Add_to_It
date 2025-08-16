using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Debug script to help identify UI button issues
/// </summary>
public class ButtonDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugging = false; // Set to false to reduce console spam
    [SerializeField] private bool logAllButtonClicks = false; // Set to false to reduce console spam
    
    void Start()
    {
        if (enableDebugging)
        {
            // Find all buttons in the scene and add debug listeners
            Button[] allButtons = FindObjectsOfType<Button>();
            Debug.Log($"ButtonDebugger: Found {allButtons.Length} buttons in scene");
            
            foreach (Button button in allButtons)
            {
                button.onClick.AddListener(() => OnButtonClicked(button));
            }
        }
    }
    
    void OnButtonClicked(Button button)
    {
        if (logAllButtonClicks)
        {
            Debug.Log($"ButtonDebugger: Button clicked! Button: {button.name}, GameObject: {button.gameObject.name}");
        }
    }
    
    void Update()
    {
        if (enableDebugging && Input.GetMouseButtonDown(0))
        {
            Debug.Log($"ButtonDebugger: Mouse click detected at position: {Input.mousePosition}");
            
            // Check if EventSystem exists
            if (EventSystem.current == null)
            {
                Debug.LogError("ButtonDebugger: No EventSystem found in scene! This is required for UI interactions.");
                return;
            }
            
            // Check if we're clicking on a UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("ButtonDebugger: Mouse click detected on UI element");
                
                // Try to find what we clicked on
                PointerEventData eventData = new PointerEventData(EventSystem.current);
                eventData.position = Input.mousePosition;
                
                System.Collections.Generic.List<RaycastResult> results = new System.Collections.Generic.List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);
                
                if (results.Count == 0)
                {
                    Debug.LogWarning("ButtonDebugger: No UI elements found at click position!");
                }
                
                foreach (RaycastResult result in results)
                {
                    Debug.Log($"ButtonDebugger: Clicked on: {result.gameObject.name} (Layer: {LayerMask.LayerToName(result.gameObject.layer)})");
                    
                    Button clickedButton = result.gameObject.GetComponent<Button>();
                    if (clickedButton != null)
                    {
                        Debug.Log($"ButtonDebugger: Found Button component on clicked object: {result.gameObject.name}");
                        Debug.Log($"ButtonDebugger: Button interactable: {clickedButton.interactable}");
                        Debug.Log($"ButtonDebugger: Button onClick listener count: {clickedButton.onClick.GetPersistentEventCount()}");
                    }
                    else
                    {
                        Debug.Log($"ButtonDebugger: No Button component found on {result.gameObject.name}");
                    }
                }
            }
            else
            {
                Debug.Log("ButtonDebugger: Mouse click detected but not on UI element");
                Debug.Log($"ButtonDebugger: EventSystem.current.IsPointerOverGameObject() returned false");
                
                // Additional debugging for UI setup
                Canvas[] canvases = FindObjectsOfType<Canvas>();
                Debug.Log($"ButtonDebugger: Found {canvases.Length} canvases in scene");
                
                foreach (Canvas canvas in canvases)
                {
                    Debug.Log($"ButtonDebugger: Canvas '{canvas.name}' - RenderMode: {canvas.renderMode}, Enabled: {canvas.enabled}");
                    
                    // Check if canvas has GraphicRaycaster
                    GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
                    if (raycaster != null)
                    {
                        Debug.Log($"ButtonDebugger: Canvas '{canvas.name}' has GraphicRaycaster - Enabled: {raycaster.enabled}");
                    }
                    else
                    {
                        Debug.LogError($"ButtonDebugger: Canvas '{canvas.name}' is missing GraphicRaycaster component!");
                    }
                }
            }
        }
    }
}
