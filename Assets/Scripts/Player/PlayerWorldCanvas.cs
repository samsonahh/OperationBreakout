using System;
using UnityEngine;

public class PlayerWorldCanvas : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;

    private Vector3 _positionOffset;

    private void Start()
    {
        _positionOffset = transform.position - _playerTransform.position;
    }

    private void LateUpdate()
    {
        transform.position = _playerTransform.position + _positionOffset;
    }
}
