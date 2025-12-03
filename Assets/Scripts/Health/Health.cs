using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [field: Header("Config")]
    [field: SerializeField] public float MaxHealth { get; private set; } = 10f;
    [field: SerializeField, ReadOnly] public float CurrentHealth { get; private set; } = 0f;

    public bool IsDead { get; private set; }
    public bool IsInvincible { get; private set; }

    public UnityEvent<float> OnDamageTaken = new();
    public UnityEvent<bool> OnInvincible = new();
    public UnityEvent OnDeath = new();

    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead)
            return;

        if (IsInvincible)
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

    /// <summary>
    /// This is for health that doesn't need to go down but still needs the damage taking events.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeFakeDamage(float damage)
    {
        if (IsInvincible)
            return;
        
        OnDamageTaken.Invoke(damage);
    }

    [Button("Take One Damage")]
    public void TakeOneDamageTest()
    {
        TakeDamage(1f);
    }

    public void SetInvincibility(bool invincibility)
    {
        IsInvincible = invincibility;
        OnInvincible.Invoke(IsInvincible);
    }
}
