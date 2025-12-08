using LBG;
using UnityEngine;

namespace EnemyStates
{
    [System.Serializable]
    [SubclassPath("Melee", "Bite")]
    public class MeleeBiteAttackState : AttackState
    {
        [SerializeField] private Vector2 _biteOffset = new Vector2(0.5f, 0f);
        [SerializeField] private float _biteRadius = 0.5f;
        [SerializeField] private float _damage = 1f;
        [SerializeField] private float _cooldown = 1f;

        private float _timer;
        
        private protected override void OnEnter()
        {
            _timer = 0f;
            
            Collider2D hit = Physics2D.OverlapCircle((Vector2)_context.transform.position + _context.ForwardDirection * _biteOffset, _biteRadius,
                LayerMask.GetMask("Hitbox"));

            if (hit == null)
                return;
            
            Health health = hit.GetComponentInParent<Health>();
            if (health == null)
                return;
            ITeam team = hit.GetComponentInParent<ITeam>();
            if (team == null)
                return;

            if (team.Team == _context.Team)
                return;
            
            health.TakeDamage(_damage);
        }

        private protected override void OnExit()
        {
            
        }

        private protected override void OnUpdate()
        {
            _timer += Time.deltaTime;
        }

        private protected override void OnFixedUpdate()
        {
            
        }

        private protected override State<Enemy> GetTransition()
        {
            if (_timer >= _cooldown)
                return _context.IdleState;
            
            return null;
        }
    }
}