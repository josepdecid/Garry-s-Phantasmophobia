using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

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
    private Room stairsRoom = null;
    [SerializeField]
    private int numSpawnRooms = 1;
    [SerializeField]
    private int numRoomsWithoutStair = 14;
    [SerializeField]
    private int numRoomsWithStair = 1;
    [SerializeField]
    private int maxLockedRooms = 2;
    [SerializeField]
    private List<Color> keyDoorOutlineColors;
    [SerializeField]
    private GameObject world;
    [SerializeField]
    private GameObject keyPrefab;
    [SerializeField]
    private GameObject powerUpPrefab;

    private int numRoomsSpawned;
    private int numLockedRooms;
    private bool canSpawnNextFloor = true;


    public void GenerateMap()
    {
        (GameObject iniRoomPrefab, Room iniRoom) = GetPrefabRoom("0");
        Floor firstFloor = Generate(iniRoomPrefab, new Vector2Int(maxX/2,0), 0, 0);
        Floor secondFloor = null;

        if (stairsRoom != null) {
            canSpawnNextFloor = false;
            //secondFloor = Generate(stairsRoom.topRoomPrefab, stairsRoom.GetRoomPos(), stairsRoom.GetRotation(), 1);
        }

        // Add powerups
        AddPowerUps(firstFloor, secondFloor);

        // Adjust size of the sky
        AdjustAtmosphere();
    }

    private void AdjustAtmosphere()
    {
        int maxXY = maxX;
        if (maxY > maxX) {
            maxXY = maxY;
        }

        // Do not scale sky for smaller grid
        if (maxXY < 10) {
            maxXY = 10;
        } 

        float scaleFactor = Mathf.Sqrt(maxXY) * tileSize * 2;
        GameObject.Find("Atmosphere/ManorSky").transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        GameObject.Find("Atmosphere/FogParticles").transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        GameObject.Find("Atmosphere/Plane").transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }

    private Floor Generate(GameObject iniRoomPrefab, Vector2Int iniPos, float iniOrientation, int height)
    {
        HashSet<Door> openDoors = new HashSet<Door>(new DoorEqualsComparer());
        Floor grid = new Floor(tileSize, heightSize, new Vector2Int(maxX, maxY), height, world);
        
        Room iniRoom = iniRoomPrefab.GetComponent<Room>();
        (Vector2Int newIniPos, Tuple<Vector2Int, Vector2Int> iniRoomBoundaries, List<Door> iniRoomDoors, List<Window> iniRoomWindows) = grid.GetIniRoomProperties(iniRoom, iniPos, iniOrientation);
        iniRoom = grid.SpawnRoom(iniRoomPrefab, iniRoom, newIniPos, iniRoomBoundaries, iniOrientation, iniRoomDoors, iniRoomWindows);
        openDoors.UnionWith(new HashSet<Door>(iniRoom.GetDoors(), new DoorEqualsComparer()));

        numRoomsSpawned = 1;
        numLockedRooms = 0;
        while (numRoomsSpawned <= temptativeSize)
        {
            // Debug.Log("length open doors: "+ openDoors.Count.ToString());
            numRoomsSpawned += 1;
            Door doorToSpawnFrom = RandomChooseDoor(openDoors);

            if (doorToSpawnFrom == null) {
                break;
            }
            
            int attemptsToSpawnFromDoor = 0;
            bool validSpawn = false;
            while (!validSpawn && attemptsToSpawnFromDoor < doorStubbornness)
            {
                attemptsToSpawnFromDoor += 1;
                (GameObject targetRoomPrefab, Room targetRoom) = AdvancedRandomChooseRoom();
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
                    List<Window> windows;
                    (validSpawn, roomCoordinatesOriginPos, roomBoundaries, rotation, doors, windows) = grid.CheckRoomSpawnValidity(targetRoom, doorToSpawnFrom, targetJoinDoor);
                    if (validSpawn)
                    {
                        Key key = RandomSpawnKey(grid, height);
                        Room room = grid.SpawnRoom(targetRoomPrefab, targetRoom, roomCoordinatesOriginPos, roomBoundaries, rotation, doors, windows);
                        room.SetKey(key);
                        grid.FixDoorMatching(room, openDoors, key);

                        if (room.IsStair()) {
                            stairsRoom = room;
                        }
                    }
                }
            }
            if (!validSpawn)
            {
                //TODO: TREAT INVALID SPAWN
            }
        }
        foreach (Door d in openDoors){
            grid.HideDoor(d, d.gameObject.transform.parent.GetComponent<Room>().curtainColorNumber);
        }

        grid.HideInsideWindows();

        return grid;
    }

    private (GameObject, Room) RandomChooseRoom()
    {
        string randRoom = UnityEngine.Random.Range(1, 16).ToString();
        return GetPrefabRoom(randRoom);
    }

    private (GameObject, Room) AdvancedRandomChooseRoom()
    {
        string randRoom;
        if (stairsRoom != null || !canSpawnNextFloor) {
            randRoom = UnityEngine.Random.Range(numSpawnRooms, numSpawnRooms + numRoomsWithoutStair).ToString();
        }
        else {
            if (numRoomsSpawned >= temptativeSize/2) {
                randRoom = UnityEngine.Random.Range(numSpawnRooms + numRoomsWithoutStair, numSpawnRooms + numRoomsWithStair + numRoomsWithoutStair).ToString();
            }
            else {
                randRoom = UnityEngine.Random.Range(numSpawnRooms, numSpawnRooms + numRoomsWithStair + numRoomsWithoutStair).ToString();
            }
        }
        
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

    private Key RandomSpawnKey(Floor grid, int numFloor){
        if (numLockedRooms >= maxLockedRooms || numRoomsSpawned <= 2){
            return null;
        }
        else {
            float fractionRoomsToSpawn = (float) (temptativeSize - numRoomsSpawned)/ ((float) temptativeSize);
            float probabilityLocked = (maxLockedRooms - numLockedRooms) * (1 - fractionRoomsToSpawn) * (1 - fractionRoomsToSpawn);

            float rand = UnityEngine.Random.Range(0.0f, 1.0f);
            if (rand < probabilityLocked) {
                int index = numLockedRooms + (numFloor * maxLockedRooms);
                
                Key key = grid.SpawnProp(keyPrefab, $"Key_{index}").GetComponent<Key>();
                key.SetOutlineColor(keyDoorOutlineColors[index]);
                
                numLockedRooms += 1;
                
                return key;
            }
            else {
                return null;
            }
        }
    }

    private void AddPowerUps(Floor first, Floor second) {
        first.SpawnProp(powerUpPrefab, $"Power_1");
        if (second != null) {
            second.SpawnProp(powerUpPrefab, $"Power_2");
        }
    }
}

