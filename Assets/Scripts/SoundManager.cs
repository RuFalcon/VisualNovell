using GamePush;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    // --- Источники звука ---
    [SerializeField] private AudioSource _musicSource; // Для фоновой музыки и эмбиенса
    [SerializeField] private AudioSource _sfxSource;   // Для звуковых эффектов

    // --- Клипы для SFX ---
    [SerializeField] private AudioClip _backgroundMusic;
    [SerializeField] private AudioClip _typingSound;
    [SerializeField] private AudioClip _buttonClick;

    void Awake()
    {
        // Классическая реализация синглтона
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        await GP_Init.Ready;
        GameManager.soundOff = GP_Player.GetBool("middleages_soundoff");
        if (!GameManager.soundOff) MusicPlay();
    }


    public void StartTypingSound()
    {
        if (_typingSound == null) return;
        _sfxSource.clip = _typingSound;
        _sfxSource.loop = true;
        _sfxSource.Play();
    }

    public void StopTypingSound()
    {
        if (_sfxSource.clip == _typingSound)
        {
            _sfxSource.Stop();
            _sfxSource.loop = false;
        }
    }

    // --- Методы для фоновой музыки/эмбиенса ---
    public void PlayMusic(AudioClip musicClip)
    {
        // Если музыка уже играет и это тот же самый трек, ничего не делаем
        if (_musicSource.isPlaying && _musicSource.clip == musicClip)
        {
            return;
        }

        _musicSource.clip = musicClip;
        _musicSource.loop = true;
        _musicSource.Play();
    }

    public void MusicPlay()
    {
        // Если музыка уже играет и это тот же самый трек, ничего не делаем
        if (_musicSource.isPlaying && _musicSource.clip == _backgroundMusic)
        {
            return;
        }

        _musicSource.clip = _backgroundMusic;
        _musicSource.loop = true;
        _musicSource.Play();
    }

    public void StopMusic()
    {
        _musicSource.Pause();
    }

    public void PlayButtonClickSound()
    {
        _sfxSource.PlayOneShot(_buttonClick);
    }

    public void PlaySFX(AudioClip sfxClip)
    {
        if (sfxClip != null)
        {
            _sfxSource.PlayOneShot(sfxClip);
        }
    }
}
