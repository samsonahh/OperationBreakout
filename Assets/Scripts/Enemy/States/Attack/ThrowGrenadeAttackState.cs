using LBG;
using UnityEngine;

namespace EnemyStates
{
    [System.Serializable]
    [SubclassPath("Throw", "Grenade")]
    public class ThrowGrenadeAttackState : AttackState
    {
        [SerializeField] private float _grenadeInitialSpeed = 5f;
        
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