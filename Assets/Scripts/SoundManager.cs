using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    #region Singleton

    public static SoundManager instance { get; private set; }

    private void Singleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    #endregion

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource soundSource;

    [SerializeField] AudioClip defaultMusic;

    float _musicMaxVolume;
    float _setVolume;

    float _duration = 3;

    public float MaxVolume => _musicMaxVolume;

    public float musicVol => musicSource.volume;
    public float soundVol => soundSource.volume;

    public List<AudioClip> randomBackgroundSounds = new();

    public void SetMusicVolume(float x)
    {
        _setVolume = x;
        musicSource.volume = x;
    }

    public void SetSoundVolume(float x)
    {
        _setVolume = x;
        soundSource.volume = x;
    }

    void Awake()
    {
        Singleton();

        musicSource.loop = true;
        _musicMaxVolume = musicSource.volume;
        musicSource.volume = 0;

        // Set Volume = 60%
        _setVolume = _musicMaxVolume * .60f;
    }

    void Start()
    {
        if (defaultMusic == null) return;
        PlayMusic(defaultMusic);

        InvokeRepeating(nameof(PlayRandomBackgroundSound), 15, 15);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySound(AudioClip clip)
    {
        soundSource.clip = clip;
        soundSource.Play();
    }

    public void PlaySound(AudioClip clip, float skipToTime)
    {
        soundSource.time = skipToTime;
        soundSource.clip = clip;
        soundSource.Play();
    }

    void PlayRandomBackgroundSound()
    {
        if (randomBackgroundSounds.Count == 0) return;
        PlaySound(randomBackgroundSounds.SelectRandom());
    }

    public void PlayDefaultMusic() => PlayMusic(defaultMusic);
}
