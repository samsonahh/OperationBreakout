using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDashUI : MonoBehaviour, IInitializable
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Slider _dashCooldownBar;
    private PlayerStates.DashState _dashState;
    
    [Header("Config")]
    [SerializeField] private float _dashCooldownShakeDuration = 0.3f;
    [SerializeField] private float _dashCooldownShakeStrength = 5f;
    [SerializeField] private int _dashCooldownShakeFrequency = 20;
    private Vector3 _dashCooldownBarStartPosition;
    
    public void Initialize()
    {
        _dashState = _playerController.DashState;
        
        _dashState.OnFailDash += PlayerDash_OnFailDash;
        _dashState.OnCooldownStarted += PlayerDash_OnCooldownStarted;
        _dashState.OnDashReady += PlayerDash_OnDashReady;
        
        _dashCooldownBarStartPosition = _dashCooldownBar.transform.localPosition;
    }

    private void OnDestroy()
    {
        _dashState.OnFailDash -= PlayerDash_OnFailDash;
        _dashState.OnCooldownStarted -= PlayerDash_OnCooldownStarted;
        _dashState.OnDashReady -= PlayerDash_OnDashReady;
    }

    private void Update()
    {
        HandleDashCooldownBar();   
    }

    private void HandleDashCooldownBar()
    {
        if (!_dashState.IsOnCooldown)
            return;

        float barValue = _dashState.CooldownTimer / _dashState.CooldownDuration;
        _dashCooldownBar.value = 1 - barValue;
    }

    private void PlayerDash_OnFailDash()
    {
        _dashCooldownBar.transform.DOKill();
        _dashCooldownBar.transform.localPosition = _dashCooldownBarStartPosition;
        
        _dashCooldownBar.transform.DOShakePosition(
            _dashCooldownShakeDuration, 
            _dashCooldownShakeStrength,
            _dashCooldownShakeFrequency
        ).OnUpdate(() =>
        {
            _dashCooldownBar.transform.localPosition =
                _dashCooldownBar.transform.localPosition.WithZ(_dashCooldownBarStartPosition.z);
        });
    }
    
    private void PlayerDash_OnCooldownStarted()
    {
        _dashCooldownBar.gameObject.SetActive(true);
    }
    
    private void PlayerDash_OnDashReady()
    {
        _dashCooldownBar.gameObject.SetActive(false);
    }
}
