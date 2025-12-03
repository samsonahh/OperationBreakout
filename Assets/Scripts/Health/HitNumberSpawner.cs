using NaughtyAttributes;
using UnityEngine;

public class HitNumberSpawner : MonoBehaviour
{
    [SerializeField] private HitNumber _hitNumberPrefab;
    [SerializeField] private Vector2 _spawnOffset;

    public void SpawnHitNumber(Vector2 position, float damageNumber)
    {
        HitNumber newHitNumber = Instantiate(_hitNumberPrefab, position + _spawnOffset, Quaternion.identity, null);
        newHitNumber.Init(damageNumber);
    }

    public void SpawnHitNumberOnSelf(float damageNumber)
    {
        SpawnHitNumber(transform.position, damageNumber);
    }

    [Button("Test Spawn")]
    public void TestSpawn()
    {
        SpawnHitNumberOnSelf(999f);
    }
}
