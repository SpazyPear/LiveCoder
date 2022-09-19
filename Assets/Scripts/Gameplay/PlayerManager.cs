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
    public bool isLocalPlayer => LocalPlayer.IsLocal;
    public List<Entity> units;
    public bool isLeftSide;

    void Awake()
    {
        LocalPlayer = PhotonNetwork.LocalPlayer;
        creditsLeft = new BindableValue<int>((x) => GameObject.FindObjectOfType<UIManager>().updateCreditUI(x));
        initPlayer();
    }

    void Start()
    {
        PhotonNetwork.SetPlayerCustomProperties(new Hashtable { { "PlayerID", playerID } });
        spawnStartingObjects();
    }

    void initPlayer()
    {
        creditsLeft.value = gameData.initialGold;
        isLeftSide = PhotonNetwork.IsMasterClient;
        gameData = Resources.Load("Scriptableobjects/GameScriptableObject") as GameData;
    }

    void spawnStartingObjects()
    {
        spawnUnit("BaseTower", isLeftSide ? new Vector2Int(GridManager.GridContents.GetLength(0) / 2, GridManager.GridContents.GetLength(1) - 1) : new Vector2Int(GridManager.GridContents.GetLength(0) / 2, 0));
        spawnUnit("CoinStore", isLeftSide ? new Vector2Int(GridManager.GridContents.GetLength(0) / 2 + 1, GridManager.GridContents.GetLength(1) - 1) : new Vector2Int(GridManager.GridContents.GetLength(0) / 2 + 1, 0));
    }

    public Entity spawnUnit(string entityType, Vector2Int spawnPos, bool ignoreCost = false)
    {
        GameObject prefab = Resources.Load("Prefabs/" + entityType) as GameObject;
        if (prefab)
        {
            if (GridManager.validMovePosition(spawnPos))
            {
                if (prefab.GetComponentInChildren<Entity>().cost <= creditsLeft.value)
                {
                    creditsLeft.value -= prefab.GetComponentInChildren<Entity>().cost;
                    Entity entity = GridManager.spawnOnGrid(prefab, spawnPos, false, isLeftSide).GetComponentInChildren<Entity>();
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
