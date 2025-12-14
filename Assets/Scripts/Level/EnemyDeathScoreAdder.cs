using System;
using UnityEngine;

public class EnemyDeathScoreAdder : MonoBehaviour
{
    private ScoreManager _scoreManager;
    
    [SerializeField] private Health _health;
    
    [Header("Config")]
    [SerializeField] private int _scoreToAdd = 5000;

    private void Start()
    {
        _scoreManager = FindAnyObjectByType<ScoreManager>();
        if (_scoreManager == null) Debug.LogError("ScoreManager not found");
        
        _health.OnDeath.AddListener(HandleOnDeath);
    }

    private void OnDestroy()
    {
        _health.OnDeath.RemoveListener(HandleOnDeath);
    }

    private void HandleOnDeath()
    {
        if (_scoreManager == null)
            return;
        
        _scoreManager.AddScore(_scoreToAdd);
    }
}
