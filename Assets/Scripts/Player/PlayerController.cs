using System;
using UnityEngine;
using PlayerStates;

public class PlayerController : MonoBehaviour
{
    [field: SerializeField] public Rigidbody2D RigidBody { get; private set; }
    [field: SerializeField] public CircleCollider2D Collider { get; private set; }
    
    private StateMachine<PlayerController> _stateMachine;

    [field: Header("States")]
    [field: SerializeField] public IdleState IdleState { get; private set; } = new();
    [field: SerializeField] public MoveState MoveState { get; private set; } = new();

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
        _stateMachine = new StateMachine<PlayerController>(this);
        
        IdleState.Init(_stateMachine, this);
        MoveState.Init(_stateMachine, this);
        
        _stateMachine.ChangeState(IdleState, true);
    }
}
