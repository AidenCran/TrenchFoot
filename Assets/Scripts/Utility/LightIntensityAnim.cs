using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(UnityEngine.Rendering.Universal.Light2D))]
public class LightIntensityAnim : MonoBehaviour
{
    UnityEngine.Rendering.Universal.Light2D _light;

    [SerializeField] float _amplitude = 1.25f;
    [SerializeField] float _duration = 2;

    float _endPosition;
    float _startPosition;

    bool _isPlaying = true;

    void Start()
    {
        _light = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        _startPosition = _light.intensity;
        _endPosition = _startPosition * _amplitude;
        Hover();
    }

    async void Hover()
    {
        while (_isPlaying)
        {
            await DOTween.To(() => _light.intensity, x => _light.intensity = x, _endPosition, _duration).AsyncWaitForCompletion();
            if (!_isPlaying) { return; }
            await DOTween.To(() => _light.intensity, x => _light.intensity = x, _startPosition, _duration).AsyncWaitForCompletion();
        }
    }

    void OnEnable()
    {
        _isPlaying = true;
        Hover();
    }

    void OnDisable() => _isPlaying = false;
}
