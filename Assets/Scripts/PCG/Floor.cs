using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public Room SpawnRoom(GameObject roomPrefab, Room room, Vector2Int roomPos, Tuple<Vector2Int, Vector2Int> roomBoundaries, float rotation, List<Door> doors, List<Window> windows)
    {
        Vector3 origin = new Vector3((-tileSize / 2), 0, (-tileSize / 2));
        Quaternion rotMatrix = Quaternion.Euler(0, rotation, 0);
        Vector3 finalPos = rotMatrix * origin;
        finalPos = finalPos - origin + new Vector3(roomPos.x * tileSize, floor * heightSize, roomPos.y * tileSize);

        GameObject prefabInstance = Instantiate(roomPrefab, finalPos, rotMatrix);
        room = prefabInstance.GetComponent<Room>();
        room.UpdateRoom(roomPos, roomBoundaries, rotation, doors, windows);
        this.rooms.Add(room);
        AddRoomToGrid(room, roomBoundaries);
        return room;
    }

    public (bool, Vector2Int, Tuple<Vector2Int, Vector2Int>, float, List<Door>, List<Window>) CheckRoomSpawnValidity(Room targetRoom, Door doorToSpawnFrom, Door targetJoinDoor)
    {
        float rotation = ComputeNewOrientation(doorToSpawnFrom, targetJoinDoor);
        Tuple<Vector2Int, Vector2Int> newRoomBoundaries = ComputeNewBoundaries(targetRoom, doorToSpawnFrom.GetOuterPos(), targetJoinDoor.GetInnerPos(), rotation);
        Vector2Int newPos = ComputeNewRoomPos(targetRoom, doorToSpawnFrom.GetOuterPos(), targetJoinDoor.GetInnerPos(), rotation);
        List<Door> newDoors = ComputeNewDoorsPos(targetRoom, doorToSpawnFrom.GetOuterPos(), targetJoinDoor.GetInnerPos(), rotation);
        List<Window> newWindows = ComputeNewWindowsPos(targetRoom, doorToSpawnFrom.GetOuterPos(), targetJoinDoor.GetInnerPos(), rotation);
        bool isValid = CheckValidity(newRoomBoundaries);
        return (isValid, newPos, newRoomBoundaries, rotation, newDoors, newWindows);

    }

    public (Vector2Int, Tuple<Vector2Int, Vector2Int>, List<Door>, List<Window>) GetIniRoomProperties(Room room, Vector2Int roomPos, float rotation){
        Tuple<Vector2Int, Vector2Int> newRoomBoundaries = ComputeNewBoundaries(room, roomPos, new Vector2Int(0,0), rotation);
        Vector2Int newPos = ComputeNewRoomPos(room, roomPos, new Vector2Int(0,0), rotation);
        List<Door> newDoors = ComputeNewDoorsPos(room, roomPos, new Vector2Int(0,0), rotation);
        List<Window> newWindows = ComputeNewWindowsPos(room, roomPos, new Vector2Int(0,0), rotation);
        return (newPos, newRoomBoundaries, newDoors, newWindows);
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
        if (x >= 0 && x < this.maxSize.x && y >= 0 && y < this.maxSize.y) {
            return grid[this.maxSize.y-y-1, x];
        }
        else {
            return null;
        }
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
    
    // TODO: Refactor togerther with ComputeNewRoomPos, since it does the same
    // Recompute positions of the windows after rotation
    private List<Window> ComputeNewWindowsPos(Room targetRoom, Vector2Int posToSpawnFrom, Vector2Int posTargetJoinDoor, float rotation)
    {
        List<Window> newWindows = new List<Window>();
        foreach (Window w in targetRoom.GetWindows())
        {
            Vector2Int innerPos = GetAbsPosition(w.GetInnerPos(), posToSpawnFrom, posTargetJoinDoor, rotation);
            Vector2Int outerPos = GetAbsPosition(w.GetOuterPos(), posToSpawnFrom, posTargetJoinDoor, rotation);

            newWindows.Add(new Window(innerPos, outerPos));
        }

        return newWindows;
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

    public void FixDoorMatching(Room newRoom, HashSet<Door> openDoors) {
        HashSet<Door> potentiallyNewOpenDoors = new HashSet<Door> (newRoom.GetDoors(), new DoorEqualsComparer());
        HashSet<Door> collidingNewRoomDoors = FilterGivesIntoSomething(potentiallyNewOpenDoors);
        potentiallyNewOpenDoors.ExceptWith(collidingNewRoomDoors); 

        List<Room> neighbours = GetNeighbours(newRoom);
        HashSet<Door> collidingRoomDoors = new HashSet<Door>(new DoorEqualsComparer());
        foreach(Room n in neighbours) {
            foreach (Door d in n.GetDoors()) {
                collidingRoomDoors.Add(d);
            }
        }
        collidingRoomDoors = FilterGivesIntoRoom(collidingRoomDoors, newRoom);

        int crd = collidingRoomDoors.Count;
        int cnr = collidingNewRoomDoors.Count;

        (HashSet<Door> matching, HashSet<Door> unmatching) = DoorIntersect(collidingRoomDoors, collidingNewRoomDoors);

        int mat = matching.Count;
        int unm = unmatching.Count;

        foreach (Door d in matching) {
            SpawnDoor(d);
        }

        // Pass a random parameter as indicator for a room
        // to ensure the same type of curtains in one room
        String r = UnityEngine.Random.Range(1, 6).ToString();
        foreach (Door d in unmatching) {
            HideDoor(d, r);
        }

        openDoors.ExceptWith(matching);
        openDoors.ExceptWith(unmatching);
        openDoors.ExceptWith(collidingNewRoomDoors);
        openDoors.UnionWith(potentiallyNewOpenDoors);
    }

    // Hide windows, which goes inside the house
    public void HideInsideWindows(){
        foreach(Room room in this.rooms){
            List<Room> neighbours = GetNeighbours(room);
            // Try to find windows, which do give into another room
            // Then hide them by a curtain
            foreach(Room n in neighbours) {

                foreach (Window w in room.GetWindows()) {
                
                    // Debug.Log("******************************************");
                    // Debug.Log("newRoom: " + room.gameObject.name + ", n:" + n.gameObject.name);
                    // Debug.Log("newRoom.rot: " + room.transform.rotation.y + ", " + room.transform.rotation);
                    // Debug.Log("n.Boundaries: " + n.GetRoomBoundaries());
                    // Debug.Log("w.outerPos: " + w.GetOuterPos());
                    // Debug.Log("w.name: " + w.name);
                    // Debug.Log("------------------------------------------");

                    // If window gives into this neighbor then hide
                    if(n.CheckPositionWithinRoom(w.GetOuterPos())) {
                        String r = UnityEngine.Random.Range(1, 6).ToString();
                        HideWindow(w, r);
                    }
                }
            }
        }
    }

    private HashSet<Door> FilterGivesIntoSomething(HashSet<Door> doors) {
        HashSet<Door> givesIntoSomething = new HashSet<Door>(doors.Where(GivesIntoSomething), new DoorEqualsComparer());
        return givesIntoSomething;
    }

    private bool GivesIntoSomething(Door door) {
        if (door.GetOuterPos().x > maxSize.x || door.GetOuterPos().x < 0 || 
            door.GetOuterPos().y > maxSize.y || door.GetOuterPos().y < 0) {
            return true;
        }
        else {
            return (GetGridRoom(door.GetOuterPos().x, door.GetOuterPos().y) != null);
        }
    }

    private HashSet<Door> FilterGivesIntoRoom(HashSet<Door> doors, Room room) {
        HashSet<Door> givesIntoRoom = new HashSet<Door>(doors.Where(d => GetGridRoom(d.GetOuterPos().x, d.GetOuterPos().y) == room), new DoorEqualsComparer());
        return givesIntoRoom;
    }

    private List<Room> GetNeighbours(Room room) {
        int minX = room.GetRoomBoundaries().Item1.x;
        int maxX = room.GetRoomBoundaries().Item2.x;
        int minY = room.GetRoomBoundaries().Item1.y;
        int maxY = room.GetRoomBoundaries().Item2.y;

        List<Room> neighbours = new List<Room>();
        // Iterate over first and last rows
        for (int i = minX; i <= maxX; ++i) {
            if (GetGridRoom(i, maxY+1) != null) {
                neighbours.Add(GetGridRoom(i, maxY+1));
            }
            if (GetGridRoom(i, minY-1) != null) {
                neighbours.Add(GetGridRoom(i, minY-1));
            }
        }
        // Iterate border columns
        for (int j = minY; j <= maxY; ++j) {
            if (GetGridRoom(maxX+1, j) != null) {
                neighbours.Add(GetGridRoom(maxX+1, j));
            }
            if (GetGridRoom(minX-1, j) != null) {
                neighbours.Add(GetGridRoom(minX-1, j));
            }
        }
        return neighbours;
    }

    private (HashSet<Door>, HashSet<Door>) DoorIntersect (HashSet<Door> doors1, HashSet<Door> doors2) {

        // Intersect
        HashSet<Door> matching = new HashSet<Door>(doors1.Where(d1 => doors2.Count(d2 => DoorsMatch(d1, d2)) == 1), new DoorEqualsComparer());
        // Except
        HashSet<Door> unmatching = new HashSet<Door>(doors1.Where(d1 => doors2.Count(d2 => DoorsMatch(d1, d2)) == 0), new DoorEqualsComparer());
        unmatching.UnionWith(new HashSet<Door>(doors2.Where(d2 => doors1.Count(d1 => DoorsMatch(d1, d2)) == 0), new DoorEqualsComparer()));
        return (matching, unmatching);
    }

    private bool DoorsMatch(Door d1, Door d2) {
        return (d1.GetOuterPos() == d2.GetInnerPos() && d1.GetInnerPos() == d2.GetOuterPos()); 
    }

    private void SpawnDoor(Door door) {
        //TODO: Spawn Keys and lock doors
        bool unlocked = true;
        GameObject doorPrefab = GetPrefabDoor();

        (Vector3 pos, Quaternion rot, Vector2Int orient) = GetDoorExactPosition(door);

        GameObject prefabInstance = Instantiate(doorPrefab, pos, rot);
        KeyDoorController keyDoor = prefabInstance.GetComponentInChildren<KeyDoorController>();
        keyDoor.SetUnlocked(unlocked);
    }

    public void HideDoor(Door door, String r) {
        GameObject hidePrefab = GetPrefabHide(r);
        (Vector3 pos, Quaternion rot, Vector2Int orient) = GetDoorExactPosition(door);

        // Aff a shift depending on the orientation
        float shiftX = 0.22f * orient.x;
        float shiftZ = 0.22f * orient.y;
        pos = new Vector3(pos.x + shiftX, pos.y, pos.z + shiftZ);

        GameObject prefabInstance = Instantiate(hidePrefab, pos, rot);
    }

    public void HideWindow(Window w, String r) {
        GameObject hidePrefab = GetPrefabHideWindow(r);
        (Vector3 pos, Quaternion rot, Vector2Int orient) = GetWindowExactPosition(w);

        // Aff a shift depending on the orientation
        float shiftX = 0.22f * orient.x - 0.13f * orient.y;
        float shiftZ = 0.22f * orient.y + 0.13f * orient.x;
        pos = new Vector3(pos.x + shiftX, pos.y + 1.3f, pos.z + shiftZ);

        GameObject prefabInstance = Instantiate(hidePrefab, pos, rot);
    }

    private GameObject GetPrefabDoor(){
        GameObject doorPrefab = (GameObject)Resources.Load("Walls/door_wrapper", typeof(GameObject));
        return doorPrefab;
    }

    private GameObject GetPrefabHide(String r){
        GameObject prefab = (GameObject)Resources.Load("Decoration/door_curtain_" + r, typeof(GameObject));
        return prefab;
    }

    private GameObject GetPrefabHideWindow(String r){
        GameObject prefab = (GameObject)Resources.Load("Decoration/window_curtain_" + r, typeof(GameObject));
        return prefab;
    }

    private (Vector3, Quaternion, Vector2Int) GetDoorExactPosition(Door door) {
        Vector2Int currentOrientation = new Vector2Int(0, 1);
        Vector2Int goalOrientation = door.GetInnerPos() - door.GetOuterPos();
        float rotation = Mathf.Atan2(currentOrientation.y, currentOrientation.x) - Mathf.Atan2(goalOrientation.y, goalOrientation.x);
        rotation = rotation * Mathf.Rad2Deg;

        Vector3 origin = new Vector3((-tileSize / 2), 0, (-tileSize / 2));
        Quaternion rotMatrix = Quaternion.Euler(0, rotation, 0);
        Vector3 finalPos = rotMatrix * origin - origin + rotMatrix * new Vector3((tileSize/2), 0, 0)
                        + new Vector3(door.GetInnerPos().x * tileSize, floor * heightSize, door.GetInnerPos().y * tileSize);
        
        return (finalPos, rotMatrix, goalOrientation);
    }

    // TODO: Refactor together with GetDoorExactPosition, since it does the same
    private (Vector3, Quaternion, Vector2Int) GetWindowExactPosition(Window w) {
        Vector2Int currentOrientation = new Vector2Int(0, 1);
        Vector2Int goalOrientation = w.GetInnerPos() - w.GetOuterPos();
        float rotation = Mathf.Atan2(currentOrientation.y, currentOrientation.x) - Mathf.Atan2(goalOrientation.y, goalOrientation.x);
        rotation = rotation * Mathf.Rad2Deg;

        Vector3 origin = new Vector3((-tileSize / 2), 0, (-tileSize / 2));
        Quaternion rotMatrix = Quaternion.Euler(0, rotation, 0);
        Vector3 finalPos = rotMatrix * origin - origin + rotMatrix * new Vector3((tileSize/2), 0, 0)
                        + new Vector3(w.GetInnerPos().x * tileSize, floor * heightSize, w.GetInnerPos().y * tileSize);
        
        return (finalPos, rotMatrix, goalOrientation);
    }
}