using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [field: Header("Config")]
    [field: SerializeField] public float MaxHealth { get; private set; } = 10f;
    [field: SerializeField, ReadOnly] public float CurrentHealth { get; private set; } = 0f;

    public bool IsDead { get; private set; }

    public UnityEvent<float> OnDamageTaken = new();
    public UnityEvent OnDeath = new();

    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead)
            return;
        
        CurrentHealth -= damage;
        
        OnDamageTaken.Invoke(damage);
        
        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            IsDead = true;
            OnDeath.Invoke();
        }
    }

    [Button("Take One Damage")]
    public void TakeOneDamageTest()
    {
        TakeDamage(1f);
    }
}
