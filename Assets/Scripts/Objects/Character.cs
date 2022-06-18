using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;


public abstract class Character : ControlledMonoBehavour
{

    public CodeContext codeContext = new CodeContext();

    public CharacterData characterData;
    public Tweener tweener;
    public bool isMoveThreadRunning;
    public int currentEnergy;
    public Vector2Int gridPos;
    bool canRegen = true;
    public float currentHealth;
    public bool debugMove;
    public int ownerPlayer;
    private List<Vector2Int> moveSet = new List<Vector2Int>();
    private int moveIndex = 0;


    private void Awake()
    {
        if (ownerPlayer == 0)
        {
            codeContext.character = this;
            GameObject.FindObjectOfType<CodeExecutor>().codeContexts.Add(codeContext);
        }
    }


    // Update is called once per frame
    public async void BaseUpdate()
    {
        if (!isMoveThreadRunning && debugMove)
        {
            //continuosMove();
            await Task.Delay(100);
           
        }
    }

    public override void OnStep()
    {
        base.OnStep();
        if (debugMove) moveUnit(1, 0);
        energyRegen();
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
        currentEnergy = characterData.maxEnergy;
        currentHealth = characterData.maxHealth;
        State.GridContents[gridPos.x, gridPos.y].Entity = gameObject;
        transform.position = State.GridContents[gridPos.x, gridPos.y].Object.transform.position;
        energyRegen();
        addUnitToPlayer();
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
                //ErrorManager.instance.PushError(new ErrorSource { function = "movePlayer", playerId = gameObject.name }, new Error("Can't move there"));
            }

        }
    }

    public bool checkPosOnGrid(Vector2Int pos)
    {
        try { return State.GridContents[pos.x, pos.y].Entity == null; }
        catch(IndexOutOfRangeException) { return false; }
    }


    public List<Character> checkForInRangeEnemies()
    {
        List<Character> foundCharacters = new List<Character>();
        for (int x = -characterData.range; x <= characterData.range; x++)
        {
            for (int y = -characterData.range; y <= characterData.range; y++)
            {
                try
                 {
                    if (State.GridContents[gridPos.x + x, gridPos.y + y].Entity && State.GridContents[gridPos.x + x, gridPos.y + y].Entity != gameObject)
                    {
                        
                        foundCharacters.Add(State.GridContents[gridPos.x + x, gridPos.y + y].Entity.GetComponent(typeof(Character)) as Character);
                    }
                }
                catch (IndexOutOfRangeException) { }
            }
        }
        return foundCharacters;
    }

    [MoonSharp.Interpreter.MoonSharpHidden]
    public void die()
    {
        Destroy(gameObject);
    }


    [MoonSharp.Interpreter.MoonSharpHidden]
    public float takeDamage(float damage)
    {
        if (currentHealth - damage > 0)
            return currentHealth -= damage;

        return -1;
    }

    abstract public float attack(Character enemy);
}
