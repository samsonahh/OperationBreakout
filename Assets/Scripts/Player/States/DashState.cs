using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PlayerStates
{
    [System.Serializable]
    public class DashState : State<PlayerController>
    {
        [Header("References")] 
        [SerializeField] private Health _health;
        [SerializeField] private SpriteRenderer _playerVisual;
        
        [TabGroup("Config", "Basic")]
        [SerializeField] private float _initialSpeed = 10f;
        [TabGroup("Config", "Basic")]
        [SerializeField] private float _dashDuration = 0.75f;
        [field: TabGroup("Config", "Basic")]
        [field: SerializeField] public float CooldownDuration { get; private set; }  = 3f;
        
        [TabGroup("Config", "Contact")]
        [SerializeField] private float _contactDamage = 1f;
        [Header("Camera Shake")]
        [TabGroup("Config", "Contact")]
        [SerializeField] private float _contactCameraShakeDuration = 0.2f;
        [TabGroup("Config", "Contact")]
        [SerializeField] private float _contactCameraShakeAmplitude = 1f;
        [TabGroup("Config", "Contact")]
        [SerializeField] private float _contactCameraShakeFrequency = 1f;
        [Header("Impact Frames")]
        [TabGroup("Config", "Contact")]
        [SerializeField] private float _contactImpactFramesTimeScale = 0.1f;
        [TabGroup("Config", "Contact")]
        [SerializeField] private float _contactImpactFramesDuration = 0.25f;
        
        [TabGroup("Config", "Invincibility")]
        [SerializeField] private float _invincibilityDuration = 0.25f;
        [TabGroup("Config", "Invincibility")]
        [SerializeField, Range(0f, 1f)] private float _invincibilityTransparency = 0.25f;
        [TabGroup("Config", "Invincibility")]
        [SerializeField] private Ease _easeType = Ease.OutCubic;
        private const string DashIFramesTweenId = "DashIFrames";

        private float _currentSpeed;
        public float CooldownTimer { get; private set; } = 0f;

        public bool IsOnCooldown { get; private set; } = false;
        public event Action OnFailDash = delegate { };
        public event Action OnCooldownStarted = delegate { };
        public event Action OnDashReady = delegate { };

        private HashSet<Collider2D> _contactDamagedColliders;
        
        private protected override void OnEnter()
        {
            // Kill previous dash tween
            DOTween.Kill(this);
            DOTween.Kill(DashIFramesTweenId);
            
            _currentSpeed = _initialSpeed;
            DOVirtual.Float(_initialSpeed, _context.MoveState.Speed, _dashDuration, (newSpeed) =>
                {
                    _currentSpeed = newSpeed;
                })
                .SetEase(_easeType)
                .SetId(this)
                .SetUpdate(false)
                .OnComplete(() =>
                {
                    _stateMachine.ChangeState(_context.MoveState);
                });
            
            // Handle invincibility
            _health.SetInvincibility(true);
            _playerVisual.color = new Color(1, 1, 1, _invincibilityTransparency);
            DOVirtual.DelayedCall(_invincibilityDuration, () =>
            {
                _health.SetInvincibility(false);
                _playerVisual.color = Color.white;
            }).SetId(DashIFramesTweenId)
            .SetUpdate(false);
            
            // reset contact hit enemies
            _contactDamagedColliders = new();
        }

        private protected override void OnExit()
        {
            CooldownTimer = 0f;
            IsOnCooldown = true;
            OnCooldownStarted.Invoke();
            
            // Just in case player never goes back to being vulnerable
            _health.SetInvincibility(false);
            _playerVisual.color = Color.white;
        }

        private protected override void OnUpdate()
        {
            _context.RotateToMousePosition();
        }

        private protected override void OnFixedUpdate()
        {
            Vector3 moveDelta = _currentSpeed * Time.fixedDeltaTime * InputManager.Instance.MoveDirection;
            _context.RigidBody.MovePosition(_context.transform.position + moveDelta);

            TryHitEnemies();
        }
        
        private protected override State<PlayerController> GetTransition()
        {
            return null;
        }

        public void UpdateDashCooldown(float deltaTime)
        {
            if (!IsOnCooldown)
                return;
            
            CooldownTimer += deltaTime;
            if (CooldownTimer >= CooldownDuration)
            {
                CooldownTimer = CooldownDuration;
                IsOnCooldown = false;
                OnDashReady?.Invoke();
            }
        }
        
        public void TryDash()
        {
            if (_stateMachine.CurrentState == this)
            {
                OnFailDash.Invoke();
                return;
            }

            if (IsOnCooldown)
            {
                OnFailDash.Invoke();
                return;
            }
            
            _stateMachine.ChangeState(this);
        }

        private void TryHitEnemies()
        {
            if (!_health.IsInvincible)
                return;
            
            Collider2D[] collisions =
                Physics2D.OverlapCircleAll(_context.transform.position, _context.HitBoxCollider.radius, LayerMask.GetMask("Hitbox"));
            if (collisions == null)
                return;
            if (collisions.Length == 0)
                return;

            foreach (Collider2D collision in collisions)
            {
                if(_contactDamagedColliders.Contains(collision))
                    continue;
                
                Health health = collision.gameObject.GetComponentInParent<Health>();
                if (health == null)
                    continue;
                ITeam team = collision.gameObject.GetComponentInParent<ITeam>();
                if (team == null)
                    continue;

                if (team.Team == _context.Team)
                    continue;
                
                health.TakeDamage(_contactDamage);
                _contactDamagedColliders.Add(collision);
                GameManager.Instance.StartImpactFrames(_contactImpactFramesTimeScale, _contactImpactFramesDuration);
                CameraManager.Instance.CameraShaker.ShakeCamera(_contactCameraShakeAmplitude, _contactCameraShakeFrequency, _contactCameraShakeDuration);
            }
        }
    }
}