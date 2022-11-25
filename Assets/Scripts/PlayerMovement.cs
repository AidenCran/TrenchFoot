using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] Transform _playerSpriteTransform;

    PlayerInput _playerInput;
    InputAction _movement;

    Vector2 _moveDirection = Vector2.zero;

    // PlayerEntity _playerEntity;

    float _moveSpeed = 5;

    void Awake() => _playerInput = new PlayerInput();

    void Start()
    {
        // _moveSpeed = _playerEntity.Stats.MoveSpeed;
        // _playerEntity.OnStatUpgrade.AddListener((x) => _moveSpeed = x.MoveSpeed);
    }

    void OnEnable()
    {
        _movement = _playerInput.Player.Move;
        _movement.Enable();
    }

    void OnDisable()
    {
        _movement.Disable();
    }

    void Update() => _moveDirection = _movement.ReadValue<Vector2>();

    void FixedUpdate()
    {
        _rb.velocity = new Vector2(_moveDirection.x * _moveSpeed, _moveDirection.y * _moveSpeed);
        var localScale = _playerSpriteTransform.localScale;
        
        switch (_moveDirection.x)
        {
            case 0:
                return;
            case -1:
            {
                localScale.x *= -1;

                _playerSpriteTransform.localScale = localScale;
                return;
            }
            case 1:
                _playerSpriteTransform.localScale = localScale;
                return;
        }
    }
}
