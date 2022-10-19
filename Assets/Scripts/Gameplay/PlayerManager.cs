using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Photon.Realtime;

public class PlayerManager : ControlledMonoBehavour
{
    public Player LocalPlayer;
    public GameData gameData;
    public BindableValue<int> creditsLeft;
    public string playerID => LocalPlayer.UserId;
    public bool isLocalPlayer => PhotonNetwork.IsConnected ? LocalPlayer.IsLocal : true;
    public List<Unit> units;
    public bool isLeftSide;

    void Awake()
    {
        //FileManager.WriteDefaults();
        isLeftSide = PhotonNetwork.IsConnected ? PhotonNetwork.IsMasterClient : isLeftSide;
        LocalPlayer = PhotonNetwork.LocalPlayer;
        creditsLeft = new BindableValue<int>((x) => GameObject.FindObjectOfType<UIManager>().updateCreditUI(x));
        initPlayer();
    }

    void Start()
    {
        spawnStartingObjects();
    }

    void initPlayer()
    {
        creditsLeft.value = gameData.initialGold;
        gameData = Resources.Load("Scriptableobjects/GameScriptableObject") as GameData;
    }

    void spawnStartingObjects()
    {
        spawnEntity("BaseTower", isLeftSide ? new Vector2Int(GridManager.GridContents.GetLength(0) / 2, GridManager.GridContents.GetLength(1) - 1) : new Vector2Int(GridManager.GridContents.GetLength(0) / 2, 0));
        spawnEntity("CoinStore", isLeftSide ? new Vector2Int(GridManager.GridContents.GetLength(0) / 2 + 1, GridManager.GridContents.GetLength(1) - 1) : new Vector2Int(GridManager.GridContents.GetLength(0) / 2 + 1, 0));
    }
    
    public Unit spawnUnit(string unitName, Vector2Int spawnPos, bool ignoreCost = false)
    {
        UnitConfig unitConfig = FileManager.GetUnitConfig(unitName);
        if (unitConfig != null)
        {
            if (GridManager.validMovePosition(spawnPos))
            {
                if (unitConfig.cost <= creditsLeft.value)
                {
                    creditsLeft.value -= unitConfig.cost;
                    Unit unit = GridManager.spawnOnGrid("Unit", spawnPos, false, isLeftSide).GetComponentInChildren<Unit>();
                    unit.InitializeUnit(unitConfig.moduleNames, unitConfig.codeContext, unit.name);
                    unit.ownerPlayer = this;
                    units.Add(unit);
                    return unit;
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

    public Entity spawnEntity(string name, Vector2Int spawnPos, bool ignoreCost = false)
    {
        GameObject prefab = Resources.Load("Prefabs/" + name) as GameObject;
        if (prefab)
        {
            if (GridManager.validMovePosition(spawnPos))
            {
                if (prefab.GetComponentInChildren<Entity>().entityData.cost <= creditsLeft.value)
                {
                    creditsLeft.value -= prefab.GetComponentInChildren<Entity>().entityData.cost;
                    Entity entity = GridManager.spawnOnGrid(name, spawnPos, false, isLeftSide).GetComponentInChildren<Entity>();
                    entity.ownerPlayer = this;
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

    public bool checkUnitOwnerShip(Unit unit)
    {
        return units.Contains(unit);
    }
    
    public void deleteUnit(Unit unit)
    {
        units.Remove(unit);
        PhotonNetwork.Destroy(unit.gameObject);
    }

}
