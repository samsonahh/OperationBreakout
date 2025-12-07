using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Health _playerHealth;
    [SerializeField] private BulletSpawner _bulletSpawner;

    [field: Header("Shoot Config")]
    [field: SerializeField] public float ShootCooldown { get; private set; } = 1f;

    [SerializeField] private int _burstCount = 3;
    [SerializeField] private float _burstDelay = 0.1f;
    [SerializeField] private float _bulletSpeed = 15f;
    [SerializeField] private float _bulletLifespan = 5f;
    [SerializeField] private float _bulletDamage = 1f;
    
    public float ShootCooldownTimer { get; private set; }
    public bool CanShoot { get; private set; } = true;
    public event Action OnFailShoot = delegate { };
    public event Action OnShoot = delegate { };
    
    [field: Header("Overheat Config")]
    [field: SerializeField] public float MaxHeat { get; private set; } = 10f;
    [SerializeField] private float _heatPerShot = 1f;
    [SerializeField] private float _heatDepletionRate = 1f;
    [field: SerializeField] public float OverheatDuration { get; private set; } = 3f;

    public float CurrentHeat { get; private set; } = 0f;
    public float OverheatTimer { get; private set; }
    public bool IsOverheated { get; private set; } = false;
    public event Action<float> OnHeatAdded = delegate { };
    public event Action OnOverheat = delegate { };
    
    private void OnEnable()
    {
        InputManager.Instance.Attack += InputManager_Attack;
    }

    private void OnDisable()
    {
        if(InputManager.Instance != null)
            InputManager.Instance.Attack -= InputManager_Attack;
    }

    private void Update()
    {
        HandleShootCooldown(Time.deltaTime);
        HandleHeatDepletion(Time.deltaTime);
        HandleOverheatDuration(Time.deltaTime);
    }

    private void HandleShootCooldown(float deltaTime)
    {
        if (CanShoot)
            return;

        ShootCooldownTimer += deltaTime;
        if (ShootCooldownTimer >= ShootCooldown)
        {
            ShootCooldownTimer = ShootCooldown;
            CanShoot = true;
        }
    }

    private void StartShootCooldown()
    {
        ShootCooldownTimer = 0f;
        CanShoot = false;
    }

    private void InputManager_Attack()
    {
        if (!CanShoot)
        {
            OnFailShoot.Invoke();
            return;
        }

        if (IsOverheated)
        {
            OnFailShoot.Invoke();
            return;
        }
        
        Burst().Forget();
        
        StartShootCooldown();
        AddHeat();
        
        OnShoot.Invoke();
    }

    private async UniTask Burst()
    {
        for (int i = 0; i < _burstCount; i++)
        {
            _bulletSpawner.Spawn(_playerController.Team, _playerController.ForwardDirection, _bulletSpeed, _bulletLifespan, _bulletDamage);
            await UniTask.Delay((int)(_burstDelay * 1000f), DelayType.DeltaTime);
        }
    }
    
    private void AddHeat()
    {
        if (IsOverheated)
            return;
        
        CurrentHeat += _heatPerShot;
        OnHeatAdded.Invoke(CurrentHeat);
        if (CurrentHeat >= MaxHeat)
        {
            CurrentHeat = MaxHeat;
            IsOverheated = true;
            OnOverheat.Invoke();
        }
    }

    private void HandleHeatDepletion(float deltaTime)
    {
        if (IsOverheated)
            return;

        if (CurrentHeat <= 0f)
            return;
        
        CurrentHeat -= _heatDepletionRate * deltaTime;
        if (CurrentHeat <= 0f)
            CurrentHeat = 0f;
    }
    
    private void HandleOverheatDuration(float deltaTime)
    {
        if (!IsOverheated)
            return;
        
        OverheatTimer += deltaTime;
        if (OverheatTimer >= OverheatDuration)
        {
            OverheatTimer = 0f;
            CurrentHeat = 0f;
            IsOverheated = false;
        }
    }
}
