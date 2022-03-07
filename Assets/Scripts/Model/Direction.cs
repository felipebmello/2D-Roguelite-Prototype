using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { North, South, East, West };

public static class DirectionEnumExtensions {
    
    public static Direction OppositeDirection(Direction dir)
    {   
        if (dir == Direction.North) dir = Direction.South;
        else if (dir == Direction.South) dir = Direction.North;
        else if  (dir == Direction.East) dir = Direction.West;
        else if (dir == Direction.West) dir = Direction.East;
        return dir;
    }
    public static Vector2Int GetValue(Direction dir)
    {   
        Vector2Int value = new Vector2Int (0, 0);
        if (dir == Direction.North) value.y += 1;
        else if (dir == Direction.South) value.y -= 1;
        else if  (dir == Direction.East) value.x += 1;
        else if (dir == Direction.West) value.x -= 1;
        return value;
    }
}

