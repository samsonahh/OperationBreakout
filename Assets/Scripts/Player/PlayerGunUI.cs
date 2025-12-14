using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGunUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerGun _playerGun;
    [SerializeField] private Slider _shootCooldownBar;
    [SerializeField] private Slider _leftHeatBar;
    [SerializeField] private Slider _rightHeatBar;
    
    [Header("Config")]
    [SerializeField] private float _shootCooldownShakeDuration = 0.3f;
    [SerializeField] private float _shootCooldownShakeStrength = 3f;
    [SerializeField] private int _shootCooldownShakeFrequency = 20;
    [SerializeField] private float _heatOverfillShakeDuration = 0.25f;
    [SerializeField] private float _heatOverfillShakeStrength = 3f;
    [SerializeField] private int _heatOverfillShakeFrequency = 20;

    private Vector2 _shootCooldownBarStartPosition;
    private Vector2 _rightHeatBarStartPosition;
    private Vector2 _leftHeatBarStartPosition;
    
    private void Awake()
    {
        _shootCooldownBarStartPosition = _shootCooldownBar.transform.localPosition;
        _leftHeatBarStartPosition = _leftHeatBar.transform.localPosition;
        _rightHeatBarStartPosition = _rightHeatBar.transform.localPosition;
    }

    private void Start()
    {
        _playerGun.OnHeatAdded += PlayerGun_OnHeatAdded;
        _playerGun.OnOverheat += PlayerGun_OnOverheat;
        _playerGun.OnFailShoot += PlayerGun_OnFailShoot;
        
        PlayerGun_OnHeatAdded(0f);
    }

    private void OnDestroy()
    {
        _playerGun.OnHeatAdded -= PlayerGun_OnHeatAdded;
        _playerGun.OnOverheat -= PlayerGun_OnOverheat;
        _playerGun.OnFailShoot -= PlayerGun_OnFailShoot;
    }

    private void Update()
    {
        HandleShootCooldownBar();
        HandleHeatFillUpBar();
        HandleOverheatCooldown();
    }

    private void HandleShootCooldownBar()
    {
        float value = _playerGun.ShootCooldownTimer / _playerGun.ShootCooldown;
        _shootCooldownBar.value = 1f - value;
    }

    private void HandleOverheatCooldown()
    {
        if (!_playerGun.IsOverheated)
            return;

        float barValue = _playerGun.OverheatTimer / _playerGun.OverheatDuration;
        _leftHeatBar.value = 1f - barValue;
        _rightHeatBar.value = 1f - barValue;
    }

    private void HandleHeatFillUpBar()
    {
        if (_playerGun.IsOverheated)
            return;
        
        float barValue = _playerGun.CurrentHeat / _playerGun.MaxHeat;
        _leftHeatBar.value = barValue;
        _rightHeatBar.value = barValue;
    }

    private void PlayerGun_OnHeatAdded(float addedHeat)
    {

    }

    private void PlayerGun_OnOverheat()
    {
        _leftHeatBar.transform.DOKill();
        _leftHeatBar.transform.localPosition = _leftHeatBarStartPosition;
        
        _leftHeatBar.transform.DOShakePosition(
            _heatOverfillShakeDuration, 
            _heatOverfillShakeStrength,
            _heatOverfillShakeFrequency
            );
        
        _rightHeatBar.transform.DOKill();
        _rightHeatBar.transform.localPosition = _rightHeatBarStartPosition;
        
        _rightHeatBar.transform.DOShakePosition(
            _heatOverfillShakeDuration, 
            _heatOverfillShakeStrength,
            _heatOverfillShakeFrequency
        );
    }

    private void PlayerGun_OnFailShoot()
    {
        _shootCooldownBar.DOKill();
        _shootCooldownBar.transform.localPosition = _shootCooldownBarStartPosition;

        _shootCooldownBar.transform.DOShakePosition(
            _shootCooldownShakeDuration, 
            _shootCooldownShakeStrength,
            _shootCooldownShakeFrequency
            );
    }
}
