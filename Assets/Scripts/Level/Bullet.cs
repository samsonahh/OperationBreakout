using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidBody;
    
    private float _shootSpeed;
    private Vector2 _shootDirection;
    
    public void Init(float shootSpeed, Vector2 shootDirection, float lifespan)
    {
        _shootSpeed = shootSpeed;
        _shootDirection = shootDirection;
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
}
