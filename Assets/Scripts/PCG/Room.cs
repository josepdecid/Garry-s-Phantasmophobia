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
    [SerializeField]
    private List<Window> windows = new List<Window>();

    // The color of curtains to use inside of one room
    // See door_curtain_N and window_curtain_N prefabs
    public String curtainColorNumber = "1";

    public void UpdateRoom(Vector2Int roomCoordinatesOriginPos, Tuple<Vector2Int, Vector2Int> roomBoundaries, float rotation, List<Door> doors, List<Window> windows)
    {
        this.roomPos = roomCoordinatesOriginPos;
        this.roomBottomLeft = roomBoundaries.Item1;
        this.roomTopRight = roomBoundaries.Item2;
        this.rotation = rotation;

        for (int i = 0; i < doors.Count; ++i) {
            this.doors[i].SetInnerPos(doors[i].GetInnerPos());
            this.doors[i].SetOuterPos(doors[i].GetOuterPos());
        }
        for (int i = 0; i < windows.Count; ++i) {
            this.windows[i].SetInnerPos(windows[i].GetInnerPos());
            this.windows[i].SetOuterPos(windows[i].GetOuterPos());
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

    public List<Window> GetWindows()
    {
        return windows;
    }

    // Return truw if given (tile) position is within room's boundaries
    public Boolean CheckPositionWithinRoom(Vector2Int pos) {
        if(pos.x >= roomBottomLeft.x 
        && pos.x <= roomTopRight.x 
        && pos.y >= roomBottomLeft.y 
        && pos.y <= roomTopRight.y) {
            return true;
        }
        return false;
    }
}