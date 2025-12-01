using UnityEngine;

namespace PlayerStates
{
    [System.Serializable]
    public class IdleState : State<PlayerController>
    {
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
            _context.RotateToMousePosition();
        }

        private protected override State<PlayerController> GetTransition()
        {
            if (InputManager.Instance.MoveDirection != Vector2.zero)
                return _context.MoveState;
            
            return null;
        }
    }
}