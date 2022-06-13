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
            Character character = obj.GetComponent(typeof(Character)) as Character;
            character.gridPos = spawnPos;
            character.enabled = true;
            character.initializePlayer(characterType);


            return obj.GetComponent(typeof(Character)) as Character;
        }
        return null;
    }
}
