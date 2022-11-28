using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseReference : MonoBehaviour
{
    PauseHandler pauseManager;

    void Start()
    {
        try
        {
            pauseManager = PauseHandler.instance;
        }
        catch { print("Pause Manager Doesn't Exist"); }
    }

    public void PauseGame()
    {
        try
        {
            pauseManager.OnPauseEvent?.Invoke(true);
        }
        catch { print("Pause Manager Doesn't Exist"); }
    }

    public void UnpauseGame()
    {
        try
        {
            pauseManager.OnPauseEvent?.Invoke(false);
        }
        catch { print("Pause Manager Doesn't Exist"); }
    }
}
