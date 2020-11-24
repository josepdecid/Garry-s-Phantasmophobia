using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private Vector2Int roomPos;
    private Tuple<Vector2Int, Vector2Int> roomBoundaries;
    private float rotation;
    private List<Door> doors = new List<Door>();

    public Room()
    {
        roomPos = new Vector2Int(0, 0);
        roomBoundaries = new Tuple<Vector2Int, Vector2Int>(new Vector2Int(0, 0), new Vector2Int(1, 0));
        rotation = 0;

        doors.Add(new Door(new Vector2Int(1, 0), new Vector2Int(1, 1)));
        doors.Add(new Door(new Vector2Int(1, 0), new Vector2Int(2, 0)));
        doors.Add(new Door(new Vector2Int(1, 0), new Vector2Int(1, -1)));
    }

    public void UpdateRoom(Vector2Int roomCoordinatesOriginPos, Tuple<Vector2Int, Vector2Int> roomBoundaries, float rotation, List<Door> doors)
    {
        this.roomPos = roomCoordinatesOriginPos;
        this.roomBoundaries = roomBoundaries;
        this.rotation = rotation;
        this.doors = doors;
    }

    public Vector2Int GetRoomPos()
    {
        return roomPos;
    }
    

    public Tuple<Vector2Int, Vector2Int> GetRoomBoundaries()
    {
        return roomBoundaries;
    }

    public float GetRotation()
    {
        return rotation;
    }

    public List<Door> GetDoors()
    {
        return doors;
    }
}