using System;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] int maxRooms = 32;
    [SerializeField] int maxNumberOfNeighbours = 3;
    [SerializeField] Room roomPrefab;
    private static Vector2Int origin = new Vector2Int (4, 4);
    private int genIterations = 0;
    [SerializeField] private List<Room> rooms = new List<Room>();
    [SerializeField] private List<Vector2Int> roomsGrid = new List<Vector2Int>();
    [SerializeField] int maxDirectionRetries = 20;

    public event Action onLevelCreatedAction;

    // Start is called before the first frame update
    void Start()
    {
        CreateAllRooms();
    }
    private void CreateAllRooms()
    {
        //Rooms are set in a 9x9 grid, so the origin is set to the 4x4 position
        Vector2Int currentRoomPos = origin;
        for (int index = 0; index < maxRooms; index++)
        {
            Room currentRoom = Instantiate(roomPrefab, this.transform);
            currentRoom.SetRoomNumber(index);
            roomsGrid.Add(currentRoomPos);
            currentRoom.SetPositionInGrid(currentRoomPos);
            //Sets the Room Type to Spawn Room
            if (index == 0) {
                currentRoom.SetRoomType(0);
            }
            //Sets the Room Type to Normal Room
            else 
            {
                currentRoom = PopulateDoors(currentRoom);
                currentRoom.SetRoomType(1);
            }
            rooms.Add(currentRoom);
            if (index < maxRooms-1) 
            {
                int deadEndCount = 0;
                while (IsDeadEnd(currentRoom, Enum.GetNames(typeof(Direction)).Length))
                {
                    Debug.Log("Room number " + currentRoom.GetRoomNumber() +
                        " is a dead end.");
                    deadEndCount++;
                    if (deadEndCount > rooms.Count)
                    {
                        ResetRoomsCreation();
                        return;
                    }
                    currentRoom = rooms[rooms.Count - deadEndCount];
                    currentRoomPos = roomsGrid[roomsGrid.Count - deadEndCount];
                    Debug.Log("Returning to previous room " + currentRoom.GetRoomNumber() +
                        " to check for other paths");
                }
                currentRoomPos = DetermineNextRoom(currentRoomPos);
                if (currentRoomPos == new Vector2Int(9, 9)) 
                {
                    ResetRoomsCreation();
                    return;
                }
            }
            else 
            {
                if (CountNeighbours(currentRoomPos) == 1) currentRoom.SetRoomType(2);
            }

        }
        onLevelCreatedAction.Invoke();
    }
    private Vector2Int DetermineNextRoom(Vector2Int currentRoomPos)
    {
        bool checkNextRoom = false;
        int gridIndex = roomsGrid.FindIndex(x => x.Equals(currentRoomPos));
        int dirTries=0;
        int roomTries=0;
        while (!checkNextRoom)
        {
            if (dirTries > maxDirectionRetries) {
                dirTries=0;
                roomTries++;
                if (gridIndex-roomTries >= 0) {
                    gridIndex -= roomTries;
                    currentRoomPos = roomsGrid[gridIndex];
                }
                else 
                {
                    return new Vector2Int (9, 9);
                }
            }
            Direction dir = (Direction) UnityEngine.Random.Range(
                0, 
                Enum.GetNames(typeof(Direction)).Length);
            Debug.Log("Trying to move to "+dir+
                " from Room "+gridIndex+
                ", position "+currentRoomPos);
            if (rooms[gridIndex].ContainsDoor(dir)) 
            {
                Debug.Log("There's already a Room " +
                    "in the "+dir+" direction...");
                dirTries++;
            }
            else 
            {
                Vector2Int nextRoomPos = AddDirectionToPos(dir, currentRoomPos);
                if (!currentRoomPos.Equals(nextRoomPos)) 
                {
                    int count = CountNeighbours(nextRoomPos, 
                        DirectionEnumExtensions.OppositeDirection(dir));
                    if (count < maxNumberOfNeighbours) 
                    {
                        if (CountNeighbours(currentRoomPos) == 1 && 
                            UnityEngine.Random.Range (0, 2) == 0 &&
                            rooms[gridIndex].GetRoomType() != 0) 
                        {
                            Debug.Log ("End Room reached at "+gridIndex+" "+currentRoomPos);
                            roomTries++;
                            dirTries = 0;
                            rooms[gridIndex].SetRoomType(2);
                            if (gridIndex-roomTries >= 0) {
                                    gridIndex -= roomTries;
                                    currentRoomPos = roomsGrid[gridIndex];
                            }
                            else 
                            {
                                return new Vector2Int (9, 9);
                            }
                         } 
                        else 
                        {
                            checkNextRoom = true;
                            rooms[gridIndex].AddDoor(dir);
                            currentRoomPos = nextRoomPos;
                            Debug.Log("Sucessfully moved "+dir+
                                ", new Room position is "+currentRoomPos);

                        }
                    }
                    else 
                    {
                        Debug.Log("Looking for another direction...");
                        dirTries++;
                        if (dirTries > maxDirectionRetries) {
                            dirTries=0;
                            roomTries++;
                            if (gridIndex-roomTries >= 0) 
                            {
                                    gridIndex -= roomTries;
                                    currentRoomPos = roomsGrid[gridIndex];
                            }
                            else 
                            {
                                return new Vector2Int (9, 9);
                            }
                        }
                    }
                }
            }
        }
        return currentRoomPos;
    }
    private int CountNeighbours(Vector2Int pos, Direction exitDir)
    {
        int neighbourCount = 1;
        foreach (Direction dir in Enum.GetValues(typeof(Direction))) 
        {
            if (dir != exitDir)
            {
                Vector2Int nextPos = AddDirectionToPos(dir, pos);
                int gridIndex = roomsGrid.FindIndex(x => x.Equals(nextPos));
                if (gridIndex != -1) 
                {
                    if (!nextPos.Equals(pos)) neighbourCount++;
                }
            }
        }
        Debug.Log("Next possible position has "+neighbourCount+
                            " neighbours populated!");
        return neighbourCount;
    }
    private int CountNeighbours(Vector2Int pos)
    {
        int neighbourCount = 0;
        foreach (Direction dir in Enum.GetValues(typeof(Direction))) 
        {
            Vector2Int nextPos = AddDirectionToPos(dir, pos);
            int gridIndex = roomsGrid.FindIndex(x => x.Equals(nextPos));
            if (gridIndex != -1) 
            {
                if (!nextPos.Equals(pos)) neighbourCount++;
            }
        }
        Debug.Log("Current position has "+neighbourCount+
                            " neighbours populated!");
        return neighbourCount;
    }
    private Room PopulateDoors(Room room)
    {
        foreach (Direction dir in Enum.GetValues(typeof(Direction))) 
        {
            Vector2Int neighbourRoom = room.GetPositionInGrid() + DirectionEnumExtensions.GetValue(dir);
            int gridIndex = roomsGrid.FindIndex(x => x.Equals(neighbourRoom));
            if (gridIndex > -1) 
            {
                if (!room.ContainsDoor(dir)) 
                {
                    room.AddDoor(dir);
                }
                if (!rooms[gridIndex].ContainsDoor(DirectionEnumExtensions.
                    OppositeDirection(dir))) 
                {
                    rooms[gridIndex].AddDoor(DirectionEnumExtensions.
                    OppositeDirection(dir));
                }
                if (rooms[gridIndex].GetRoomType() == 2 &&
                    rooms[gridIndex].GetNumberOfDoors() > 1) 
                {
                    rooms[gridIndex].SetRoomType(1);
                }
            }
            
        }
        return room;
    }
    public void ResetRoomsCreation()
    {
        //foreach (Room r in rooms) Destroy(r);
        rooms = new List<Room>();
        roomsGrid = new List<Vector2Int>();
        genIterations++;
        CreateAllRooms();
    }
    private bool IsDeadEnd(Room room, int maxNumberOfExits)
    {
        if (room.GetNumberOfDoors() == maxNumberOfExits) 
        {
            return true;
        }
        if (room.GetNumberOfDoors() == maxNumberOfExits-1 && (
            room.GetPositionInGrid().x == 0 ||
            room.GetPositionInGrid().x == 8 ||
            room.GetPositionInGrid().y == 0 ||
            room.GetPositionInGrid().y == 8))
        {
            return true;
        }
        if (room.GetNumberOfDoors() == maxNumberOfExits-2)
        {
            if (room.GetPositionInGrid().y == 0 || room.GetPositionInGrid().y == 8) 
            {
                if (room.GetPositionInGrid().x == 0 || room.GetPositionInGrid().x == 8) 
                {
                    return true;
                }
            }
        }
        return false;
    }
    private Vector2Int AddDirectionToPos(Direction dir, Vector2Int currentRoomPos)
    {
        if (dir == Direction.North && currentRoomPos.y < 8)
        {
            currentRoomPos += DirectionEnumExtensions.GetValue(dir);
        }
        if (dir == Direction.South && currentRoomPos.y > 0)
        {
            currentRoomPos += DirectionEnumExtensions.GetValue(dir);
        }
        if (dir == Direction.East && currentRoomPos.x < 8)
        {
            currentRoomPos += DirectionEnumExtensions.GetValue(dir);
        }
        if (dir == Direction.West && currentRoomPos.x > 0)
        {
            currentRoomPos += DirectionEnumExtensions.GetValue(dir);
        }
        return currentRoomPos;
    }
    public List<Room> GetRooms()
    {
        return rooms;
    }
    public Vector2Int GetGridOrigin() 
    {
        return origin;
    }

}
