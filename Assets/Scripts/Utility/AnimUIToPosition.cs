using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimUIToPosition : MonoBehaviour
{
    [SerializeField] RectTransform _transform;
    [SerializeField] Vector2 _offset;

    [SerializeField] float _duration = 3;

    [SerializeField] Ease easeType;

    Vector2 _startPosition;
    Vector2 _endPosition;

    [SerializeField] bool _animOnStart = false;

    bool _animComplete;

    void Start()
    {
        _startPosition = _transform.anchoredPosition;
        _endPosition = _startPosition + _offset;

        if (!_animOnStart) return;
        AnimUIElement();
    }

    public void AnimUIElement(bool isFoward = true)
    {
        if (isFoward)
        {
            _transform.DOAnchorPos(_endPosition, _duration).SetEase(easeType);
            _animComplete = true;
            return;
        }

        if (!isFoward)
        {
            _transform.DOAnchorPos(_startPosition, _duration).SetEase(easeType);
            _animComplete = false;
        }
    }

    public void ToggleAnim()
    {
        AnimUIElement(!_animComplete);
    }
}
