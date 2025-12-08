using System;
using UnityEngine;

public class Ticker : Singleton<Ticker>
{
    [field: SerializeField] public float Interval { get; private set; } = 0.5f;
    
    public event Action OnTick = delegate { };

    private float _timer;

    private protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        _timer += Time.unscaledDeltaTime;
        if (_timer >= Interval)
        {
            _timer = 0f;
            OnTick?.Invoke();
        }
    }
}