using System;
using UnityEngine;
using EnemyStates;
using LBG;
using Sirenix.OdinInspector;

public class Enemy : MonoBehaviour
{
    private StateMachine<Enemy> _stateMachine;

    [field: Header("States")]
    [field: SerializeReference, SubclassSelector] public IdleState IdleState { get; private set; }
    [field: SerializeReference, SubclassSelector] public PatrolState PatrolState { get; private set; }
    [field: SerializeReference, SubclassSelector] public AttackState AttackState { get; private set; }
    
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