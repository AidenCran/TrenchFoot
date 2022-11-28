using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuMusic : MonoBehaviour
{
    void Start()
    {
        SoundManager.instance.PlayMenuMusic();
    }
}
