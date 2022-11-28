using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CameraPan : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera vCam;
    CinemachineTrackedDolly _camDolly;

    const float PanPercentage = 0.25f;
    const float Duration = 0.5f;

    float CurrentValue => _camDolly.m_PathPosition;

    [SerializeField] Button panLeftButton;
    [SerializeField] Button panRightButton;
    
    // Start is called before the first frame update
    void Start()
    {
        _camDolly = vCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        panLeftButton.onClick.AddListener(PanLeft);
        panRightButton.onClick.AddListener(PanRight);
    }

    void PanLeft()
    {
        var value = CurrentValue - PanPercentage;
        value = Mathf.Clamp01(value);
        
        DOTween.To((x) => _camDolly.m_PathPosition = x, CurrentValue, value, Duration).SetEase(Ease.Linear);
    }

    void PanRight()
    {
        var value = CurrentValue + PanPercentage;
        value = Mathf.Clamp01(value);
        
        DOTween.To((x) => _camDolly.m_PathPosition = x, CurrentValue, value, Duration).SetEase(Ease.Linear);
    }
}
