using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the UI elements on the Settings Panel.
/// </summary>
public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Dropdown windowModeDropdown;

    private void OnEnable()
    {
        // When the panel is shown, update the UI to reflect current settings.
        volumeSlider.value = SettingsManager.Instance.CurrentVolume;

        // Convert FullScreenMode enum to dropdown index
        FullScreenMode currentMode = SettingsManager.Instance.CurrentWindowMode;
        if (currentMode == FullScreenMode.Windowed)
            windowModeDropdown.value = 1;
        else if (currentMode == FullScreenMode.FullScreenWindow)
            windowModeDropdown.value = 2;
        else
            windowModeDropdown.value = 0; // Default to Fullscreen
    }

    public void OnVolumeChanged(float value)
    {
        SettingsManager.Instance.SetVolume(value);
    }

    public void OnWindowModeChanged(int value)
    {
        SettingsManager.Instance.SetWindowMode(value);
    }

    public void OnBackButtonPressed()
    {
        // Destroy the panel instead of just hiding it.
        // This will ensure the reference in the menu script becomes null.
        Destroy(gameObject);
    }
}