using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelTimerUI : MonoBehaviour
{
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private Image _clockImage;
    
    [Header("Shake Config")]
    [SerializeField] private float _shakeDuration = 0.3f;
    [SerializeField] private float _shakeStrengthMultiplier = 5f;

    private Vector2 _startTimerPosition;
    private Vector2 _startClockPosition;

    private void Awake()
    {
        _startTimerPosition = _timerText.rectTransform.position;
        _startClockPosition = _clockImage.rectTransform.position;
    }

    private void Start()
    {
        _levelManager.OnTimerSubtracted += LevelManager_OnTimerSubtracted;
    }

    private void OnDestroy()
    {
        _levelManager.OnTimerSubtracted -= LevelManager_OnTimerSubtracted;
    }

    private void Update()
    {
        _timerText.text = $"{Utils.FormatTimeMSSMs(_levelManager.CurrentTimer)}";
    }

    private void LevelManager_OnTimerSubtracted(float subtraction)
    {
        ShakeTimer(subtraction * _shakeStrengthMultiplier);
    }
    
    private void ShakeTimer(float strength)
    {
        _timerText.DOKill();
        
        _timerText.rectTransform.position = _startTimerPosition;
        _timerText.rectTransform.DOShakePosition(_shakeDuration, strength);
        
        _clockImage.DOKill();
        
        _clockImage.rectTransform.position = _startClockPosition;
        _clockImage.rectTransform.DOShakePosition(_shakeDuration, strength);
    }
}
