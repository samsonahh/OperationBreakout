using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreTextUI : MonoBehaviour
{
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private TMP_Text _scoreText;
    
    private Vector2 _startPosition;
    private int _targetScore;
    private int _currentScore;
    private float _scoreCountUpTimer;
    
    [Header("Config")]
    [SerializeField] private float _scoreCountUpDelay = 0.01f;
    [SerializeField] private int _scoreCountUpAmount = 5;
    [SerializeField] private float _shakeDuration = 0.3f;
    [SerializeField] private float _shakeStrength = 3f;
    [SerializeField] private int _shakeFrequency = 20;
    
    private void Awake()
    {
        _startPosition = transform.localPosition;
    }

    private void Start()
    {
        _scoreManager.OnScoreAdded += ScoreManager_OnScoreAdded;
    }

    private void OnDestroy()
    {
        _scoreManager.OnScoreAdded -= ScoreManager_OnScoreAdded;
    }

    private void Update()
    {
        HandleScoreCountUp();

        _scoreText.text = GetScoreString(_currentScore);
    }

    private void ScoreManager_OnScoreAdded(int addedScore)
    {
        _targetScore = _scoreManager.Score;
        _scoreCountUpTimer = 0f;

        transform.DOKill();
        transform.localPosition = _startPosition;

        transform.DOShakePosition(
            _shakeDuration,
            _shakeStrength,
            _shakeFrequency
        );
    }

    private void HandleScoreCountUp()
    {
        if (_currentScore >= _targetScore)
        {
            _currentScore = _targetScore;
            return;
        }

        _scoreCountUpTimer += Time.deltaTime;

        if (_scoreCountUpTimer >= _scoreCountUpDelay)
        {
            _scoreCountUpTimer = 0f;
            _currentScore = Mathf.Min(
                _currentScore + _scoreCountUpAmount,
                _targetScore
            );
        }
    }

    private string GetScoreString(int score)
    {
        return score.ToString("D7");
    }
}
