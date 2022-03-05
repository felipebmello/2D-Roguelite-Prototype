using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private int roomNumber;
    private int numberOfDoors;

    public enum Direction { North, South, East, West };
    private enum Type { Normal, Special };

    private List<Direction> doorDirections = new List<Direction>();

    public int GetRoomNumber()
    {
        return roomNumber;
    }

    public void SetRoomNumber(int genIndex)
    {
        this.roomNumber = genIndex;
    }

    public void AddDoor(Direction dir)
    {
        numberOfDoors++;
        doorDirections.Add(dir);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
