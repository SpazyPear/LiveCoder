using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using PythonProxies;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
public enum CLASSTYPE
{
    Tank,
    Brawler,
    Scout
}

public abstract class Character : Entity
{

    [HideInInspector]
    public CharacterData characterData { get { return entityData as CharacterData; } }
    public Tweener tweener;
    public bool isMoveThreadRunning;
    public int currentEnergy;
    bool canRegen = true;
    public bool debugMove;
    private List<Vector2Int> moveSet = new List<Vector2Int>();
    private int moveIndex = 0;
    public bool startInScene;
    const float shakeDuration = 0.3f;
    const float shakeMagnitude = 1f;

    public override void Start()
    {
        base.Start();
        initializeUnit();
    }

    public override object CreateProxy()
    {
        return new CharacterHandlerProxy(this);
    }

    

    public override void OnStep()
    {
        base.OnStep();
        if (debugMove) moveUnit(1, 0);
        energyRegen();
    }

    public void MoveTo(Vector2Int pos)
    {
        SetPath(GameObject.FindObjectOfType<Pathfinder>().FindPath(gridPos, pos));
        MoveOnPathNext();
    }

    public void SetPath(List<Vector2Int> path)
    {
        moveIndex = 0;
        moveSet = path;
    }

    public void MoveToCharacter (Character character)
    {
        SetPath(GameObject.FindObjectOfType<Pathfinder>().FindPath(this, character));
        MoveOnPathNext();
    }


    public bool PathCompleted()
    {
        if (moveIndex >= moveSet.Count - 1) return true;
        return false;
    }

    public void MoveOnPathNext()
    {
        if (!isDisabled)
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
    }

    void energyRegen()
    {
        currentEnergy = Mathf.Clamp(currentEnergy + 1, 0, characterData.maxEnergy);
    }


    public async virtual void initializeUnit()
    {
        currentEnergy = characterData.maxEnergy;
        currentHealth = characterData.maxHealth;

        while (GridManager.GridContents == null)
            await Task.Yield();

        if (startInScene)
        {
            //GameManager.placeOnGrid(gameObject, gridPos);
        }
        energyRegen();
    }

    public void addUnitToPlayer()
    {
        foreach (PlayerManager player in GameObject.FindObjectsOfType<PlayerManager>())
        {
            if (player.playerID == ownerPlayer.playerID)
                player.units.Add(this);
        }
    }

    public void replicatedMove(int x, int y)
    {
        photonView.RPC("moveUnit", RpcTarget.All, x, y);
    }

    [PunRPC]
    public IEnumerator moveUnit(int XDirection, int YDirecton)
    {
        if (!isDisabled)
        {
            if (checkPosOnGrid(new Vector2Int(gridPos.x + XDirection, gridPos.y + YDirecton)))
            {
                GridManager.GridContents[gridPos.x, gridPos.y].Entity = null;
              
                gridPos = new Vector2Int(gridPos.x + XDirection, gridPos.y + YDirecton);
                GridManager.GridContents[gridPos.x, gridPos.y].Entity = gameObject;
                tweener.AddTween(transform, transform.position, GridManager.GridContents[gridPos.x, gridPos.y].Object.transform.position, characterData.playerSpeed);
                currentEnergy -= 1;
            }
            else
            {
                ErrorManager.instance.PushError(new ErrorSource { function = "movePlayer", playerId = gameObject.name }, new Error("Can't move there"));
            }
        }
        yield return null;
    }

    public bool checkPosOnGrid(Vector2Int pos)
    {
        try { return GridManager.GridContents[pos.x, pos.y].Entity == null || (GridManager.GridContents[pos.x, pos.y].Entity != null && GridManager.GridContents[pos.x, pos.y].Entity.GetComponentInChildren<Trap>() != null); }
        catch(IndexOutOfRangeException) { return false; }
    }

    public void recieveOre(OreDepositData data)
    {
        currentEnergy += data.value;
    }
    
    public void attack (int x, int y)
    {
        if (!isDisabled)
        {
            if (Mathf.Max(x, y) <= characterData.range)
            {
                Entity target = GridManager.getEntityAtPos(gridPos + new Vector2Int(x, y));
                if (target != null && currentEnergy > 0)
                {
                    photonView.RPC("replicatedAttack", RpcTarget.All, target.viewID);
                }
                else
                {
                    ErrorManager.instance.PushError(new ErrorSource { function = "attack", playerId = gameObject.name }, new Error("That target isn't in range."));
                }
            }
            else
            {
                ErrorManager.instance.PushError(new ErrorSource { function = "attack", playerId = gameObject.name }, new Error("Attack position out of range for this unit."));
            }
        }
    }
    
    [PunRPC]
    public virtual IEnumerator replicatedAttack(int targetInstance)
    {
        GameManager.unitInstances[targetInstance].takeDamage(1, this);
        currentEnergy--;
        yield return null;
    }

    public override void OnEMPDisable(float strength)
    {
        base.OnEMPDisable(strength);
    }

}
