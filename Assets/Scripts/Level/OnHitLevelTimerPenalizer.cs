using System;
using UnityEngine;

public class OnHitLevelTimerPenalizer : MonoBehaviour
{
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private Health _playerHealth;

    private void Start()
    {
        _playerHealth.OnDamageTaken.AddListener(PlayerHealth_OnDamageTaken);
    }

    private void OnDestroy()
    {
        _playerHealth.OnDamageTaken.RemoveListener(PlayerHealth_OnDamageTaken);
    }

    private void PlayerHealth_OnDamageTaken(float damage)
    {
        _levelManager.SubtractTimer(damage);
    }
}
