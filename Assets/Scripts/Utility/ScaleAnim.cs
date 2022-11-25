using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public class ScaleAnim : MonoBehaviour
{
    [SerializeField] float _amplitude = 1.25f;
    [SerializeField] float _duration = 2;

    //float _staticDefaultScale;

    float _endScale;
    float _startScale;

    bool _isPlaying;

    void Start()
    {
        //_staticDefaultScale = transform.localScale.x;
        Scale();
    }

    async void Scale()
    {
        _startScale = transform.localScale.x;
        _endScale = _startScale * _amplitude;

        while (_isPlaying)
        {
            await transform.DOScale(_endScale, _duration).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
            if (!_isPlaying) { return; }
            await transform.DOScale(_startScale, _duration).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
        }
    }

    void OnEnable()
    {
        _isPlaying = true;
        Scale();
    }

    void OnDisable()
    {
        _isPlaying = false;
        //transform.localScale = new Vector3(_staticDefaultScale, _staticDefaultScale, _staticDefaultScale);
    }
}
