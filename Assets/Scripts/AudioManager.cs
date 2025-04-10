using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
    }

    [Header("Sound Effects")]
    [SerializeField] private SoundEffect[] soundEffects;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Settings")]
    [SerializeField] private float fadeSpeed = 1f;
    private bool isMuted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudio()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        // Load mute state
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        UpdateMuteState();
    }

    public void PlaySound(string soundName)
    {
        if (isMuted) return;

        SoundEffect sound = System.Array.Find(soundEffects, s => s.name == soundName);
        if (sound != null && sfxSource != null)
        {
            sfxSource.pitch = sound.pitch;
            sfxSource.PlayOneShot(sound.clip, sound.volume);
        }
        else
        {
            Debug.LogWarning($"Sound {soundName} not found!");
        }
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicSource != null && musicSource.clip != musicClip)
        {
            musicSource.clip = musicClip;
            if (!isMuted)
            {
                musicSource.Play();
            }
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
        UpdateMuteState();
    }

    private void UpdateMuteState()
    {
        if (musicSource != null)
        {
            musicSource.mute = isMuted;
        }
        if (sfxSource != null)
        {
            sfxSource.mute = isMuted;
        }
    }

    public void PlayJumpSound()
    {
        PlaySound("Jump");
    }

    public void PlayScoreSound()
    {
        PlaySound("Score");
    }

    public void PlayHitSound()
    {
        PlaySound("Hit");
    }

    public void PlayGameOverSound()
    {
        PlaySound("GameOver");
    }

    public void PlayButtonClickSound()
    {
        PlaySound("ButtonClick");
    }
}
