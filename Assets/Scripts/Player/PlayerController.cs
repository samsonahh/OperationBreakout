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

    [Header("Config")]
    [SerializeField] private float _rotationSpeed = 10f;
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
        _stateMachine = new StateMachine<PlayerController>(this);
        
        IdleState.Init(_stateMachine, this);
        MoveState.Init(_stateMachine, this);
        
        _stateMachine.ChangeState(IdleState, true);
    }

    public void RotateToMousePosition()
    {
        // Get mouse pos
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = worldMousePosition - transform.position;
        
        // Calculate target angle
        float targetAngle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        // Build rotation
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        // Smoothly rotate
        RigidBody.MoveRotation(Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            _rotationSpeed * Time.deltaTime
        ));

        ForwardDirection = targetRotation * Vector2.up;
    }
}
