using UnityEngine;

[System.Serializable]
public class BulletSpawner
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _spawnPoint;

    public Bullet Spawn(Vector2 forwardDirection, float speed, float lifespan, float damage)
    {
        Bullet bullet = GameObject.Instantiate(_bulletPrefab, _spawnPoint.position, Quaternion.identity);
        bullet.Init(speed, forwardDirection, lifespan, damage);

        return bullet;
    }
}