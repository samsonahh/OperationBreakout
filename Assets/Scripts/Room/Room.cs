using System;
using Pathfinding;
using UnityEngine;

public class Room : MonoBehaviour
{
    [field: SerializeField] public Transform EntranceTransform { get; private set; }
    [field: SerializeField] public Transform ExitTransform { get; private set; }
    [field: SerializeField] public BoxCollider2D RoomRectSpace { get; private set; }

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
}
