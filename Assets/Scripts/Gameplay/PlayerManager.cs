using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class PlayerManager : ControlledMonoBehavour
{
    public GameData gameData;
    public int goldLeft { get { return goldLeft; } set { goldLeft = value; GameObject.FindObjectOfType<UIManager>().togglePlayerUI(); } }
    public int playerID;
    public List<Entity> units;
    public bool isAttacking;

    private async void Start()
    {
        await Task.Delay(1000);
        spawnUnit("Soldier", new Vector2Int(1, 1));
        initPlayer();
    }

    void initPlayer()
    {
        GameManager.OnPhaseChange.AddListener(changePhase);
        gameData = Resources.Load("Scriptableobjects/GameScriptableObject") as GameData;
        if (isAttacking)
            goldLeft = gameData.attackGoldIncrease[0];
        else
            goldLeft = gameData.defenceGoldIncrease[0];
    }

    void changePhase(int newPhase)
    {
        
    }

    public Entity spawnUnit(string entityType, Vector2Int spawnPos)
    {
        GameObject prefab = Resources.Load("Prefabs/" + entityType + "Prefab") as GameObject;
        
        if (prefab)
        {
            if (State.validMovePosition(spawnPos))
            {
                if (prefab.GetComponent<Entity>().cost <= goldLeft)
                {
                    goldLeft -= prefab.GetComponent<Entity>().cost;
                    Entity entity = GameManager.spawnOnGrid(prefab, spawnPos).GetComponent<Entity>();
                    entity.ownerPlayer = this;
                    units.Add(entity);

                    return entity;
                }
                else
                    ErrorManager.instance.PushError(new ErrorSource { function = "spawnUnit", playerId = gameObject.name }, new Error("Not enough credits"));
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
