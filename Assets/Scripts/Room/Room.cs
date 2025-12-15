using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Room : MonoBehaviour
{
    [field: SerializeField] public Transform EntranceTransform { get; private set; }
    [field: SerializeField] public Transform ExitTransform { get; private set; }
    [field: SerializeField] public BoxCollider2D RoomRectSpace { get; private set; }
    [SerializeField] private int _corridorWallThickness = 1;
    [SerializeField] private int _corridorWidth = 1;
    [SerializeField] private GameObject _corridorWallPrefab;
    public bool IsEntranceUsed { get; private set; }
    public bool IsExitUsed { get; private set; }
    public Room PreviousRoom { get; private set; }
    
    private GridGraph _graph;
    
    [Header("Spawn")]
    [SerializeField] private GameObject _winTriggerPrefab;
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private Dr _doctorPrefab;
    [SerializeField] private int _enemyCount = 3;
    [SerializeField] private Transform _spawnPointsTransform;
    private HashSet<int> _usedSpawnPoints = new();
    private List<Enemy> _spawnedEnemies = new();

    private void Start()
    {
        GenerateNavMesh();
        SpawnEnemies();
    }

    public void GenerateNavMesh()
    {
        AstarData data = AstarPath.active.data;
        
        _graph = data.AddGraph(typeof(GridGraph)) as GridGraph;
        if (_graph == null)
        {
            Debug.LogError("Failed to make a new graph");
            return;
        }
        
        _graph.is2D = true;
        
        _graph.collision.use2D = true;
        _graph.collision.diameter = 0.5f;
        _graph.collision.mask = LayerMask.GetMask("Environment");

        _graph.center = transform.position;
        _graph.SetDimensions(Mathf.CeilToInt(RoomRectSpace.size.x), Mathf.CeilToInt(RoomRectSpace.size.y), 1);
        
        AstarPath.active.Scan(_graph);
    }

    public void SpawnEnemies()
    {
        _usedSpawnPoints.Clear();
        
        int spawnCount = Mathf.Min(_enemyCount, _spawnPointsTransform.childCount);

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPos = GetRandomSpawnPoint();
            Enemy spawnedEnemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
            _spawnedEnemies.Add(spawnedEnemy);
        }
    }

    public void ClearEnemies()
    {
        for (int i = 0; i < _spawnedEnemies.Count; i++)
        {
            if (_spawnedEnemies[i] != null)
            {
                Destroy(_spawnedEnemies[i].gameObject);
            }
        }
        _spawnedEnemies.Clear();
    }

    public void SpawnDoctor()
    {
        Vector3 spawnPosition = GetRandomSpawnPoint();
        Instantiate(_doctorPrefab, spawnPosition, Quaternion.identity);
    }
    
    public void MarkEntranceUsed()
    {
        IsEntranceUsed = true;
        EntranceTransform.gameObject.SetActive(false);
    }

    public void MarkExitUsed()
    {
        IsExitUsed = true;
        ExitTransform.gameObject.SetActive(false);
    }

    /// <summary>
    /// Creates the win conditions at entrance
    /// </summary>
    public void CreateFinishExitAtEntrance()
    {
        MarkEntranceUsed();
        Instantiate(_winTriggerPrefab, EntranceTransform.position, Quaternion.identity);
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
        
        // Wall Size: The length matches the corridor length. Thickness is the Y-scale.
        Vector3 wallScale = new Vector3(length - 1f, _corridorWallThickness, 1f); 

        // --- Spawn Top Wall ---
        
        // Position: Center + (Half Path Width) + (Half Wall Thickness)
        // This places the wall's center just outside the pathway.
        float topOffset = halfPathWidth + halfWallThickness;
        Vector3 topWallPosition = center + perpendicular * topOffset;
        
        GameObject topWall = Instantiate(_corridorWallPrefab, topWallPosition, Quaternion.identity, transform);
        topWall.GetComponent<SpriteRenderer>().size = wallScale;
        topWall.GetComponent<BoxCollider2D>().size = wallScale;
        topWall.layer = LayerMask.NameToLayer("Environment");
        topWall.name = "CorridorWall_Top";
        
        // --- Spawn Bottom Wall ---
        
        // Position: Center - (Half Path Width) - (Half Wall Thickness)
        float bottomOffset = halfPathWidth + halfWallThickness;
        Vector3 bottomWallPosition = center - perpendicular * bottomOffset;
        
        GameObject bottomWall = Instantiate(_corridorWallPrefab, bottomWallPosition, Quaternion.identity, transform);
        bottomWall.GetComponent<SpriteRenderer>().size = wallScale;
        bottomWall.GetComponent<BoxCollider2D>().size = wallScale;
        bottomWall.layer = LayerMask.NameToLayer("Environment");
        bottomWall.name = "CorridorWall_Bottom";
    }

    private Vector3 GetRandomSpawnPoint()
    {
        int spawnPointCount = _spawnPointsTransform.childCount;

        if (_usedSpawnPoints.Count >= spawnPointCount)
        {
            Debug.LogWarning("All spawn points have been used.");
            return _spawnPointsTransform.GetChild(0).position;
        }

        int index;
        do
        {
            index = Random.Range(0, spawnPointCount);
        }
        while (_usedSpawnPoints.Contains(index));

        _usedSpawnPoints.Add(index);
        return _spawnPointsTransform.GetChild(index).position;
    }
}
