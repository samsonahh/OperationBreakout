using System;
using Animancer;
using UnityEngine;
using PlayerStates;
using Sirenix.OdinInspector;

public class PlayerController : MonoBehaviour, ITeam
{
    [field: Header("References")]
    [field: SerializeField] public Rigidbody2D RigidBody { get; private set; }
    [field: SerializeField] public CircleCollider2D HitBoxCollider { get; private set; }
    
    public Team Team { get; set; } = Team.Player;
    
    private StateMachine<PlayerController> _stateMachine;

    [field: TabGroup("States", "Idle")]
    [field: SerializeField] public IdleState IdleState { get; private set; } = new();
    [field: TabGroup("States", "Move")]
    [field: SerializeField] public MoveState MoveState { get; private set; } = new();
    [field: TabGroup("States", "Dash")]
    [field: SerializeField] public DashState DashState { get; private set; } = new();

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
        
        DashState.UpdateDashCooldown(Time.deltaTime);
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
        DashState.Init(_stateMachine, this);
        
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
        transform.rotation = targetRotation;
        
        ForwardDirection = targetRotation * Vector2.right;
    }
}
