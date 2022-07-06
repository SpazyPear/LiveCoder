using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;

public static class State
{
    public static event EventHandler onLevelLoad;
    public static Tile[,] GridContents;
    public static int EnergyRegen = 4;
    public static GameManager gameManager;

    public static void initializeLevel()
    {
        onLevelLoad?.Invoke(null, EventArgs.Empty);
    }

    public static bool isPosInBounds(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.y >= 0 && pos.x < GridContents.GetLength(0) && pos.y < GridContents.GetLength(1))
        {
            return true;
        }
        return false;
    }

    public static bool validMovePosition(Vector2Int pos)
    {
        if (isPosInBounds(pos) && GridContents[pos.x, pos.y].Entity == null)
        {
            return true;
        }
        return false;
    }

    public static Vector3 gridToWorldPos(Vector2Int gridPoint)
    {
        return GridContents[gridPoint.x, gridPoint.y].Object.transform.position;
    }

}
