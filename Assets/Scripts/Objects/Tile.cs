using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    public GameObject TileObject;
    public PlaceableObject OccupyingObject;
    public Vector2Int gridPosition;

    public Tile(GameObject Object, Vector2Int gridPos, PlaceableObject Unit = null )
    {
        this.TileObject = Object;
        this.OccupyingObject = Unit;
        this.gridPosition = gridPos;
    }
    
}
