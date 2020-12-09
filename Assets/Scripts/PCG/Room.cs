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

    public void UpdateRoom(Vector2Int roomCoordinatesOriginPos, Tuple<Vector2Int, Vector2Int> roomBoundaries, float rotation, List<Door> doors)
    {
        this.roomPos = roomCoordinatesOriginPos;
        this.roomBottomLeft = roomBoundaries.Item1;
        this.roomTopRight = roomBoundaries.Item2;
        this.rotation = rotation;

        for (int i = 0; i < doors.Count; ++i) {
            this.doors[i].SetInnerPos(doors[i].GetInnerPos());
            this.doors[i].SetOuterPos(doors[i].GetOuterPos());
        }
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

    public List<Door> GetDoors()
    {
        return doors;
    }
}