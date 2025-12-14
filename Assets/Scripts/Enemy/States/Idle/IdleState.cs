using LBG;
using UnityEngine;

namespace EnemyStates
{
    [System.Serializable]
    public abstract class IdleState : State<Enemy>
    {
        [SerializeField] private protected AnimationClip _animationClip;
    }
}