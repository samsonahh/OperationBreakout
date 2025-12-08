using System.Collections.Generic;
using System.Linq;
using LBG;
using Pathfinding;
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
        private List<Vector3> _path;
        private Vector3 _currentMoveDirection;
        
        private protected override void OnEnter()
        {
            _duration = UnityEngine.Random.Range(_durationInterval.x, _durationInterval.y);
            _timer = 0f;

            Ticker.Instance.OnTick += Ticker_OnTick;
            _context.Seeker.pathCallback += OnPathFound;

            _targetLocation = _context.GetRandomWalkableTarget(_radius);
            _context.AIPath.destination =  _targetLocation;
            
            RequestNewPath();
        }

        private protected override void OnExit()
        {
            if(Ticker.Instance != null)
                Ticker.Instance.OnTick -= Ticker_OnTick;
            
            _context.Seeker.pathCallback -= OnPathFound;
        }

        private protected override void OnUpdate()
        {
            _timer += Time.deltaTime;
        }

        private protected override void OnFixedUpdate()
        {
            if (_path.Count <= 1)
                return;
            
            _context.RigidBody.MovePosition(_context.transform.position + _currentMoveDirection * (_moveSpeed * Time.fixedDeltaTime));
            _context.RotateToLookAtPosition(_context.transform.position + _currentMoveDirection, _rotationSpeed);
        }

        private protected override State<Enemy> GetTransition()
        {
            if (_context.CurrentTarget != null)
                return _context.AttackWindupState;
            
            if (_timer >= _duration)
                return _context.IdleState;

            if (Vector2.SqrMagnitude((Vector2)(_targetLocation - _context.transform.position)) <= 0.05f * 0.05f)
                return _context.IdleState;
            
            return null;
        }

        private void Ticker_OnTick()
        {
            RequestNewPath();
        }

        private void RequestNewPath()
        {
            _path = new();
            _context.AIPath.SearchPath();
        }

        private void OnPathFound(Path path)
        {
            _path = path.vectorPath;
            if (path.vectorPath.Count == 0)
            {
                _currentMoveDirection = Vector3.zero;
            }
            else
            {
                _currentMoveDirection = (_path[1] - _context.transform.position).normalized;
            }
        }
    }
}