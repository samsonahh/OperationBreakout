using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EnemyStates;
using LBG;
using Pathfinding;
using Sirenix.OdinInspector;
using Random = System.Random;

public class Enemy : MonoBehaviour, ITeam
{
    [field: Header("References")]
    [field: SerializeField] public Rigidbody2D RigidBody { get; private set; }
    [field: SerializeField] public CircleCollider2D HitBoxCollider { get; private set; }
    [field: SerializeField] public AIPath AIPath { get; private set; }
    [field: SerializeField] public Seeker Seeker { get; private set; }

    public Team Team { get; set; } = Team.Enemy;
    
    private StateMachine<Enemy> _stateMachine;
    
    [field: TabGroup("States", "Idle")]
    [field: SerializeReference, SubclassSelector]
    public IdleState IdleState { get; private set; }
    
    [field: TabGroup("States", "Patrol")]
    [field: SerializeReference, SubclassSelector]
    public PatrolState PatrolState { get; private set; }
    
    [field: TabGroup("States", "Chase")]
    [field: SerializeField]
    public ChaseState ChaseState { get; private set; }
    
    [field: TabGroup("States", "Pre Attack")]
    [field: SerializeField]
    public AttackWindupState AttackWindupState { get; private set; }
    
    [field: TabGroup("States", "Attack")]
    [field: SerializeReference, SubclassSelector]
    public AttackState AttackState { get; private set; }
    
    public Vector2 ForwardDirection { get; private set; }
    
    [field: Header("Detection")]
    [field: SerializeField] public float DetectionConeAngle { get; private set; } = 45f;
    [field: SerializeField] public int DetectionConeSteps { get; private set; } = 16;
    [field: SerializeField] public float DetectionMaxDistance { get; private set; } = 5f;
    public Transform CurrentTarget { get; private set; }
    
    private void Awake()
    {
        SetupStateMachine();

        Ticker.Instance.OnTick += Ticker_OnTick;
    }

    private void OnDestroy()
    {
        _stateMachine?.Destroy();
        
        if(Ticker.Instance != null)
            Ticker.Instance.OnTick -= Ticker_OnTick;
    }

    private void Update()
    {
        _stateMachine?.Update();
    }

    private void FixedUpdate()
    {
        _stateMachine?.FixedUpdate();
    }

    private void OnDrawGizmosSelected()
    {
        float halfAngle = DetectionConeAngle / 2f;
        float angleIncrement = DetectionConeAngle / (2 * DetectionConeSteps);
        float startAngle = -halfAngle;

        for (int i = 0; i <= (2 * DetectionConeSteps); i++)
        {
            float currentAngle = startAngle + (i * angleIncrement);
            // *** FIX: Use 2D direction helper ***
            Vector2 rayDirection = GetRayDirectionFromAngle2D(currentAngle); 
        
            Gizmos.color = Color.red;
        
            // *** FIX: Draw a 2D line (Vector2 is implicitly converted to Vector3 with z=0) ***
            Vector3 startPosition = transform.position;
            Vector3 endPosition = startPosition + (Vector3)rayDirection * DetectionMaxDistance;
        
            Gizmos.DrawLine(startPosition, endPosition);
        }
    }
    
    private void SetupStateMachine()
    {
        _stateMachine = new StateMachine<Enemy>(this);
        
        IdleState.Init(_stateMachine, this);
        PatrolState.Init(_stateMachine, this);
        ChaseState.Init(_stateMachine, this);
        AttackWindupState.Init(_stateMachine, this);
        AttackState.Init(_stateMachine, this);
        
        _stateMachine.ChangeState(IdleState, true);
    }

    private void Ticker_OnTick()
    {
        CurrentTarget = FindClosestTarget();
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

    public Vector3 GetRandomWalkableTarget(float _radius)
    {
        if (AstarPath.active == null)
        {
            Debug.LogError("AstarPath is not active.");
            return Vector3.zero;
        }

        // 1. Pick a raw random point in world space
        Vector3 rawTarget = (Vector3)UnityEngine.Random.insideUnitCircle.normalized * _radius + transform.position;
    
        // 2. Find the nearest node info to that raw point
        // The default GetNearest search is usually constrained enough for this purpose.
        NNInfo info = AstarPath.active.GetNearest(rawTarget);

        // 3. Return the position of the nearest *valid* node
        // This position is guaranteed to be on a walkable node, unless the graph is tiny.
        if (info.node != null && info.node.Walkable)
        {
            return info.position;
        }
        else 
        {
            // Fallback: If for some reason the nearest node is not walkable (rare, but possible
            // if the graph is very constrained), you might return the current position 
            // or re-run the check.
            Debug.LogWarning("Nearest node was not found or was unwalkable. Retrying.");
            return transform.position; // Return current position as safe fallback
        }
    }

    public Transform FindClosestTarget()
    {
        return FindClosestTargetInSteppedCone(DetectionMaxDistance, DetectionConeAngle, DetectionConeSteps);
    }
    
    public Transform FindClosestTargetInSteppedCone(float radius, float coneAngle, int stepsPerSide)
    {
        List<Transform> detectedTargets = new List<Transform>();

        // Calculate the half-angle of the cone (measured from the center line)
        float halfAngle = coneAngle / 2f;
    
        // Determine the angle between each Raycast step
        // The total number of steps is (2 * stepsPerSide) + 1 (for the center ray)
        float angleIncrement = coneAngle / (2 * stepsPerSide);

        // Start with the angle on the far left of the cone sweep
        float startAngle = -halfAngle;

        for (int i = 0; i <= (2 * stepsPerSide); i++)
        {
            float currentAngle = startAngle + (i * angleIncrement);
            Vector2 rayDirection = GetRayDirectionFromAngle2D(currentAngle);
            
            LayerMask layerMask = LayerMask.GetMask("Player", "Enemy", "Environment");
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDirection, radius, layerMask);
        
            if(hits.Length == 0)
                continue;

            foreach (RaycastHit2D hit in hits)
            {
                if(hit.collider == null)
                    continue;
                
                Transform hitTransform = hit.collider.transform;
                if(Utils.IsSelfOrDescendantOf(hitTransform, transform))
                    continue;
                
                if (!hitTransform.TryGetComponent(out ITeam teamComponent))
                    break;
                
                if(teamComponent.Team == Team)
                    continue;

                detectedTargets.Add(hitTransform);
                break;
            }
        }
        
        return detectedTargets
            .Distinct() // Remove duplicates (if multiple rays hit the same target)
            .OrderBy(target => Vector3.Distance(transform.position, target.position)) // Sort by distance
            .FirstOrDefault(); // Return the closest one, or null
    }
    
    private Vector2 GetRayDirectionFromAngle2D(float angleInDegrees)
    {
        // The rotation is based on the object's current rotation (transform.rotation), 
        // which in 2D is a rotation around Z. We use Euler angles to get the current Z rotation.
        float baseAngle = transform.eulerAngles.z; 

        // Unity's 2D rotation is counter-clockwise, and 0 degrees is typically along the positive X-axis.
        // However, the enemy's rotation is likely set such that ForwardDirection is correct.
        // We add the cone angle to the current base Z rotation.
        float finalAngle = baseAngle + angleInDegrees;

        // Convert angle to radians for trigonometric functions
        float angleInRadians = finalAngle * Mathf.Deg2Rad;

        // Calculate the 2D direction vector (X = cos, Y = sin)
        return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
    }
    
    public void Kill()
    {
        Destroy(gameObject);
    }
}