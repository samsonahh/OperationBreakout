using LBG;
using UnityEngine;

namespace EnemyStates
{
    [System.Serializable]
    [SubclassPath("Gun", "Single Shot")]
    public class SingleShotAttackState : AttackState
    {
        [SerializeField] private BulletSpawner _bulletSpawner;
        [SerializeField] private float _bulletSpeed = 10f;
        [SerializeField] private float _bulletLifespan = 5f;
        [SerializeField] private float _bulletDamage = 1f;
        [SerializeField] private int _shootCount = 1;
        [SerializeField] private float _shootCooldown = 1f;
        
        private int _currentShootCount;
        private float _currentShootCooldownTimer;
        
        private protected override void OnEnter()
        {
            _currentShootCount = 0;
            _currentShootCooldownTimer = _shootCooldown; // shoot immediately
        }

        private protected override void OnExit()
        {
            
        }

        private protected override void OnUpdate()
        {
            HandleShootingCooldown();
        }

        private protected override void OnFixedUpdate()
        {
            
        }

        private protected override State<Enemy> GetTransition()
        {
            if (_currentShootCount >= _shootCount && _currentShootCooldownTimer >= _shootCooldown)
                return _context.PatrolState;
            
            return null;
        }

        private void HandleShootingCooldown()
        {
            _currentShootCooldownTimer += Time.deltaTime;
            if (_currentShootCooldownTimer >= _shootCooldown && _currentShootCount < _shootCount)
            {
                _bulletSpawner.Spawn(_context.Team, _context.ForwardDirection, _bulletSpeed, _bulletLifespan, _bulletDamage);
                _currentShootCount++;
                _currentShootCooldownTimer = 0f;
            }
        }
    }
}