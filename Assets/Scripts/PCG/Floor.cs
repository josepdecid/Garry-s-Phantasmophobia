using UnityEngine;
using System;
using System.Collections.Generic;

public class Floor : MonoBehaviour
{
    private int tileSize;
    private int heightSize;
    private Vector2Int maxSize;
    private List<Room> rooms;
    private Room[,] grid;
    private int floor;

    public Floor(int tileSize, int heightSize, Vector2Int maxSize, int floor)
    {
        this.tileSize = tileSize;
        this.heightSize = heightSize;
        this.maxSize = maxSize;
        this.rooms = new List<Room>();
        this.grid = new Room[maxSize.y, maxSize.x];
        AddRoomToGrid(null, new Tuple<Vector2Int, Vector2Int>(new Vector2Int(0, 0), new Vector2Int(maxSize.x-1, maxSize.y-1)));
        this.floor = floor;
    }

    public Room SpawnRoom(GameObject roomPrefab, Room room, Vector2Int roomPos, Tuple<Vector2Int, Vector2Int> roomBoundaries, float rotation, List<Door> doors)
    {
        Vector3 origin = new Vector3((-tileSize / 2), 0, (-tileSize / 2));
        Quaternion rotMatrix = Quaternion.Euler(0, rotation, 0);
        Vector3 finalPos = rotMatrix * origin;
        finalPos = finalPos - origin + new Vector3(roomPos.x * tileSize, floor * heightSize, roomPos.y * tileSize);

        room = Instantiate(roomPrefab, finalPos, rotMatrix).GetComponent<Room>();
        room.UpdateRoom(roomPos, roomBoundaries, rotation, doors);
        this.rooms.Add(room);
        AddRoomToGrid(room, roomBoundaries);
        return room;
    }

    public (bool, Vector2Int, Tuple<Vector2Int, Vector2Int>, float, List<Door>) CheckRoomSpawnValidity(Room targetRoom, Door doorToSpawnFrom, Door targetJoinDoor)
    {
        float rotation = ComputeNewOrientation(doorToSpawnFrom, targetJoinDoor);
        Tuple<Vector2Int, Vector2Int> newRoomBoundaries = ComputeNewBoundaries(targetRoom, doorToSpawnFrom.GetOuterPos(), targetJoinDoor.GetInnerPos(), rotation);
        Vector2Int newPos = ComputeNewRoomPos(targetRoom, doorToSpawnFrom.GetOuterPos(), targetJoinDoor.GetInnerPos(), rotation);
        List<Door> newDoors = ComputeNewDoorsPos(targetRoom, doorToSpawnFrom.GetOuterPos(), targetJoinDoor.GetInnerPos(), rotation);
        bool isValid = CheckValidity(newRoomBoundaries);
        return (isValid, newPos, newRoomBoundaries, rotation, newDoors);

    }

    public (Vector2Int, Tuple<Vector2Int, Vector2Int>, List<Door>) GetIniRoomProperties(Room room, Vector2Int roomPos, float rotation){
        Tuple<Vector2Int, Vector2Int> newRoomBoundaries = ComputeNewBoundaries(room, roomPos, new Vector2Int(0,0), rotation);
        Vector2Int newPos = ComputeNewRoomPos(room, roomPos, new Vector2Int(0,0), rotation);
        List<Door> newDoors = ComputeNewDoorsPos(room, roomPos, new Vector2Int(0,0), rotation);
        return (newPos, newRoomBoundaries, newDoors);
    }

    public void AddRoomToGrid(Room room, Tuple<Vector2Int, Vector2Int> roomBoundaries) {
        for(int x=roomBoundaries.Item1.x; x <= roomBoundaries.Item2.x; x++){
            for(int y=roomBoundaries.Item1.y; y <= roomBoundaries.Item2.y; y++){
                SetGridRoom(x, y, room);
            }
        }
    }

    public void SetGridRoom(int x, int y, Room room) {
        grid[this.maxSize.y-y-1, x] = room;
    }

    public Room GetGridRoom(int x, int y) {
        return grid[this.maxSize.y-y-1, x];
    }

    private float ComputeNewOrientation(Door doorToSpawnFrom, Door targetJoinDoor)
    {
        Vector2Int currentOrientation = targetJoinDoor.GetOuterPos() - targetJoinDoor.GetInnerPos();
        Vector2Int goalOrientation = doorToSpawnFrom.GetInnerPos() - doorToSpawnFrom.GetOuterPos();

        float angle = Mathf.Atan2(currentOrientation.y, currentOrientation.x) - Mathf.Atan2(goalOrientation.y, goalOrientation.x);

        return angle * Mathf.Rad2Deg;
    }

    private Tuple<Vector2Int, Vector2Int> ComputeNewBoundaries(Room targetRoom, Vector2Int posToSpawnFrom, Vector2Int posTargetJoinDoor, float rotation)
    {
        Vector2Int minPos = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int maxPos = new Vector2Int(int.MinValue, int.MinValue);

        for (int i = targetRoom.GetRoomBoundaries().Item1.x; i <= targetRoom.GetRoomBoundaries().Item2.x; ++i)
        {
            for (int j = targetRoom.GetRoomBoundaries().Item1.y; j <= targetRoom.GetRoomBoundaries().Item2.y; ++j)
            {
                Vector2Int relPos = GetAbsPosition(new Vector2Int(i, j), posToSpawnFrom, posTargetJoinDoor, rotation);

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

    private Vector2Int ComputeNewRoomPos(Room targetRoom, Vector2Int posToSpawnFrom, Vector2Int posTargetJoinDoor, float rotation)
    {
        return GetAbsPosition((targetRoom.GetRoomPos()), posToSpawnFrom, posTargetJoinDoor, rotation);
    }
    
    private List<Door> ComputeNewDoorsPos(Room targetRoom, Vector2Int posToSpawnFrom, Vector2Int posTargetJoinDoor, float rotation)
    {
        List<Door> newDoors = new List<Door>();
        foreach (Door d in targetRoom.GetDoors())
        {
            Vector2Int innerPos = GetAbsPosition(d.GetInnerPos(), posToSpawnFrom, posTargetJoinDoor, rotation);
            Vector2Int outerPos = GetAbsPosition(d.GetOuterPos(), posToSpawnFrom, posTargetJoinDoor, rotation);
            newDoors.Add(new Door(innerPos, outerPos));
        }

        return newDoors;
    }

    private Vector2Int GetAbsPosition(Vector2Int relPos, Vector2Int doorToSpawnFromAbsPos, Vector2Int targetDoorRelPos, float rotation)
    {
        float cos = Mathf.Cos(rotation * Mathf.Deg2Rad);
        float sin = Mathf.Sin(rotation * Mathf.Deg2Rad);

        int posX = relPos.x - targetDoorRelPos.x;
        int posY = relPos.y - targetDoorRelPos.y;

        int relPosX = Convert.ToInt32(posX * cos + posY * sin);
        int relPosY = Convert.ToInt32(posY * cos - posX * sin);

        return new Vector2Int(relPosX + doorToSpawnFromAbsPos.x, relPosY + doorToSpawnFromAbsPos.y);
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

    private bool DoOverlap(Tuple<Vector2Int, Vector2Int> boundaries1, Tuple<Vector2Int, Vector2Int> boundaries2)
    {
        if (boundaries1.Item2.x < boundaries2.Item1.x || boundaries1.Item1.x > boundaries2.Item2.x)
        {
            return false;
        }

        if (boundaries1.Item2.y < boundaries2.Item1.y || boundaries1.Item1.y > boundaries2.Item2.y)
        {
            return false;
        }
        return true;
    }

    private bool OutOfBounds(Tuple<Vector2Int, Vector2Int> boundaries)
    {
        Debug.Log(boundaries);
        Debug.Log(maxSize);
        if (boundaries.Item1.x < 0 || boundaries.Item1.x >= maxSize.x) {
            return true;
        }

        if (boundaries.Item1.y < 0 || boundaries.Item1.y >= maxSize.y) {
            return true;
        }

        if (boundaries.Item2.x < 0 || boundaries.Item2.x >= maxSize.x) {
            return true;
        }

        if (boundaries.Item2.y < 0 || boundaries.Item2.y >= maxSize.y) {
            return true;
        }


        return false;
    }

/*     public void FixDoorMatching(Room newRoom, List<Door> openDoors) {
        List<Door> potentiallyNewOpenDoors = newRoom.GetDoors();
        List<Door> collidingRoomDoors = new List<Door>();
        List<Room> neighbors = GetNeighbors(newRoom);
        foreach(Room n in neighbors) {
            collidingRoomDoors.AddRange(n.GetDoors());
        }

        collidingRoomDoors = FilterGivesIntoSomething(collidingRoomDoors);
        List<Door> collidingNewRoomDoors = FilterGivesIntoSomething(potentiallyNewOpenDoors); //Que no te chafe la lista, porque viene de newRoom.GetDoors(), que sea una nueva

        potentiallyNewOpenDoors = potentiallyNewOpenDoors.


        for door in matching_doors:
		    create_door(door, locked_room_key); // Suponemos que está creada asi que solo añadir aqui la logica de bloquear la puerta

	    for door in unmatching_doors:
		    remove_door(door); // De momento no hacer nada


        // Añadir a openDoors el potentiallyNewOpenDoors y quitar el collidingRoomDoors
    
    } */
}