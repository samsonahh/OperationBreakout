using NaughtyAttributes;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [field: Header("Config")]
    [field: SerializeField] public float MaxHealth { get; private set; } = 10f;
    [field: SerializeField, ReadOnly] public float CurrentHealth { get; private set; } = 0f;

    public bool IsDead { get; private set; }
    
    public event Action<float> OnHealthChanged = delegate { };
    public event Action OnDeath = delegate { };

    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead)
            return;
        
        CurrentHealth -= damage;
        
        OnHealthChanged.Invoke(CurrentHealth);
        
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
