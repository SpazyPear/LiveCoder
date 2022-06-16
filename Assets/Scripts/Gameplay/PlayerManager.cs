using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class PlayerManager : ControlledMonoBehavour
{
    public int playerID;
    public List<Character> units;

    public void unitsOfType(string typeName)
    {
       
    }

    public async void Start()
    {
        await Task.Delay(1000);
        spawnUnit("Soldier", new Vector2Int(0, 0));
    }

    public Character spawnUnit(string characterType, Vector2Int spawnPos)
    {
        GameObject prefab = Resources.Load("Prefabs/" + characterType + "Prefab") as GameObject;
        if (prefab)
        {
            if (State.validMovePosition(spawnPos))
            {
                GameObject obj = Instantiate(prefab, State.GridContents[spawnPos.x, spawnPos.y].Object.transform.position, Quaternion.identity);
                Character character = obj.GetComponent(typeof(Character)) as Character;
                character.gridPos = spawnPos;
                character.ownerPlayer = playerID;
                character.enabled = true;
                character.initializePlayer(characterType);
                return obj.GetComponent(typeof(Character)) as Character;
            }
            else
                ErrorManager.instance.PushError(new ErrorSource { function = "spawnUnit", playerId = gameObject.name }, new Error("Can't spawn there"));
        }
        else
            ErrorManager.instance.PushError(new ErrorSource { function = "spawnUnit", playerId = gameObject.name }, new Error("Invalid character name"));

        return null;
    }

    public void win()
    {
        Debug.Log("Won");
    }

    public bool checkUnitOwnerShip(Character unit)
    {
        return units.Contains(unit);
    }


}
