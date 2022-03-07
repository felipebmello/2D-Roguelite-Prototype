using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private int roomNumber;
    [SerializeField] private Vector2Int posInGrid;
    [SerializeField] private Type roomType = Type.Normal;

    private enum Type { Spawn, Normal, End };

    [SerializeField] private List<Direction> doorDirections = new List<Direction>();

    public int GetRoomNumber()
    {
        return roomNumber;
    }
    public void SetRoomNumber(int genIndex)
    {
        this.roomNumber = genIndex;
    }

    internal void SetPositionInGrid(Vector2Int roomPos)
    {
        this.posInGrid = roomPos;
    }
    internal Vector2Int GetPositionInGrid()
    {
        return this.posInGrid;
    }

    public void SetRoomType(int value)
    {
        roomType = (Type) value;
    }
    
    public int GetRoomType()
    {
        return (int) roomType;
    }

    public void AddDoor(Direction dir)
    {
        doorDirections.Add(dir);
    }

    public bool ContainsDoor(Direction dir) 
    {
        return doorDirections.Contains(dir);
    }

    public int GetNumberOfDoors() 
    {
        return doorDirections.Count;
    }
}
