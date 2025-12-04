using System;
using DG.Tweening;
using UnityEngine;

namespace PlayerStates
{
    [System.Serializable]
    public class DashState : State<PlayerController>
    {
        [Header("References")] 
        [SerializeField] private Health _health;
        [SerializeField] private SpriteRenderer _playerVisual;
        
        [Header("Config")]
        [SerializeField] private float _initialSpeed = 10f;
        [SerializeField] private float _dashDuration = 0.75f;
        [SerializeField] private float _invincibilityDuration = 0.25f;
        [SerializeField, Range(0f, 1f)] private float _invincibilityTransparency = 0.25f;
        [SerializeField] private Ease _easeType = Ease.OutCubic;
        [field: SerializeField] public float CooldownDuration { get; private set; }  = 3f;
        private const string DashIFramesTweenId = "DashIFrames";

        private float _currentSpeed;
        public float CooldownTimer { get; private set; } = 0f;

        public bool IsOnCooldown { get; private set; } = false;
        public event Action OnFailDash = delegate { };
        public event Action OnCooldownStarted = delegate { };
        public event Action OnDashReady = delegate { };
        
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
            }).SetId(DashIFramesTweenId);
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
    }
}