using LBG;
using UnityEngine;

namespace EnemyStates
{
    [System.Serializable]
    public abstract class PatrolState : State<Enemy>
    {
        [SerializeField] private protected AnimationClip _animationClip;
    }
}