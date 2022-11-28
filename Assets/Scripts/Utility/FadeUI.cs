using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


/// <summary>
/// Provides Fade-able UI for canvas groups
/// </summary>

[RequireComponent(typeof(CanvasGroup))]
public class FadeUI : MonoBehaviour
{
    CanvasGroup _canvasGroup;

    [SerializeField] float _duration = 1;

    // Defines if Value is visible or not
    [SerializeField] bool _isDisplayed;

    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        SetCanvasFade(_isDisplayed);
    }

    public void ShowUI()
    {
        if (_isDisplayed) return;
        ShowUI(_duration);
        // FadeCanvas(1);
        // _isDisplayed = !_isDisplayed;
        // _canvasGroup.blocksRaycasts = true;
        // _canvasGroup.interactable = true;
    }

    public void ShowUI(float speed)
    {
        if (_isDisplayed) return;
        FadeCanvas(1, speed);
        _isDisplayed = !_isDisplayed;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

    public void HideUI()
    {
        if (!_isDisplayed) return;
        HideUI(_duration);
        // FadeCanvas(0);
        // _isDisplayed = !_isDisplayed;
        // _canvasGroup.blocksRaycasts = false;
        // _canvasGroup.interactable = false;
    }

    public void HideUI(float speed)
    {
        if (!_isDisplayed) return;
        FadeCanvas(0, speed);
        _isDisplayed = !_isDisplayed;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }
    
    void FadeCanvas(float value) => _canvasGroup.DOFade(value, _duration);
    void FadeCanvas(float value, float duration) => _canvasGroup.DOFade(value, duration);

    public void SetCanvasFade(bool enabled)
    {
        if (enabled) ShowUI();
        else HideUI();
    }
    
    public void InvertedSetCanvasFade(bool enabled)
    {
        bool b = !enabled;
        if (b) ShowUI();
        else HideUI();
    }
}
