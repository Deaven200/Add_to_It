using UnityEngine;

/// <summary>
/// A persistent singleton that manages saving, loading, and applying game settings.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance
    {
        get; private set;
    }

    // PlayerPrefs Keys
    private const string VolumeKey = "MasterVolume";
    private const string WindowModeKey = "WindowMode";

    public float CurrentVolume
    {
        get; private set;
    }
    public FullScreenMode CurrentWindowMode
    {
        get; private set;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
    }

    public void SetVolume(float volume)
    {
        CurrentVolume = volume;
        AudioListener.volume = CurrentVolume;
        PlayerPrefs.SetFloat(VolumeKey, CurrentVolume);
        PlayerPrefs.Save();
    }

    public void SetWindowMode(int modeIndex)
    {
        // 0: Fullscreen, 1: Windowed, 2: Borderless
        FullScreenMode mode = FullScreenMode.ExclusiveFullScreen;
        if (modeIndex == 1)
            mode = FullScreenMode.Windowed;
        else if (modeIndex == 2)
            mode = FullScreenMode.FullScreenWindow;

        CurrentWindowMode = mode;
        Screen.fullScreenMode = CurrentWindowMode;
        PlayerPrefs.SetInt(WindowModeKey, modeIndex);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        // Load Volume, default to 1f (max volume)
        CurrentVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        AudioListener.volume = CurrentVolume;

        // Load Window Mode, default to 0 (Fullscreen)
        int windowModeIndex = PlayerPrefs.GetInt(WindowModeKey, 0);
        SetWindowMode(windowModeIndex); // Use the method to apply it
    }
}