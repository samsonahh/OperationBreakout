using UnityEngine;

namespace EnemyStates
{
    [System.Serializable]
    public class AttackWindupState : State<Enemy>
    {
        [SerializeField] private float _duration = 0.25f;
        [SerializeField] private float _lookAtTargetRotationSpeed = 10f;

        private float _timer;
        
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
            if (_context.CurrentTarget == null)
                return;
            
            _context.RotateToLookAtPosition(_context.CurrentTarget.position, _lookAtTargetRotationSpeed);
        }

        private protected override State<Enemy> GetTransition()
        {
            if (_timer >= _duration)
                return _context.AttackState;

            return null;
        }
    }
}