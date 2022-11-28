using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseHandler : MonoBehaviour
{
    #region Singleton

    public static PauseHandler instance { get; private set; }

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
    
    public UnityEvent<bool> OnPauseEvent = new UnityEvent<bool>();
    public bool IsPaused { get; private set; }

    void Awake()
    {
        Singleton();
        OnPauseEvent.AddListener(SetPauseState);
    }

    public void SetPauseState(bool state)
    {
        IsPaused = state;
    }
}
