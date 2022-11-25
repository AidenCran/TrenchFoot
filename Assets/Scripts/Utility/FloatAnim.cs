using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public class FloatAnim : MonoBehaviour
{
    [SerializeField] float _amplitude = 1.25f;
    [SerializeField] float _duration = 2;

    //float _staticStartPositionY;

    float _endPosition;
    float _startPosition;

    bool _isPlaying;

    void Start()
    {
        //_startPosition = transform.position.y;
        //_endPosition = _startPosition * _amplitude;
        //_staticStartPositionY = _startPosition;
        Hover();
    }

    async void Hover()
    {
        _startPosition = transform.position.y;
        _endPosition = _startPosition * _amplitude;

        while (_isPlaying)
        {
            await transform.DOMoveY(_endPosition, _duration).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
            if (!_isPlaying) { return; }
            await transform.DOMoveY(_startPosition, _duration).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
        }
    }

    void OnEnable()
    {
        _isPlaying = true;
        Hover();
    }

    void OnDisable()
    {
        _isPlaying = false;
        //var pos = transform.position;
        //transform.position = new Vector3(pos.x, _staticStartPositionY, pos.z);
    }
}
