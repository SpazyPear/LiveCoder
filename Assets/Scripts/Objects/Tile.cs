using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    public GameObject Object;
    public GameObject Entity;
    public Vector2Int gridPosition;

    public Tile(GameObject Object, Vector2Int gridPos, GameObject Entity = null )
    {
        this.Object = Object;
        this.Entity = Entity;
        this.gridPosition = gridPos;
    }
    
}
