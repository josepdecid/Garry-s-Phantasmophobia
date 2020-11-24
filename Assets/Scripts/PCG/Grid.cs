using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    private int tileSize;
    private int heightSize;
    private Vector2Int maxSize;
    private List<Room> rooms;
    private int floor;

    public Grid(int tileSize, int heightSize, Vector2Int maxSize, int floor)
    {
        this.tileSize = tileSize;
        this.heightSize = heightSize;
        this.maxSize = maxSize;
        this.rooms = new List<Room>();
        this.floor = floor;
       
    }

    public void SpawnRoom(GameObject roomPrefab, Room room, Vector2Int roomPos, Tuple<Vector2Int, Vector2Int> roomBoundaries, float rotation, List<Door> doors)
    {
        room.UpdateRoom(roomPos, roomBoundaries, rotation, doors);
        this.rooms.Add(room);
        Vector3 origin = new Vector3((-tileSize / 2), 0, (-tileSize / 2));
        Quaternion rotMatrix = Quaternion.Euler(0, rotation, 0);
        Vector3 finalPos = rotMatrix * origin;
        finalPos = finalPos - origin + new Vector3(+room.GetRoomPos().x * tileSize, floor * heightSize, (-tileSize / 2) + room.GetRoomPos().y * tileSize);
        Instantiate(roomPrefab, finalPos, rotMatrix);
    }

    public (bool, Vector2Int, Tuple<Vector2Int, Vector2Int>, float, List<Door>) CheckRoomSpawnValidity(Room targetRoom, Door doorToSpawnFrom, Door targetJoinDoor)
    {
        float rotation = ComputeNewOrientation(doorToSpawnFrom, targetJoinDoor);
        Tuple<Vector2Int, Vector2Int> newRoomBoundaries = ComputeNewBoundaries(targetRoom, doorToSpawnFrom, targetJoinDoor, rotation);
        Vector2Int newPos = ComputeNewRoomPos(targetRoom, doorToSpawnFrom, targetJoinDoor, rotation);
        List<Door> newDoors = ComputeNewDoorsPos(targetRoom, doorToSpawnFrom, targetJoinDoor, rotation);
        bool isValid = CheckValidity(newRoomBoundaries);
        return (isValid, newPos, newRoomBoundaries, rotation, newDoors);

    }

    private float ComputeNewOrientation(Door doorToSpawnFrom, Door targetJoinDoor)
    {
        Vector2Int currentOrientation = targetJoinDoor.GetOuterPos() - targetJoinDoor.GetInnerPos();
        Vector2Int goalOrientation = doorToSpawnFrom.GetInnerPos() - doorToSpawnFrom.GetOuterPos();

        float angle = Mathf.Atan2(currentOrientation.y, currentOrientation.x) - Mathf.Atan2(goalOrientation.y, goalOrientation.x);

        return angle * Mathf.Rad2Deg;
    }


    private Vector2Int ComputeNewRoomPos(Room targetRoom, Door doorToSpawnFrom, Door targetJoinDoor, float rotation)
    {
        return GetAbsPosition((targetRoom.GetRoomPos()), targetJoinDoor.GetInnerPos(), doorToSpawnFrom.GetOuterPos(), rotation);
    }

    private Tuple<Vector2Int, Vector2Int> ComputeNewBoundaries(Room targetRoom, Door doorToSpawnFrom, Door targetJoinDoor, float rotation)
    {
        Vector2Int minPos = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int maxPos = new Vector2Int(int.MinValue, int.MinValue);

        for (int i = targetRoom.GetRoomBoundaries().Item1.x; i <= targetRoom.GetRoomBoundaries().Item2.x; ++i)
        {
            for (int j = targetRoom.GetRoomBoundaries().Item1.y; j <= targetRoom.GetRoomBoundaries().Item2.y; ++j)
            {
                Vector2Int relPos = GetAbsPosition(new Vector2Int(i, j), targetJoinDoor.GetInnerPos(), doorToSpawnFrom.GetOuterPos(), rotation);

                if (relPos.x <= minPos.x && relPos.y <= minPos.y)
                {
                    minPos = relPos;
                }

                if (relPos.x >= maxPos.x && relPos.y >= maxPos.y)
                {
                    maxPos = relPos;
                }
            }
        }

        return new Tuple<Vector2Int, Vector2Int>(minPos, maxPos);
    }

    private List<Door> ComputeNewDoorsPos(Room targetRoom, Door doorToSpawnFrom, Door targetJoinDoor, float rotation)
    {
        List<Door> newDoors = new List<Door>();
        foreach (Door d in targetRoom.GetDoors())
        {
            Vector2Int innerPos = GetAbsPosition(d.GetInnerPos(), targetJoinDoor.GetInnerPos(), doorToSpawnFrom.GetOuterPos(), rotation);
            Vector2Int outerPos = GetAbsPosition(d.GetOuterPos(), targetJoinDoor.GetInnerPos(), doorToSpawnFrom.GetOuterPos(), rotation);
            newDoors.Add(new Door(innerPos, outerPos));
        }

        return newDoors;
    }

    private Vector2Int GetAbsPosition(Vector2Int relPos, Vector2Int targetDoorRelPos, Vector2Int doorToSpawnAbsPos, float rotation)
    {
        float cos = Mathf.Cos(rotation * Mathf.Deg2Rad);
        float sin = Mathf.Sin(rotation * Mathf.Deg2Rad);

        int posX = relPos.x - targetDoorRelPos.x;
        int posY = relPos.y - targetDoorRelPos.y;

        int relPosX = Convert.ToInt32(posX * cos + posY * sin);
        int relPosY = Convert.ToInt32(posY * cos - posX * sin);

        return new Vector2Int(relPosX + doorToSpawnAbsPos.x, relPosY + doorToSpawnAbsPos.y);
    }

    private bool CheckValidity(Tuple<Vector2Int, Vector2Int> absBoundaries)
    {
        bool valid = true;
        int i = 0;
        while (valid && i < rooms.Count)
        {
            if (DoOverlap(rooms[i].GetRoomBoundaries(), absBoundaries) || OutOfBounds(absBoundaries))
            {
                valid = false;
            }
            ++i;
        }

        return valid;
    }

    private bool DoOverlap(Tuple<Vector2Int, Vector2Int> coordinates1, Tuple<Vector2Int, Vector2Int> coordinates2)
    {
        if (coordinates1.Item2.x < coordinates2.Item1.x || coordinates1.Item1.x > coordinates2.Item2.x)
        {
            return false;
        }

        if (coordinates1.Item2.y < coordinates2.Item1.y || coordinates1.Item1.y > coordinates2.Item2.y)
        {
            return false;
        }
        return true;
    }

    private bool OutOfBounds(Tuple<Vector2Int, Vector2Int> coordinates)
    {
        return false;
    }
}