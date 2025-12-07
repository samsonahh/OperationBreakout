using Sirenix.OdinInspector;
using UnityEngine;

public class HitNumberSpawner : MonoBehaviour
{
    [SerializeField] private HitNumber _hitNumberPrefab;
    [SerializeField] private Vector2 _spawnOffset;
    
    [Header("Random Offset")]
    [SerializeField] private bool _useRandomOffset = true;
    [SerializeField, ShowIf("_useRandomOffset")] private float _offsetRadius = 0.25f;

    public void SpawnHitNumber(Vector2 position, float damageNumber)
    {
        HitNumber newHitNumber = Instantiate(_hitNumberPrefab, position + _spawnOffset, Quaternion.identity, null);
        newHitNumber.Init(damageNumber);
    }

    public void SpawnHitNumberOnSelf(float damageNumber)
    {
        if (_useRandomOffset)
        {
            Vector2 randomOffset = Random.insideUnitCircle.normalized * _offsetRadius;
            SpawnHitNumber((Vector2)transform.position + randomOffset, damageNumber);
        }
        else
        {
            SpawnHitNumber(transform.position, damageNumber);
        }
    }

    [Button("Test Spawn")]
    public void TestSpawn()
    {
        SpawnHitNumberOnSelf(999f);
    }
}
