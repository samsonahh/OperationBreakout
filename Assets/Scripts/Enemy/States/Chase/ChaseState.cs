using UnityEngine;

namespace EnemyStates
{
    [System.Serializable]
    public class ChaseState : State<Enemy>
    {
        [SerializeField] private float _speed = 2f;
        [SerializeField] private float _rotationSpeed = 5f;
        [SerializeField] private float _maxDuration = 5f;
        [SerializeField] private float _minDuration = 2f;
        
        private Transform _target;
        public void SetTarget(Transform target) => _target = target;

        private float _timer;
        private float _duration;
        
        private protected override void OnEnter()
        {
            _context.AIPath.canMove = true;
            _context.AIPath.destination = _target.position;
            _context.AIPath.maxSpeed = _speed;
            
            _duration = Random.Range(_minDuration, _maxDuration);
            _timer = 0f;
        }

        private protected override void OnExit()
        {
            _context.AIPath.canMove = false;
        }

        private protected override void OnUpdate()
        {
            _timer += Time.deltaTime;
            
        }

        private protected override void OnFixedUpdate()
        {
            _context.AIPath.destination = _target.position;
            
            if(_context.AIPath.desiredVelocity != Vector3.zero)
                _context.RotateToLookAtPosition(_context.transform.position + _context.AIPath.desiredVelocity, _rotationSpeed);
        }

        private protected override State<Enemy> GetTransition()
        {
            if (_target == null)
                return _context.IdleState;

            if (_timer >= _duration)
            {
                _context.AttackWindupState.SetLockOnTarget(_target);
                return _context.AttackWindupState;
            }
            
            return null;
        }
    }
}