using System;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidBody;

    private Team _team;
    private float _shootSpeed;
    private Vector2 _shootDirection;
    private float _damage;
    
    public void Init(Team team, float shootSpeed, Vector2 shootDirection, float lifespan, float damage)
    {
        _team = team;
        _shootSpeed = shootSpeed;
        _shootDirection = shootDirection;
        _damage = damage;
        FaceShootDirection();
        
        Destroy(gameObject, lifespan);
    }

    private void FixedUpdate()
    {
        _rigidBody.MovePosition(_rigidBody.position + _shootDirection * (_shootSpeed * Time.fixedDeltaTime));
    }

    private void FaceShootDirection()
    {
        if (_shootDirection == Vector2.zero)
            return;

        float angle = Mathf.Atan2(_shootDirection.y, _shootDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleOnHitHealth(other);
    }

    private void HandleOnHitHealth(Collider2D other)
    {
        Health health = other.GetComponentInParent<Health>();
        if (health == null)
            return;

        ITeam teamComponent = other.GetComponentInParent<ITeam>();
        if (teamComponent == null)
            return;

        if (teamComponent.Team == _team)
            return;
        
        health.TakeDamage(_damage);
        Explode();
    }
    
    private void Explode()
    {
        Destroy(gameObject);
    }
}
