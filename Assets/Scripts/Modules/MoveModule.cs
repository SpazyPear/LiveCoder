using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PythonProxies;
using Photon.Pun;
using System;
using static PythonProxies.UnitProxy;

public class MoveModule : Module
{
    public MoveData moveData { get { return moduleData as MoveData; } private set { } }
    private List<Vector2Int> moveSet = new List<Vector2Int>();
    private int moveIndex = 0;

    protected override void Awake()
    {
        moveData = Resources.Load("ModuleConfig/MoveScriptableObject") as MoveData;
        base.Awake();
    }

    public void MoveTo(Vector2Int pos)
    {
        SetPath(GameObject.FindObjectOfType<Pathfinder>().FindPath(owningUnit.gridPos, pos));
        MoveOnPathNext();
    }

    public void SetPath(List<Vector2Int> path)
    {
        moveIndex = 0;
        moveSet = path;
    }

    public void MoveToObject(PlaceableObject obj)
    {
        SetPath(GameObject.FindObjectOfType<Pathfinder>().FindPath(owningUnit, obj));
        MoveOnPathNext();
    }

    public bool PathCompleted()
    {
        if (moveIndex >= moveSet.Count - 1) return true;
        return false;
    }
    
    public void MoveOnPathNext()
    {
        if (moveIndex < moveSet.Count)
        {
            replicatedMove(moveSet[moveIndex].x, moveSet[moveIndex].y);
            moveIndex += 1;
        }
        else
        {
            ErrorManager.instance.PushError(new ErrorSource
            {
                function = "pathfindingMove",
                playerId = transform.name
            }, new Error("Pathfinding has completed but you're still trying to move"));
        }
    }

    public void moveUnit(int x, int y)
    {
        owningUnit.photonView.RPC("moveUnit", RpcTarget.All, x, y);
    }
    
    [PunRPC]
    public void replicatedMove(int XDirection, int YDirecton)
    {
        if (GridManager.validMovePosition(new Vector2Int(owningUnit.gridPos.x + XDirection, owningUnit.gridPos.y + YDirecton)))
        {
            owningUnit.gridPos = new Vector2Int(owningUnit.gridPos.x + XDirection, owningUnit.gridPos.y + YDirecton);
            GridManager.GridContents[owningUnit.gridPos.x, owningUnit.gridPos.y].OccupyingObject = owningUnit;
            owningUnit.currentEnergy -= 1;
        }
        else
        {
            ErrorManager.instance.PushError(new ErrorSource { function = "movePlayer", playerId = gameObject.name }, new Error("Can't move there"));
        }
    }

    public bool checkPosOnGrid(Vector2Int pos)
    {
        try { return GridManager.GridContents[pos.x, pos.y].OccupyingObject == null; }
        catch (IndexOutOfRangeException) { return false; }
    }

    public override object CreateProxy()
    {
        return new MoveModuleProxy(this);
    }

    public override string displayName()
    {
        return "moveModule";
    }

    protected override void AddPrefab()
    {
        moduleObj = GridManager.InstantiateObject("Prefabs/Modules/MoveModule", transform.position, Quaternion.identity);
        moduleObj.transform.SetParent(transform);
    }
}
