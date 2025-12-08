using Pathfinding;
using UnityEngine;

public class Room : MonoBehaviour
{
    [field: SerializeField] public Transform EntranceTransform { get; private set; }
    [field: SerializeField] public Transform ExitTransform { get; private set; }
    [field: SerializeField] public BoxCollider2D RoomRectSpace { get; private set; }
    [SerializeField] private float _corridorWallThickness = 1f;
    [SerializeField] private float _corridorWidth = 2f;
    [SerializeField] private GameObject _corridorWallPrefab;
    public bool IsEntranceUsed { get; private set; }
    public bool IsExitUsed { get; private set; }
    public Room PreviousRoom { get; private set; }

    private void Start()
    {
        GenerateNavMesh();
    }

    public void GenerateNavMesh()
    {
        AstarData data = AstarPath.active.data;
        
        GridGraph newGraph = data.AddGraph(typeof(GridGraph)) as GridGraph;
        if (newGraph == null)
        {
            Debug.LogError("Failed to make a new graph");
            return;
        }
        
        newGraph.is2D = true;
        
        newGraph.collision.use2D = true;
        newGraph.collision.diameter = 0.5f;
        newGraph.collision.mask = LayerMask.GetMask("Environment");

        newGraph.center = transform.position;
        newGraph.SetDimensions(Mathf.CeilToInt(RoomRectSpace.size.x), Mathf.CeilToInt(RoomRectSpace.size.y), 1);
        
        AstarPath.active.Scan(newGraph);
    }

    public void MarkEntranceUsed()
    {
        IsEntranceUsed = true;
    }

    public void MarkExitUsed()
    {
        IsExitUsed = true;
    }

    public void SetPreviousRoom(Room previousRoom)
    {
        PreviousRoom = previousRoom;
    }
    
    public void ConnectToPreviousRoom()
    {
        if (PreviousRoom == null)
        {
            Debug.LogError("Cannot connect: PreviousRoom is null.");
            return;
        }
        
        // 1. Get the connection points
        Vector3 exitPos = PreviousRoom.ExitTransform.position;
        Vector3 entrancePos = EntranceTransform.position;
        
        // 2. Calculate the center, distance, and direction of the connection
        
        Vector3 connectionCenter = Vector3.Lerp(exitPos, entrancePos, 0.5f);
        float distance = Vector3.Distance(exitPos, entrancePos); // Length of the path
        
        // We assume horizontal connection, so the rotation is zero.
        // If you plan for turns, you would need to calculate rotation based on the vector (entrancePos - exitPos).

        // 3. Generate Nav Mesh Graph for the Corridor
        GenerateCorridorNavMesh(connectionCenter, distance);
        
        // 4. Generate the Walls
        GenerateCorridorWalls(connectionCenter, distance, exitPos, entrancePos);
    }

    // Helper method for the Corridor Nav Mesh (using AstarPath logic)
    private void GenerateCorridorNavMesh(Vector3 center, float length)
    {
        AstarData data = AstarPath.active.data;
        
        // Add a new GridGraph instance
        GridGraph corridorGraph = data.AddGraph(typeof(GridGraph)) as GridGraph;
        if (corridorGraph == null)
        {
            Debug.LogError("Failed to make a new graph for the corridor");
            return;
        }
        
        corridorGraph.is2D = true;
        
        // Configure collision settings (matching your room settings)
        corridorGraph.collision.use2D = true;
        corridorGraph.collision.diameter = 0.5f;
        corridorGraph.collision.mask = LayerMask.GetMask("Environment");

        // Set the graph dimensions and center
        corridorGraph.center = center;
        
        // Length is X size, _corridorWidth is the Y size for the pathfinding area
        corridorGraph.SetDimensions(Mathf.CeilToInt(length), Mathf.CeilToInt(_corridorWidth), 1);
        
        // Scan the new graph
        AstarPath.active.Scan(corridorGraph);
    }

    // Helper method for the Corridor Walls
    private void GenerateCorridorWalls(Vector3 center, float length, Vector3 startPos, Vector3 endPos)
    {
        // Get the perpendicular direction (Up/Down) to place the walls
        Vector3 perpendicular = Vector3.up; 
        
        // 1. Calculate half the path width (the distance from center to the inside edge of the wall)
        float halfPathWidth = _corridorWidth / 2f;
        
        // 2. Wall Thickness (used for wall scale and position offset)
        float halfWallThickness = _corridorWallThickness / 2f;

        // Wall Height (as requested)
        const float wallHeight = 2f; 
        
        // Wall Size: The length matches the corridor length. Thickness is the Y-scale.
        Vector3 wallScale = new Vector3(length, _corridorWallThickness, 1f); 

        // --- Spawn Top Wall ---
        
        // Position: Center + (Half Path Width) + (Half Wall Thickness)
        // This places the wall's center just outside the pathway.
        float topOffset = halfPathWidth + halfWallThickness;
        Vector3 topWallPosition = center + perpendicular * topOffset;
        
        GameObject topWall = Instantiate(_corridorWallPrefab, topWallPosition, Quaternion.identity, transform);
        topWall.transform.localScale = wallScale;
        topWall.layer = LayerMask.NameToLayer("Environment");
        topWall.name = "CorridorWall_Top";
        
        // --- Spawn Bottom Wall ---
        
        // Position: Center - (Half Path Width) - (Half Wall Thickness)
        float bottomOffset = halfPathWidth + halfWallThickness;
        Vector3 bottomWallPosition = center - perpendicular * bottomOffset;
        
        GameObject bottomWall = Instantiate(_corridorWallPrefab, bottomWallPosition, Quaternion.identity, transform);
        bottomWall.transform.localScale = wallScale;
        bottomWall.layer = LayerMask.NameToLayer("Environment");
        bottomWall.name = "CorridorWall_Bottom";
    }
}
