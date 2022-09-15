using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public Character spawnUnit(string characterType, Vector2Int spawnPos)
    {
        GameObject prefab = Resources.Load("Prefabs/" + characterType + "Prefab") as GameObject;
        return GridManager.spawnOnGrid(prefab, spawnPos).GetComponent<Character>();
    }

}
