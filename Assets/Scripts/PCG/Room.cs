using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField]
    private Vector2Int roomPos;
    [SerializeField]
    private Vector2Int roomBottomLeft;
    [SerializeField]
    private Vector2Int roomTopRight;
    [SerializeField]
    private float rotation;
    [SerializeField]
    private List<Door> doors = new List<Door>();

    public void UpdateRoom(Vector2Int roomCoordinatesOriginPos, Tuple<Vector2Int, Vector2Int> roomBoundaries, float rotation, HashSet<Door> doors)
    {
        this.roomPos = roomCoordinatesOriginPos;
        this.roomBottomLeft = roomBoundaries.Item1;
        this.roomTopRight = roomBoundaries.Item2;
        this.rotation = rotation;
        this.doors = new List<Door>(doors.ToList<Door>());
        
    }

    public Vector2Int GetRoomPos()
    {
        return roomPos;
    }

    public Tuple<Vector2Int, Vector2Int> GetRoomBoundaries()
    {
        return new Tuple<Vector2Int, Vector2Int>(roomBottomLeft, roomTopRight);
    }

    public float GetRotation()
    {
        return rotation;
    }

    public HashSet<Door> GetDoors()
    {
        return new HashSet<Door>(doors, new DoorEqualsComparer());
    }
}