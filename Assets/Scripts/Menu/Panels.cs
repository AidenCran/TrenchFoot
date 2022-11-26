using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Panels : MonoBehaviour
{
    public GameObject MenuPanel;
    public GameObject ControlsPanel;

    public void StartTutorial()
    {
        MenuPanel.SetActive(false);
        ControlsPanel.SetActive(true);
    }

    public void ExitTutorial()
    {
        MenuPanel.SetActive(true);
        ControlsPanel.SetActive(false);
    }

}
