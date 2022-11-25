using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationReference : MonoBehaviour
{
    Navigation _navigation;

    void Start()
    {
        try { _navigation = Navigation.instance; }
        catch { }
    }

    public void ChangeScene(string str)
    {
        _navigation.ChangeScene(str);
    }

    public void Quit() => _navigation.Quit();
}
