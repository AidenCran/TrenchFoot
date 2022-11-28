using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Panels : MonoBehaviour
{
    public GameObject MenuPanel;
    public GameObject TutorialPanel;
    public GameObject SystemPanel;
    public GameObject WinLosePanel;


    public void StartTutorial()
    {
        MenuPanel.SetActive(false);
        TutorialPanel.SetActive(true);
    }

    public void ExitTutorial()
    {
        MenuPanel.SetActive(true);
        TutorialPanel.SetActive(false);
    }

    public void StartSystem()
    {
        TutorialPanel.SetActive(false);
        SystemPanel.SetActive(true);
    }

    public void ExitSystem()
    {
        TutorialPanel.SetActive(true);
        SystemPanel.SetActive(false);
    }

    public void StartWinLose()
    {
        SystemPanel.SetActive(false);
        WinLosePanel.SetActive(true);
    }

    public void ExitWinLose()
    {
        SystemPanel.SetActive(true);
        WinLosePanel.SetActive(false);
    }



}
