using System;
using UnityEngine;
using EnemyStates;

public class Enemy : MonoBehaviour
{
    private StateMachine<Enemy> _stateMachine;

    [field: Header("States")]
    [field: SerializeField] public IdleState IdleState { get; private set; } = new();
    [field: SerializeField] public PatrolState PatrolState { get; private set; } = new();
    [field: SerializeField] public AttackState AttackState { get; private set; } = new();
    
    private void Awake()
    {
        SetupStateMachine();
    }

    private void OnDestroy()
    {
        _stateMachine?.Destroy();
    }

    private void Update()
    {
        _stateMachine?.Update();
    }

    private void FixedUpdate()
    {
        _stateMachine?.FixedUpdate();
    }

    private void SetupStateMachine()
    {
        _stateMachine = new StateMachine<Enemy>(this);
        
        IdleState.Init(_stateMachine, this);
        PatrolState.Init(_stateMachine, this);
        AttackState.Init(_stateMachine, this);
        
        _stateMachine.ChangeState(IdleState, true);
    }
}