using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    #region Singleton

    public static GameHandler instance { get; private set; }

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

    void Awake() => Singleton();

    [SerializeField] FadeUI _mainCanvas;
    [SerializeField] FadeUI _canvas;
    [SerializeField] TMP_Text _resultTitle;

    [SerializeField] AudioClip WinClip;
    [SerializeField] AudioClip LoseClip;

    AudioSource _audio;
    
    public bool IsGameOver { get; private set; }

    void Start()
    {
        SoundManager.instance.PlayLevelMusic();
        _audio = GetComponent<AudioSource>();
    }

    public void WinGame() => DisplayResult(true);

    public void LoseGame() => DisplayResult(false);

    void DisplayResult(bool isWin)
    {
        _mainCanvas.HideUI();
        IsGameOver = isWin;
        _canvas.ShowUI();
        _resultTitle.text = isWin ? "GAME WON" : "GAME LOST";
        
        var clip = isWin ? WinClip : LoseClip;
        _audio.PlayOneShot(clip, 0.5f);
    }
}
