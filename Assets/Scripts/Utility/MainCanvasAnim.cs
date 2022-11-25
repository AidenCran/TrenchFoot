using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainCanvasAnim : MonoBehaviour
{
    [SerializeField] List<FadeUI> _fadeUIList = new List<FadeUI>();
    List<RectTransform> _rects = new List<RectTransform>();

    RectTransform _currentRect;

    float _duration = 1;

    int _lastIndex;
    int _currentIndex = 0;

    void Start()
    {
        foreach (var item in _fadeUIList)
        {
            _rects.Add(item.GetComponent<RectTransform>());
        }

        _currentRect = _rects[0];
    }

    void ChangeUIPanel(RectTransform rect, bool isForward = true)
    {
        _currentRect = rect;

        float shiftValue = /*2000*/Screen.height/1.2f;
        if (!isForward) shiftValue *= -1;

        rect.localPosition = new Vector3(0, shiftValue);
        rect.DOAnchorPosY(0, _duration);
    }

    public void SelectUI(int index)
    {
        _currentIndex = index;
        _currentRect.DOAnchorPosY(/*-2000*/-Screen.height/1.2f, _duration);

        ChangeUIPanel(_rects[index]);
    }

    public void SelectUI(int index, bool isForward)
    {
        _lastIndex = _currentIndex;
        _currentIndex = index;

        float shiftValue = /*-2000*/-Screen.height/1.2f;
        if (!isForward) shiftValue *= -1;

        _currentRect.DOAnchorPosY(shiftValue, _duration);

        ChangeUIPanel(_rects[index], isForward);
    }

    public void NextUI()
    {
        if (_currentIndex + 1 >= _fadeUIList.Count) return;
        SelectUI(_currentIndex + 1);
    }

    public void PreviousUI()
    {
        if (_currentIndex - 1 <= -1) return;

        // Reverse Anim Direction
        SelectUI(_currentIndex - 1, false);
    }

    // Utility => Button
    public void SelectUIReverse(int index) => SelectUI(index, false);
}
