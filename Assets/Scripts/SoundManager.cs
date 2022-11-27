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

    [SerializeField] AudioSource _musicSource;
    [SerializeField] AudioSource _soundSource;

    [SerializeField] AudioClip _defaultMusic;

    float _musicMaxVolume;
    float _setVolume;

    float _duration = 3;

    public float MaxVolume => _musicMaxVolume;

    public float musicVol => _musicSource.volume;
    public float soundVol => _soundSource.volume;

    public void SetMusicVolume(float x)
    {
        _setVolume = x;
        _musicSource.volume = x;
    }

    public void SetSoundVolume(float x)
    {
        _setVolume = x;
        _soundSource.volume = x;
    }

    void Awake()
    {
        Singleton();

        _musicSource.loop = true;
        _musicMaxVolume = _musicSource.volume;
        _musicSource.volume = 0;

        // Set Volume = 60%
        _setVolume = _musicMaxVolume * .60f;
    }

    void Start()
    {
        if (_defaultMusic == null) return;
        PlayMusic(_defaultMusic);
    }

    public async void PlayMusic(AudioClip clip)
    {
        _musicSource.clip = clip;
        
        if (_musicSource.isPlaying)
        {
            await _musicSource.DOFade(0, _duration).AsyncWaitForCompletion();
        }
        
        _musicSource.Play();
        await _musicSource.DOFade(_setVolume, _duration).AsyncWaitForCompletion();
    }

    public void PlaySound(AudioClip clip)
    {
        _soundSource.clip = clip;
        _soundSource.Play();
    }

    public void PlaySound(AudioClip clip, float skipToTime)
    {
        _soundSource.time = skipToTime;
        _soundSource.clip = clip;
        _soundSource.Play();
    }

    public void PlayDefaultMusic() => PlayMusic(_defaultMusic);
}
