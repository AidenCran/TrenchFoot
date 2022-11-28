using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    #region Singleton

    public static PauseMenu instance { get; private set; }

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
    }

    #endregion
    
    [SerializeField] FadeUI menuFade;

    public UnityEvent<bool> OnPauseEvent = new UnityEvent<bool>();
    
    public bool IsPaused { get; private set; }

    void Awake()
    {
        Singleton();
    }

    public void Pause()
    {
        IsPaused = true;
        OnPauseEvent?.Invoke(IsPaused);
        menuFade.ShowUI();
    }

    public void Resume()
    {
        IsPaused = false;
        OnPauseEvent?.Invoke(IsPaused);
        menuFade.HideUI();
    }

    public void MainMenu(int sceneID)
    {
        Resume();
        SceneManager.LoadScene(sceneID);
    }
}
