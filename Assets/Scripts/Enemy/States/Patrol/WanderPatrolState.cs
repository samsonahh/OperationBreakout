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
        
        private protected override void OnEnter()
        {
            _context.Animator.Play(_animationClip);
            
            _duration = UnityEngine.Random.Range(_durationInterval.x, _durationInterval.y);
            _timer = 0f;
            
            _targetLocation = _context.GetRandomWalkableTarget(_radius);
            
            _context.AIPath.canMove = true;
            _context.AIPath.maxSpeed = _moveSpeed;
            _context.AIPath.destination =  _targetLocation;
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
            if(_context.AIPath.desiredVelocity != Vector3.zero)
                _context.RotateToLookAtPosition(_context.transform.position + _context.AIPath.desiredVelocity, _rotationSpeed);
        }

        private protected override State<Enemy> GetTransition()
        {
            if (_context.CurrentTarget != null)
            {
                _context.ChaseState.SetTarget(_context.CurrentTarget);
                return _context.ChaseState;
            }
            
            if (_timer >= _duration)
                return _context.IdleState;

            if (Vector2.SqrMagnitude((Vector2)(_targetLocation - _context.transform.position)) <= 0.05f * 0.05f)
                return _context.IdleState;
            
            return null;
        }
    }
}