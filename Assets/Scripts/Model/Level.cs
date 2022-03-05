using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    
    [SerializeField] int maxRooms = 8;
    [SerializeField] Room roomPrefab;

    private List<Room> rooms = new List<Room>();
    private List<int> roomsIndexes = new List<int>();

    public event Action onLevelCreatedAction;

    // Start is called before the first frame update
    void Start()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        int index, currentRoomIndex = 35;
        bool checkNextRoom = false;
        for (index = 0; index < maxRooms; index++)
        {
            Room currentRoom = Instantiate(roomPrefab, this.transform);
            currentRoom.SetRoomNumber(currentRoomIndex);
            DetermineNextRoom();


            while (!checkNextRoom)
            {
                Room.Direction dir = (Room.Direction) UnityEngine.Random.Range(
                    0, 
                    Enum.GetNames(typeof(Room.Direction)).Length);
                int nextRoomIndex = CheckNeighbour(dir, currentRoomIndex);
                if (nextRoomIndex != currentRoomIndex)
                {
                    checkNextRoom = true;
                    currentRoom.AddDoor(dir);
                    currentRoomIndex = nextRoomIndex;
                }
            }
            //currentRoom.transform.position += (new Vector3 (1,0,0) * index);

            rooms.Add(currentRoom);
            roomsIndexes.Add(currentRoomIndex);
        }
        onLevelCreatedAction.Invoke();
    }

    private int CheckNeighbour(Room.Direction dir, int currentRoomIndex)
    {
        if (dir == Room.Direction.North)
        {
            currentRoomIndex += 10;
        }
        if (dir == Room.Direction.South)
        {
            currentRoomIndex -= 10;
        }
        if (dir == Room.Direction.East)
        {
            currentRoomIndex += 1;
        }
        if (dir == Room.Direction.West)
        {
            currentRoomIndex -= 1;
        }
        return currentRoomIndex;
    }

    public List<Room> GetRooms()
    {
        return rooms;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
