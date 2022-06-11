using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public GameObject Object;
    public GameObject Entity;

    public Tile(GameObject Object, GameObject Entity = null)
    {
        this.Object = Object;
        this.Entity = Entity;
    }
    
}
