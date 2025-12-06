using LBG;
using UnityEngine;

namespace EnemyStates
{
    [System.Serializable]
    [SubclassPath(SubClassName = "Basic")]
    public class BasicIdleState : IdleState
    {
        [SerializeField] private float _basic;
        
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