using System;
using UnityEngine;
using EnemyStates;
using LBG;
using Sirenix.OdinInspector;

public class Enemy : MonoBehaviour
{
    [field: SerializeField] public Rigidbody2D RigidBody { get; private set; }
    [field: SerializeField] public CircleCollider2D Collider { get; private set; }
    
    private StateMachine<Enemy> _stateMachine;
    
    [field: TabGroup("States", "Idle")]
    [field: SerializeReference, SubclassSelector]
    public IdleState IdleState { get; private set; }
    
    [field: TabGroup("States", "Patrol")]
    [field: SerializeReference, SubclassSelector]
    public PatrolState PatrolState { get; private set; }
    
    [field: TabGroup("States", "Attack")]
    [field: SerializeReference, SubclassSelector]
    public AttackState AttackState { get; private set; }
    
    public Vector2 ForwardDirection { get; private set; }
    
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
    
    public void RotateToLookAtPosition(Vector3 position, float rotationSpeed)
    {
        Vector2 lookAtDirection = position - transform.position;
        
        // Calculate target angle
        float targetAngle = Mathf.Atan2(lookAtDirection.y, lookAtDirection.x) * Mathf.Rad2Deg;

        // Build rotation
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, targetAngle), rotationSpeed * Time.fixedDeltaTime);
        transform.rotation = targetRotation;

        ForwardDirection = targetRotation * Vector2.right;
    }
}