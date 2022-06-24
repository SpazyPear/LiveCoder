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


    private void Awake()
    {
        if (ownerPlayer == 0)
        {
            codeContext.character = this;
            GameObject.FindObjectOfType<CodeExecutor>().codeContexts.Add(codeContext);
        }
    }


    // Update is called once per frame


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

    async void energyRegen()
    {
       currentEnergy = Mathf.Clamp(currentEnergy + 1, 0, characterData.maxEnergy);
     
    }

    [MoonSharp.Interpreter.MoonSharpHidden]
    public async void initializePlayer(string CharacterDataPath)
    {
        while (State.GridContents == null)
            await Task.Yield();
        characterData = Resources.Load("ScriptableObjects/" + CharacterDataPath + "ScriptableObject") as CharacterData;
        

        if (startInScene)
            GameManager.placeOnGrid(gameObject, gridPos);
        energyRegen();
        addUnitToPlayer();
    }

    public T initializeCharacterClass<T>(CharacterData data) where T : CharacterData
    {
        T characterSpecificData = data as T;
        currentEnergy = characterSpecificData.maxEnergy;
        currentHealth = characterSpecificData.maxHealth;
        return characterSpecificData;
    }

    public void addUnitToPlayer()
    {
        foreach (PlayerManager player in GameObject.FindObjectsOfType<PlayerManager>())
        {
            if (player.playerID == ownerPlayer)
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
                currentEnergy -= XDirection + YDirecton;
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
                        
                        foundCharacters.Add(State.GridContents[gridPos.x + x, gridPos.y + y].Entity.GetComponent(typeof(T)) as T);
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
}
