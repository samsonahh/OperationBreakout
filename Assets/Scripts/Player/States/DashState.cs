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
        
        [Header("Config")]
        [SerializeField] private float _initialSpeed = 10f;
        [SerializeField] private float _dashDuration = 0.75f;
        [SerializeField] private float _invincibilityDuration = 0.25f;
        [SerializeField] private Ease _easeType = Ease.OutCubic;
        [field: SerializeField] public float CooldownDuration { get; private set; }  = 3f;
        private const string DashIFramesTweenId = "DashIFrames";

        private float _currentSpeed;
        public float CooldownTimer { get; private set; } = 0f;

        public bool IsOnCooldown => CooldownTimer <= 0;
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
            DOVirtual.DelayedCall(_invincibilityDuration, () =>
            {
                _health.SetInvincibility(false);
            }).SetId(DashIFramesTweenId);
        }

        private protected override void OnExit()
        {
            CooldownTimer = CooldownDuration;
            
            // Just in case player never goes back to being vulnerable
            _health.SetInvincibility(false);
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
            if (IsOnCooldown)
                return;
            
            CooldownTimer -= deltaTime;

            if (IsOnCooldown)
            {
                CooldownTimer = 0f;
                OnDashReady?.Invoke();
            }
        }
        
        public void TryDash()
        {
            if (_stateMachine.CurrentState == this)
                return;
            
            if (!IsOnCooldown)
                return;
            
            _stateMachine.ChangeState(this);
        }
    }
}