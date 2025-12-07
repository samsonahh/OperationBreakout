using LBG;
using UnityEngine;

namespace EnemyStates
{
    [System.Serializable]
    [SubclassPath(SubClassName = "Wander")]
    public class WanderPatrolState : PatrolState
    {
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _rotationSpeed = 5f;
        [SerializeField] private Vector2 _durationInterval = new Vector2(3f, 6f);
        [SerializeField] private float _radius = 5f;

        private float _duration;
        private float _timer;

        private Vector3 _targetLocation;
        
        private protected override void OnEnter()
        {
            _duration = UnityEngine.Random.Range(_durationInterval.x, _durationInterval.y);
            _timer = 0f;

            _targetLocation = (Vector3)UnityEngine.Random.insideUnitCircle.normalized * _radius + _context.transform.position;
        }

        private protected override void OnExit()
        {
            
        }

        private protected override void OnUpdate()
        {
            _timer += Time.deltaTime;
        }

        private protected override void OnFixedUpdate()
        {
            Vector3 direction = (Vector2)(_targetLocation - _context.transform.position).normalized;
            _context.RigidBody.MovePosition(_context.transform.position + direction * (_moveSpeed * Time.fixedDeltaTime));
            
            _context.RotateToLookAtPosition(_targetLocation, _rotationSpeed);
        }

        private protected override State<Enemy> GetTransition()
        {
            if (_timer >= _duration)
                return _context.IdleState;

            if (Vector2.SqrMagnitude((Vector2)_targetLocation - (Vector2)_context.transform.position) <= 0.05f * 0.05f)
                return _context.IdleState;
            
            return null;
        }
    }
}