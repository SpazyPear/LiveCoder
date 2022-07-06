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
    public BindableValue<int> creditsLeft;
    public int playerID;
    public List<Entity> units;
    public bool isAttacking;

    void Awake()
    {
        creditsLeft = new BindableValue<int>((x) => GameObject.FindObjectOfType<UIManager>().updateCreditUI(x));
        initPlayer();
    }

    void initPlayer()
    {
        GameManager.OnPhaseChange.AddListener(changePhase);
        gameData = Resources.Load("Scriptableobjects/GameScriptableObject") as GameData;
        /*if (isAttacking)
            creditsLeft.value += gameData.attackGoldIncrease[0];
        else
            creditsLeft.value += gameData.defenceGoldIncrease[0];*/
    }

    void changePhase(int newPhase)
    {
        
    }

    public Entity spawnUnit(string entityType, Vector2Int spawnPos)
    {
        GameObject prefab = Resources.Load("Prefabs/" + entityType) as GameObject;
        if (prefab)
        {
            if (State.validMovePosition(spawnPos))
            {
                if (prefab.GetComponentInChildren<Entity>().cost <= creditsLeft.value)
                {
                    creditsLeft.value -= prefab.GetComponentInChildren<Entity>().cost;
                    Entity entity = GameManager.spawnOnGrid(prefab, spawnPos).GetComponentInChildren<Entity>();
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
