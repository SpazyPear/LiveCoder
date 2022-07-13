using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public enum CLASSTYPE
{
    Tank,
    Brawler,
    Scout
}

public abstract class Character : Entity
{

    public CodeContext codeContext = new CodeContext();
    public CharacterData characterData;
    public Tweener tweener;
    public bool isMoveThreadRunning;
    public int currentEnergy;
    bool canRegen = true;
    public bool debugMove;
    private List<Vector2Int> moveSet = new List<Vector2Int>();
    private int moveIndex = 0;
    public bool startInScene;

    public virtual void Start()
    {
       
        
        initializeUnit();
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

        if (moveIndex < moveSet.Count)
        {
            moveUnit(moveSet[moveIndex].x, moveSet[moveIndex].y);
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

    void energyRegen()
    {
        currentEnergy = Mathf.Clamp(currentEnergy + 1, 0, characterData.maxEnergy);
    }


    [MoonSharp.Interpreter.MoonSharpHidden]
    public async virtual void initializeUnit()
    {
        currentEnergy = characterData.maxEnergy;
        currentHealth = characterData.maxHealth;

        while (State.GridContents == null)
            await Task.Yield();

        if (startInScene)
        {
            GameManager.placeOnGrid(gameObject, gridPos);
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

    public void moveUnit(int XDirection, int YDirecton)
    {
        if (currentEnergy > 0)
        {
            if (checkPosOnGrid(new Vector2Int(gridPos.x + XDirection, gridPos.y + YDirecton)))
            {
                State.GridContents[gridPos.x, gridPos.y].Entity = null;
                gridPos = new Vector2Int(gridPos.x + XDirection, gridPos.y + YDirecton);
                State.GridContents[gridPos.x, gridPos.y].Entity = gameObject;
                tweener.AddTween(transform, transform.position, State.GridContents[gridPos.x, gridPos.y].Object.transform.position, characterData.playerSpeed);
                currentEnergy -= 1;
            }
            else
            {
                ErrorManager.instance.PushError(new ErrorSource { function = "movePlayer", playerId = gameObject.name }, new Error("Can't move there"));
            }
        }
    }

    public bool checkPosOnGrid(Vector2Int pos)
    {
        try { return State.GridContents[pos.x, pos.y].Entity == null; }
        catch(IndexOutOfRangeException) { return false; }
    }


    public List<T> checkForInRangeEntities<T>() where T : Entity
    {
        List<T> foundCharacters = new List<T>();
        for (int x = -characterData.range; x <= characterData.range; x++)
        {
            for (int y = -characterData.range; y <= characterData.range; y++)
            {
                try
                 {
                    if (State.GridContents[gridPos.x + x, gridPos.y + y].Entity && State.GridContents[gridPos.x + x, gridPos.y + y].Entity != gameObject)
                    {
                        
                        foundCharacters.Add(State.GridContents[gridPos.x + x, gridPos.y + y].Entity.GetComponentInChildren(typeof(T)) as T);
                    }
                }
                catch (IndexOutOfRangeException) { }
            }
        }
        return foundCharacters;
    }

    public void recieveOre(OreDepositData data)
    {
        currentEnergy += data.value;
    }
    

    public virtual void attack<T> (T target) where T : Entity
    {
        if (target != null && currentEnergy > 0 && checkForInRangeEntities<T>().Contains(target))
        {
            target.takeDamage(1, this);
        }

        else
        {
            ErrorManager.instance.PushError(new ErrorSource { function = "attack", playerId = gameObject.name }, new Error("That target isn't in range."));
        }
        currentEnergy--;

    }

     public override void die(Character sender = null)
    {
        ownerPlayer.units.Remove(this);
        if (ownerPlayer.units.Count == 0)
            GameManager.OnAttackUnitsCleared.Invoke();
        base.die();

    }
}
