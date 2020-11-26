﻿using System;
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
        Generate(new Vector2Int(0,0), 0);
    }

    private void Generate(Vector2Int iniPos, int iniOrientation)
    {
        List<Door> openDoors = new List<Door>();
        Grid grid = new Grid(tileSize, heightSize, new Vector2Int(maxX, maxY), 0);

        (GameObject iniRoomPrefab, Room iniRoom) = randomChooseRoom();
        grid.SpawnRoom(iniRoomPrefab, iniRoom, iniPos, iniRoom.GetRoomBoundaries(), iniOrientation, iniRoom.GetDoors());
        openDoors.AddRange(iniRoom.GetDoors());

        int numRoomsSpawned = 1;
        while (numRoomsSpawned <= temptativeSize)
        {
            numRoomsSpawned += 1;
            int randOpen = UnityEngine.Random.Range(0, openDoors.Count);
            Door doorToSpawnFrom = openDoors[randOpen];
            
            int attemptsToSpawnFromDoor = 0;
            bool validSpawn = false;
            while (!validSpawn && attemptsToSpawnFromDoor < doorStubbornness)
            {
                attemptsToSpawnFromDoor += 1;
                (GameObject targetRoomPrefab, Room targetRoom) = randomChooseRoom();
                List<Door> targetRoomDoors = targetRoom.GetDoors();

                int attemptsToSpawnRoom = 0;
                while (!validSpawn && attemptsToSpawnRoom < roomStubbornness)
                {
                    attemptsToSpawnRoom += 1;

                    int randTarget = UnityEngine.Random.Range(0, targetRoomDoors.Count);
                    Door targetJoinDoor = targetRoomDoors[randTarget];

                    Vector2Int roomCoordinatesOriginPos;
                    Tuple<Vector2Int, Vector2Int> roomBoundaries;
                    float rotation;
                    List<Door> doors;
                    (validSpawn, roomCoordinatesOriginPos, roomBoundaries, rotation, doors) = grid.CheckRoomSpawnValidity(targetRoom, doorToSpawnFrom, targetJoinDoor);
                    if (validSpawn)
                    {
                        // TODO: SPAWN KEYS, FIX DOORS MATCHING
                        grid.SpawnRoom(targetRoomPrefab, targetRoom, roomCoordinatesOriginPos, roomBoundaries, rotation, doors);
                        openDoors.AddRange(doors);
                    }
                }
            }
            if (!validSpawn)
            {
                //TODO: TREAT INVALID SPAWN
            }
        }
    }

    private (GameObject, Room) randomChooseRoom()
    {
        string randRoom = UnityEngine.Random.Range(1, 4).ToString();
        GameObject roomPrefab = (GameObject)Resources.Load("Prefabs/Room_Example"+randRoom, typeof(GameObject));
        return (roomPrefab, roomPrefab.GetComponent<Room>());
    }
}
