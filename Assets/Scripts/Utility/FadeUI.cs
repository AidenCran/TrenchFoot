using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


/// <summary>
/// Provides Fadable UI for canvas groups
/// </summary>

[RequireComponent(typeof(CanvasGroup))]
public class FadeUI : MonoBehaviour
{
    CanvasGroup _canvasGroup;

    float _duration = 1;

    // Defines if Value is visible or not
    [SerializeField] bool _isDisplayed;

    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_isDisplayed)
        { 
            ShowUI(); 
        }
        else 
        { 
            HideUI(); 
        }
    }

    public void ShowUI()
    {
        if (!_isDisplayed)
        {
            FadeCanvas(1);
            _isDisplayed = !_isDisplayed;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }
    }

    public void ShowUI(float speed)
    {
        if (!_isDisplayed)
        {
            FadeCanvas(1, speed);
            _isDisplayed = !_isDisplayed;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }
    }

    public void HideUI()
    {
        if (_isDisplayed)
        {
            FadeCanvas(0);
            _isDisplayed = !_isDisplayed;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }
    }

    public void HideUI(float speed)
    {
        if (_isDisplayed)
        {
            FadeCanvas(0, speed);
            _isDisplayed = !_isDisplayed;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }
    }

    public void FadeCanvas(float value) => _canvasGroup.DOFade(value, _duration);

    public void FadeCanvas(float value, float duration) => _canvasGroup.DOFade(value, duration);
}
