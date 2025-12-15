using System;
using UnityEngine;

public class EnemyDeathManager : MonoBehaviour
{
    private ScoreManager _scoreManager;
    private LevelManager _levelManager;
    
    [SerializeField] private Enemy _enemy;
    [SerializeField] private Health _health;
    
    [Header("Config")]
    [SerializeField] private float _timeToAdd = 10f;
    [SerializeField] private int _scoreToAdd = 5000;

    private void Start()
    {
        _scoreManager = FindAnyObjectByType<ScoreManager>();
        if (_scoreManager == null) Debug.LogError("ScoreManager not found");
        
        _levelManager = FindAnyObjectByType<LevelManager>();
        if (_levelManager == null) Debug.LogError("LevelManager not found");
        
        _health.OnDeath.AddListener(HandleOnDeath);
        _health.OnDamageTaken.AddListener(HandleOnDamageTaken);
    }

    private void OnDestroy()
    {
        _health.OnDeath.RemoveListener(HandleOnDeath);
        _health.OnDamageTaken.RemoveListener(HandleOnDamageTaken);
    }

    private void HandleOnDeath()
    {
        if (_scoreManager == null)
            return;
        
        _scoreManager.AddScore(_scoreToAdd);
        _levelManager.AddTimer(_timeToAdd);
    }

    private void HandleOnDamageTaken(float damageTaken)
    {
        if (_enemy.StateMachine.CurrentState == _enemy.IdleState ||
            _enemy.StateMachine.CurrentState == _enemy.PatrolState)
        {
            _enemy.ChaseState.SetTarget(GameObject.FindAnyObjectByType<PlayerController>().transform);
            _enemy.StateMachine.ChangeState(_enemy.ChaseState);
        }
    }
}
