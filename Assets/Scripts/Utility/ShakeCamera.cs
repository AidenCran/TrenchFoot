using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class ShakeCamera : MonoBehaviour
{
    #region Singleton

    public static ShakeCamera instance { get; private set; }

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

    CinemachineImpulseSource _impulseSource;

    void Awake()
    {
        Singleton();
    }

    void Start()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake()
    {
        _impulseSource.GenerateImpulse();
    }

    public void Shake(float intensity)
    {
        _impulseSource.GenerateImpulse(intensity);
    }
}
