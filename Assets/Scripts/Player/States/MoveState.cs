using UnityEngine;

namespace PlayerStates
{
    [System.Serializable]
    public class MoveState : State<PlayerController>
    {
        [SerializeField] private float _speed = 5f;
        
        private protected override void OnEnter()
        {
            
        }

        private protected override void OnExit()
        {
            
        }

        private protected override void OnUpdate()
        {
            
        }

        private protected override void OnFixedUpdate()
        {
            Vector3 moveDelta = _speed * Time.fixedDeltaTime * InputManager.Instance.MoveDirection;
            _context.RigidBody.MovePosition(_context.transform.position + moveDelta);
            
            _context.RotateToMousePosition();
        }
        
        private protected override State<PlayerController> GetTransition()
        {
            if (InputManager.Instance.MoveDirection == Vector2.zero)
                return _context.IdleState;
            
            return null;
        }
    }
}