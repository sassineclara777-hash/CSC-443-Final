using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Auto Play")]
    [SerializeField] private bool playMainMenuMusicOnStart;
    [SerializeField] private bool playGameplayMusicOnStart;

    [Header("Background Music")]
    public AudioClip mainMenuMusic;
    public AudioClip gameplayMusic;

    [Header("Sound Effects")]
    public AudioClip jumpSound;
    public AudioClip coinPickupSound;
    public AudioClip gameOverSound;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        Instance = this;

        musicSource = GetComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.spatialBlend = 0f;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;
        sfxSource.spatialBlend = 0f;
    }

    private void Start()
    {
        if (playMainMenuMusicOnStart)
        {
            PlayMainMenuMusic();
        }
        else if (playGameplayMusicOnStart)
        {
            PlayGameplayMusic();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void PlayMainMenuMusic()
    {
        PlayBackgroundMusic(mainMenuMusic);
    }

    public void PlayGameplayMusic()
    {
        PlayBackgroundMusic(gameplayMusic);
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void JumpSound()
    {
        PlaySound(jumpSound);
    }

    public void CoinPickupSound()
    {
        PlaySound(coinPickupSound);
    }

    public void GameOverSound()
    {
        StopBackgroundMusic();
        PlaySound(gameOverSound);
    }

    private void PlayBackgroundMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}