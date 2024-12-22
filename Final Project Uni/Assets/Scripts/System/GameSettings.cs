using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    [Header("Audio Settings")] 
    public float musicVolume = 50;
    public float sfxVolume = 50;
    public bool isMusicEnabled = true;
    public bool isSFXEnabled = true;

    [Header("Graphics Settings")] public bool isFullScreen = true;
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;
    public int graphicsQuality = 2;
    public bool isVSyncEnabled = true;
    public bool isAntiAliasingEnabled = true;
    public bool isBloomEnabled = true;
    public bool isShadowsEnabled = true;


    private void Start()
    {
        if (Instance == null) Instance = this;
        LoadSettings();
    }

    public void LoadSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 50);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 50);
        isMusicEnabled = PlayerPrefs.GetInt("IsMusicEnabled", 1) == 1;
        isSFXEnabled = PlayerPrefs.GetInt("IsSFXEnabled", 1) == 1;

        isFullScreen = PlayerPrefs.GetInt("IsFullScreen", 1) == 1;
        resolutionWidth = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        resolutionHeight = PlayerPrefs.GetInt("ResolutionHeight", 1080);
        graphicsQuality = PlayerPrefs.GetInt("GraphicsQuality", 2);
        isVSyncEnabled = PlayerPrefs.GetInt("IsVSyncEnabled", 1) == 1;
        isAntiAliasingEnabled = PlayerPrefs.GetInt("IsAntiAliasingEnabled", 1) == 1;
        isBloomEnabled = PlayerPrefs.GetInt("IsBloomEnabled", 1) == 1;

        ApplySettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("IsMusicEnabled", isMusicEnabled ? 1 : 0);
        PlayerPrefs.SetInt("IsSFXEnabled", isSFXEnabled ? 1 : 0);

        PlayerPrefs.SetInt("IsFullScreen", isFullScreen ? 1 : 0);
        PlayerPrefs.SetInt("ResolutionWidth", resolutionWidth);
        PlayerPrefs.SetInt("ResolutionHeight", resolutionHeight);
        PlayerPrefs.SetInt("GraphicsQuality", graphicsQuality);
        PlayerPrefs.SetInt("IsVSyncEnabled", isVSyncEnabled ? 1 : 0);
        PlayerPrefs.SetInt("IsAntiAliasingEnabled", isAntiAliasingEnabled ? 1 : 0);
        PlayerPrefs.SetInt("IsBloomEnabled", isBloomEnabled ? 1 : 0);


        PlayerPrefs.Save();
        ApplySettings();
    }


    private void ApplySettings()
    {
        // Apply audio settings
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMusicVolume(musicVolume);
            SoundManager.Instance.SetSFXVolume(sfxVolume);
            SoundManager.Instance.ToggleMusic();
            SoundManager.Instance.ToggleSFX();
        }

        // Apply graphics settings
        Screen.fullScreen = isFullScreen;
        Screen.SetResolution(resolutionWidth, resolutionHeight, isFullScreen);
        QualitySettings.SetQualityLevel(graphicsQuality);
        QualitySettings.vSyncCount = isVSyncEnabled ? 1 : 0;
        QualitySettings.antiAliasing = isAntiAliasingEnabled ? 8 : 0;
        QualitySettings.shadows = isShadowsEnabled ? ShadowQuality.All : ShadowQuality.Disable;
        //QualitySettings.bloom = isBloomEnabled ? 1 : 0;
    }
}