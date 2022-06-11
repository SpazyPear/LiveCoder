using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public Character spawnUnit(string characterType, Vector2Int spawnPos)
    {
        GameObject prefab = Resources.Load("Prefabs/" + characterType + "Prefab") as GameObject;
        if (prefab)
        {
            GameObject obj = Instantiate(prefab, State.GridContents[spawnPos.x, spawnPos.y].Object.transform.position, Quaternion.identity);
            Character character = obj.AddComponent(Type.GetType(characterType)) as Character;
            character.gridPos = spawnPos;
            State.GridContents[spawnPos.x, spawnPos.y].Entity = obj;
            return character;
        }
        return null;
    }
}


//Copy Paste For In Game
/*Soldier soldier;
void Start()
{
    soldier = GameObject.FindObjectOfType<UnitSpawner>().spawnUnit("Soldier", new Vector2Int(0, 0)) as Soldier;
}

// Runs every 1 secoond
void OnStep()
{
    soldier.movePlayer(0, 1);
}*/