using LBG;
using UnityEngine;

namespace EnemyStates
{
    [System.Serializable]
    [SubclassPath(SubClassName = "Spin")]
    public class SpinIdleState : IdleState
    {
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _spinSpeed = 1f;

        private int _direction;
        private float _timer;
        
        private protected override void OnEnter()
        {
            _context.Animator.Play(_animationClip);
            
            _timer = 0f;
            _direction = UnityEngine.Random.Range(0, 2) - 1;
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
            _context.transform.Rotate(0f, 0f, _direction * _spinSpeed * Time.fixedDeltaTime);
        }

        private protected override State<Enemy> GetTransition()
        {
            if (_context.CurrentTarget != null)
            {
                _context.ChaseState.SetTarget(_context.CurrentTarget);
                return _context.ChaseState;
            }
            
            if (_timer >= _duration)
                return _context.PatrolState;
            
            return null;
        }
    }
}