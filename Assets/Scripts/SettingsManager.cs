using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    // Game Settings
    [Header("Difficulty Settings")]
    [SerializeField] private float easySpeed = 2.5f;
    [SerializeField] private float mediumSpeed = 3.5f;
    [SerializeField] private float hardSpeed = 4.5f;
    
    [Header("Audio Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.7f;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    // PlayerPrefs Keys
    private const string DIFFICULTY_KEY = "GameDifficulty";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    private Difficulty currentDifficulty;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadSettings()
    {
        // Load difficulty
        currentDifficulty = (Difficulty)PlayerPrefs.GetInt(DIFFICULTY_KEY, (int)Difficulty.Medium);

        // Load volume settings
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.7f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);

        // Apply loaded settings
        ApplySettings();
    }

    private void ApplySettings()
    {
        // Apply volume settings if AudioManager exists
        if (AudioManager.Instance != null)
        {
            // Assuming AudioManager has methods to set volume
            // AudioManager.Instance.SetMusicVolume(musicVolume);
            // AudioManager.Instance.SetSFXVolume(sfxVolume);
        }
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        PlayerPrefs.SetInt(DIFFICULTY_KEY, (int)difficulty);
        PlayerPrefs.Save();

        // Update game speed based on difficulty
        if (PipeSpawner.Instance != null)
        {
            float newSpeed = GetSpeedForDifficulty(difficulty);
            // Assuming PipeSpawner has a method to set speed
            // PipeSpawner.Instance.SetSpeed(newSpeed);
        }
    }

    public float GetSpeedForDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                return easySpeed;
            case Difficulty.Medium:
                return mediumSpeed;
            case Difficulty.Hard:
                return hardSpeed;
            default:
                return mediumSpeed;
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.Save();
        ApplySettings();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
        PlayerPrefs.Save();
        ApplySettings();
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    public Difficulty GetCurrentDifficulty()
    {
        return currentDifficulty;
    }

    public void ResetToDefaults()
    {
        SetDifficulty(Difficulty.Medium);
        SetMusicVolume(0.7f);
        SetSFXVolume(1f);
    }
}
