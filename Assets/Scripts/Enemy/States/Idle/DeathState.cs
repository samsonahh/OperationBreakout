using UnityEngine;

namespace EnemyStates
{
    [System.Serializable]
    public class DeathState : State<Enemy>
    {
        [SerializeField] private AnimationClip _animationClip;

        private float _duration;
        private float _timer;

        private protected override void OnInit()
        {
            _duration = _animationClip.length;
        }

        private protected override void OnEnter()
        {
            _context.Animator.Play(_animationClip);
            
            _timer = 0f;
        }

        private protected override void OnExit()
        {
            
        }

        private protected override void OnUpdate()
        {
            _timer += Time.deltaTime;
            
            if(_timer >= _duration)
                GameObject.Destroy(_context.gameObject);
        }

        private protected override void OnFixedUpdate()
        {
            
        }
    }
}