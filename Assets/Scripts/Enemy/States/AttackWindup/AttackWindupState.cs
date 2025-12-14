using UnityEngine;

namespace EnemyStates
{
    [System.Serializable]
    public class AttackWindupState : State<Enemy>
    {
        [SerializeField] private float _duration = 0.25f;
        [SerializeField] private float _lookAtTargetRotationSpeed = 10f;

        private float _timer;
        private Transform _target;

        public void SetLockOnTarget(Transform target)
        {
            _target = target;
        }
        
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
            if (_target == null)
                return;
            
            _context.RotateToLookAtPosition(_target.position, _lookAtTargetRotationSpeed);
        }

        private protected override State<Enemy> GetTransition()
        {
            if (_timer >= _duration)
            {
                _context.AttackState.SetLockOnTarget(_target);
                return _context.AttackState;
            }

            return null;
        }
    }
}