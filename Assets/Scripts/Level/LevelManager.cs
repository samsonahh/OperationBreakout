using System;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private float _startTimer;

    public float CurrentTimer { get; private set; } = 0f;
    public bool IsTimerDepleted { get; private set; }
    public Action<float> OnTimerAdded = delegate { };
    public Action<float> OnTimerSubtracted = delegate { };
    public Action OnTimerDepleted = delegate { };

    private void Start()
    {
        CurrentTimer = _startTimer;
    }

    private void Update()
    {
        CountdownTimer(Time.deltaTime);
    }

    private void CountdownTimer(float deltaTime)
    {
        if (IsTimerDepleted)
            return;
        
        CurrentTimer -= deltaTime;
        if (CurrentTimer <= 0)
        {
            IsTimerDepleted = true;
            CurrentTimer = 0f;
            
            HandleTimerDepleted();
        }
    }

    private void HandleTimerDepleted()
    {
        GameManager.Instance.ChangeGameState(GameState.Results);
        OnTimerDepleted.Invoke();
    }

    public void SubtractTimer(float subtractionAmount)
    {
        CountdownTimer(subtractionAmount);
        OnTimerSubtracted.Invoke(subtractionAmount);
    }

    public void AddTimer(float additionAmount)
    {
        CountdownTimer(-additionAmount);
        OnTimerAdded.Invoke(additionAmount);
    }
}
