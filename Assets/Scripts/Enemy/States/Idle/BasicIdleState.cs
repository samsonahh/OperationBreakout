using LBG;
using UnityEngine;

namespace EnemyStates
{
    [System.Serializable]
    [SubclassPath(SubClassName = "Basic")]
    public class BasicIdleState : IdleState
    {
        [SerializeField] private float _duration = 1f;

        private float _timer = 0f;
        
        private protected override void OnEnter()
        {
            _timer = 0f;
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