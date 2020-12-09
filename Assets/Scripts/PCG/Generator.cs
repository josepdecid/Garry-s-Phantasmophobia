using System;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField]
    private int temptativeSize = 10;
    [SerializeField]
    private int doorStubbornness = 10;
    [SerializeField]
    private int roomStubbornness = 10;

    [SerializeField]
    private int tileSize = 6;
    [SerializeField]
    private int heightSize = 4;
    [SerializeField]
    private int maxX = 40;
    [SerializeField]
    private int maxY = 40;

    void Start()
    {
        Generate(new Vector2Int(0+(maxX/2),0), 0);
    }

    private void Generate(Vector2Int iniPos, int iniOrientation)
    {
        HashSet<Door> openDoors = new HashSet<Door>(new DoorEqualsComparer());
        Floor grid = new Floor(tileSize, heightSize, new Vector2Int(maxX, maxY), 0);

        (GameObject iniRoomPrefab, Room iniRoom) = GetPrefabRoom("0");

        (Vector2Int newIniPos, Tuple<Vector2Int, Vector2Int> iniRoomBoundaries, List<Door> iniRoomDoors) = grid.GetIniRoomProperties(iniRoom, iniPos, iniOrientation);
        iniRoom = grid.SpawnRoom(iniRoomPrefab, iniRoom, newIniPos, iniRoomBoundaries, iniOrientation, iniRoomDoors);
        openDoors.UnionWith(new HashSet<Door>(iniRoom.GetDoors(), new DoorEqualsComparer()));

        int numRoomsSpawned = 1;
        while (numRoomsSpawned <= temptativeSize)
        {
            Debug.Log("length open doors: "+ openDoors.Count.ToString());
            numRoomsSpawned += 1;
            Door doorToSpawnFrom = RandomChooseDoor(openDoors);
            
            int attemptsToSpawnFromDoor = 0;
            bool validSpawn = false;
            while (!validSpawn && attemptsToSpawnFromDoor < doorStubbornness)
            {
                attemptsToSpawnFromDoor += 1;
                (GameObject targetRoomPrefab, Room targetRoom) = RandomChooseRoom();
                List<Door> targetRoomDoors = targetRoom.GetDoors();

                int attemptsToSpawnRoom = 0;
                while (!validSpawn && attemptsToSpawnRoom < roomStubbornness)
                {
                    attemptsToSpawnRoom += 1;
                    Door targetJoinDoor = RandomChooseDoor(targetRoomDoors);

                    Vector2Int roomCoordinatesOriginPos;
                    Tuple<Vector2Int, Vector2Int> roomBoundaries;
                    float rotation;
                    List<Door> doors;
                    (validSpawn, roomCoordinatesOriginPos, roomBoundaries, rotation, doors) = grid.CheckRoomSpawnValidity(targetRoom, doorToSpawnFrom, targetJoinDoor);
                    if (validSpawn)
                    {
                        Room room = grid.SpawnRoom(targetRoomPrefab, targetRoom, roomCoordinatesOriginPos, roomBoundaries, rotation, doors);
                        grid.FixDoorMatching(room, openDoors);
                    }
                }
            }
            if (!validSpawn)
            {
                //TODO: TREAT INVALID SPAWN
            }
        }
    }

    private (GameObject, Room) RandomChooseRoom()
    {
        string randRoom = UnityEngine.Random.Range(1, 15).ToString();
        return GetPrefabRoom(randRoom);
    }

    private (GameObject, Room) GetPrefabRoom(string r)
    {
        GameObject roomPrefab = (GameObject)Resources.Load("Prefabs/Room_Example"+r, typeof(GameObject));
        return (roomPrefab, roomPrefab.GetComponent<Room>());
    }

    private Door RandomChooseDoor(HashSet<Door> doors)
    {
        int randIdx = UnityEngine.Random.Range(0, doors.Count);
        int currentIdx = 0;
        foreach(Door d in doors) {
            if (currentIdx == randIdx){
                return d;
            }
            ++currentIdx;
        }
        return null;
    }

    private Door RandomChooseDoor(List<Door> doors)
    {
        int randIdx = UnityEngine.Random.Range(0, doors.Count);
        return doors[randIdx];
    }
}

