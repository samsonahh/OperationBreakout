using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int _roomCount = 5;
    [SerializeField] private float _roomMinDistance = 3;
    [SerializeField] private float _roomMaxDistance = 6;
    [SerializeField] private List<Room> _roomPrefabs = new();
    
    private List<Room> _spawnedRooms = new();
    
    private void Start()
    {
        Room startRoom = SpawnRoom(Vector2.zero);
        _spawnedRooms.Add(startRoom);

        for (int i = 1; i < _roomCount; i++)
        {
            if (TrySpawnAndConnectNewRoom())
            {
                Debug.Log($"Successfully placed and connected room {i}.");
            }
            else
            {
                Debug.LogError($"Failed to place a room after {i} attempts.");
                break; // Stop if placement fails too many times
            }
        }
        
        // Final scan
        AstarPath.active.Scan();
    }
    
    private Room SpawnRoom(Vector3 position)
    {
        // Select a random room prefab
        Room roomPrefab = _roomPrefabs[UnityEngine.Random.Range(0, _roomPrefabs.Count)];
    
        // Instantiate it at the given position
        Room newRoom = Instantiate(roomPrefab, position, Quaternion.identity);
        return newRoom;
    }
    
    private bool TrySpawnAndConnectNewRoom()
    {
        const int maxAttempts = 10;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // 1. Randomly pick an **existing** room to connect to
            Room existingRoom = _spawnedRooms[UnityEngine.Random.Range(0, _spawnedRooms.Count)];
            
            // 2. Randomly pick an **open Exit** from the existing room
            Transform connectionExit = existingRoom.ExitTransform; // Assumes implementation in Room.cs
            if (connectionExit == null) continue;

            // 3. Spawn the new room
            Room newRoom = SpawnRoom(Vector3.zero);

            // 4. Randomly pick an **Entrance** from the new room to connect
            Transform connectionEntrance = newRoom.EntranceTransform; // Assumes implementation in Room.cs
            if (connectionEntrance == null)
            {
                Destroy(newRoom.gameObject);
                continue;
            }

            // 5. Determine the required spacing
            float randomSpacing = UnityEngine.Random.Range(_roomMinDistance, _roomMaxDistance);
            
            // 6. Calculate the new room's desired position.

            // A. Get the necessary offset from the new room's center to its Entrance point.
            // This is needed to calculate the room's center based on the final Entrance X position.
            Vector3 entranceOffsetFromCenter = connectionEntrance.localPosition; 
            
            // B. Calculate the width required for the offset plus spacing.
            float newRoomHalfWidth = newRoom.RoomRectSpace.size.x / 2f;
            
            // C. Calculate the **Target Center X Position**
            
            // Start X at the existing room's exit position (connectionExit.position.x)
            float targetX = connectionExit.position.x;
            
            // Add the buffer: (Half the new room's width) + (the random spacing)
            float totalShiftDistance = newRoomHalfWidth + randomSpacing;
            
            // Move the X position by the total shift
            targetX += totalShiftDistance;
            
            // D. Calculate the **Target Center Y Position** (Crucial Step)
            
            // We want the new room's entrance Y position to match the connectionExit.y.
            // NewRoomCenterY = connectionExit.y - (NewRoomEntranceY - NewRoomCenterY)
            float targetY = connectionExit.position.y - entranceOffsetFromCenter.y;
            
            // 7. Set the final new room position
            Vector3 newRoomPosition = new Vector3(targetX, targetY, 0f); // Set Z to 0 for 2D
            newRoom.transform.position = newRoomPosition;

            // --- FINAL CHECK AND CLEANUP ---
            
            // 8. **Overlap Check (Crucial!):**
            if (!IsOverlapping(newRoom)) 
            {
                _spawnedRooms.Add(newRoom);
    
                // Set the connection data
                newRoom.SetPreviousRoom(existingRoom); // Use the existingRoom we connected to
    
                // Perform the connection (Nav Mesh and Walls)
                newRoom.ConnectToPreviousRoom(); 

                existingRoom.MarkExitUsed(); 
                newRoom.MarkEntranceUsed();
                return true; // Success!
            }
            else
            {
                // If overlapping, destroy the room and try again
                Destroy(newRoom.gameObject);
            }
        }

        return false;
    }
    
    private bool IsOverlapping(Room newRoom)
    {
        // Assuming rooms have a size property (Width/Height)
        float minDistancePadding = 1f; // Add a buffer
    
        foreach (Room existingRoom in _spawnedRooms)
        {
            // Get half sizes
            Vector2 existingHalfSize = new Vector2(existingRoom.RoomRectSpace.size.x / 2f, existingRoom.RoomRectSpace.size.y / 2f);
            Vector2 newHalfSize = new Vector2(newRoom.RoomRectSpace.size.x / 2f, newRoom.RoomRectSpace.size.y / 2f);
        
            // Use AABB (Axis-Aligned Bounding Box) check
        
            // Distance between centers
            float xDistance = Mathf.Abs(newRoom.transform.position.x - existingRoom.transform.position.x);
            float yDistance = Mathf.Abs(newRoom.transform.position.y - existingRoom.transform.position.y);

            // Minimum required distance to NOT overlap
            float requiredXDistance = existingHalfSize.x + newHalfSize.x + minDistancePadding;
            float requiredYDistance = existingHalfSize.y + newHalfSize.y + minDistancePadding;

            if (xDistance < requiredXDistance && yDistance < requiredYDistance)
            {
                // Overlap detected!
                return true;
            }
        }
        return false;
    }
}
