using LBG;
using UnityEngine;

namespace EnemyStates
{
    [System.Serializable]
    public abstract class AttackState : State<Enemy>
    {
        private protected Transform _target;
        
        public void SetLockOnTarget(Transform target) => _target = target;
    }
}