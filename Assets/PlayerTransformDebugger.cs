using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTransformDebugger : MonoBehaviour
{
    [HideInInspector] public bool DebugPlayerTransform;
    [Range(0.1f, 10)] [SerializeField] private float _movementSpeed;
    [Range(0.1f, 10)] [SerializeField] private float _rotationSpeed;
    private Transform _player;
    private float _horizontalInput = 0;
    private float _forwardInput = 0;

    private void Start()
    {
        _player = TXRPlayer.Instance.transform;
    }

    private void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _forwardInput = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        if (!DebugPlayerTransform)
        {
            return;
        }

        MovePlayer();
        RotatePlayer();
    }

    private void MovePlayer()
    {
        float horizontalMovement = _horizontalInput * _movementSpeed * Time.deltaTime;
        float forwardMovement = _forwardInput * _movementSpeed * Time.deltaTime;

        _player.position = new Vector3(_player.position.x + horizontalMovement, _player.position.y, _player.position.z + forwardMovement);
    }

    private void RotatePlayer()
    {
    }
}