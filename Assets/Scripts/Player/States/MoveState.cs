using UnityEngine;

namespace PlayerStates
{
    [System.Serializable]
    public class MoveState : State<PlayerController>
    {
        [field: SerializeField] public float Speed { get; private set; } = 5f;

        private protected override void OnEnter()
        {
            InputManager.Instance.Dash += InputManager_Dash;
        }

        private protected override void OnExit()
        {
            InputManager.Instance.Dash -= InputManager_Dash;
        }

        private protected override void OnUpdate()
        {
            _context.RotateToMousePosition();
        }

        private protected override void OnFixedUpdate()
        {
            Vector3 moveDelta = Speed * Time.fixedDeltaTime * InputManager.Instance.MoveDirection;
            _context.RigidBody.MovePosition(_context.transform.position + moveDelta);
        }
        
        private protected override State<PlayerController> GetTransition()
        {
            if (InputManager.Instance.MoveDirection == Vector2.zero)
                return _context.IdleState;
            
            return null;
        }

        private void InputManager_Dash()
        {
            _context.DashState.TryDash();
        }
    }
}