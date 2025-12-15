using System;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    private LevelManager _levelManager;

    private void Start()
    {
        _levelManager = FindObjectOfType<LevelManager>();
        if(_levelManager == null) Debug.LogError("LevelManager not found");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerController>(out var playerController))
        {
            _levelManager.WinGame();
        }
    }
}
