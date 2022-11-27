using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundReference : MonoBehaviour
{
    SoundManager soundManager;

    void Start() => soundManager = SoundManager.instance;

    public void PlayMusic(AudioClip clip) => soundManager.PlayMusic(clip);

    public void PlaySound(AudioClip clip) => soundManager.PlaySound(clip);
}
