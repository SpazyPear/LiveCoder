using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public abstract class Character : MonoBehaviour
{
    public CharacterData characterData;
    public Tweener tweener;
    public bool isMoveThreadRunning;
    public int currentEnergy;
    public Vector2Int gridPos;
    bool canRegen = true;
    public float currentHealth;
    public bool debugMove;

    private List<Vector2Int> moveSet = new List<Vector2Int>();
    private int moveIndex = 0;


    // Update is called once per frame
    public async void BaseUpdate()
    {
        if (!isMoveThreadRunning && debugMove)
        {
            continuosMove();
            await Task.Delay(100);
            try
            {

                attack(checkForInRangeEnemies()[0]);
            }
            catch (ArgumentOutOfRangeException e)
            { 
            }
        }
    }

    public void SetPath(List<Vector2Int> path)
    {
        print("Setting path with length " + path.Count);
        moveIndex = 0;
        moveSet = path;
    }

    public bool PathCompleted()
    {
        if (moveIndex >= moveSet.Count - 1) return true;
        return false;
    }

    public void MoveOnPathNext()
    {

        if (moveIndex < moveSet.Count - 1)
        {
            movePlayer(moveSet[moveIndex].x, moveSet[moveIndex].y);
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
        while (canRegen)
        {
            await Task.Delay(State.EnergyRegen * 1000);
            currentEnergy = Mathf.Clamp(currentEnergy + 1, 0, characterData.maxEnergy);
        }
    }

    async void continuosMove()
    {
        isMoveThreadRunning = true;
        movePlayer(0, 1);
        await Task.Delay(1000);
        isMoveThreadRunning = false;
    }

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
    }

    public void movePlayer(int XDirection, int YDirecton)
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

    public void die()
    {
        Destroy(gameObject);
    }

    public float takeDamage(float damage)
    {
        if (currentHealth - damage > 0)
            return currentHealth -= damage;

        return -1;
    }
    abstract public float attack(Character enemy);
}
