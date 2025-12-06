using LBG;
using UnityEngine;

namespace EnemyStates
{
    [System.Serializable]
    [SubclassPath(SubClassName = "Spin")]
    public class SpinIdleState : IdleState
    {
        [SerializeField] private float _spinSpeed = 1f;
        
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
            
        }

        private protected override State<Enemy> GetTransition()
        {
            return null;
        }
    }
}