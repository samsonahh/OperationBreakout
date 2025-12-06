using LBG;
using UnityEngine;

namespace EnemyStates
{
    [System.Serializable]
    [SubclassPath("Melee", "Swing")]
    public class MeleeSwingAttackState : AttackState
    {
        [SerializeField] private float _swingAngle = 10f;
        
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