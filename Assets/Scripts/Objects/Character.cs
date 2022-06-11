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
    public Vector2Int startPos;
    public bool debugMove;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoveThreadRunning && debugMove)
        {
            continuosMove();
        }
    }

    async void healthRegen()
    {
        while (canRegen)
        {
            await Task.Delay(State.EnergyRegen * 1000);
            currentEnergy++;
        }
    }

    async void continuosMove()
    {
        isMoveThreadRunning = true;
        State.GridContents[gridPos.x, gridPos.y].Entity = null;
        movePlayer(0, 1);
        await tweener.waitForComplete();
        State.GridContents[gridPos.x, gridPos.y].Entity = gameObject;
        await Task.Delay(500);
        isMoveThreadRunning = false;
    }

    public void initializePlayer(string CharacterDataPath)
    {
        /*Vector3 spawnPos = State.GridContents[startPos.x, startPos.y].Object.transform.position;
        characterObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity, this.transform);
        gridPos = new Vector2Int(startPos.x, startPos.y);
        State.GridContents[startPos.x, startPos.y].Entity = gameObject;*/
        tweener = gameObject.AddComponent<Tweener>();
        characterData = Resources.Load(CharacterDataPath) as CharacterData;
        currentEnergy = characterData.maxEnergy;
        healthRegen();
    }

    public void movePlayer(int XDirection, int YDirecton)
    {
        if (currentEnergy > 0)
        {
            if (checkPosOnGrid(new Vector2Int(gridPos.x + XDirection, gridPos.y + YDirecton)))
            {
                gridPos = new Vector2Int(gridPos.x + XDirection, gridPos.y + YDirecton);
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
        if (pos.x < State.GridContents.Length && pos.y < State.GridContents.GetLength(0) && State.GridContents[pos.x, pos.y].Entity == null)
        {
            return true;
        }
        return false;
    }


    public List<Character> checkForInRangeEnemies()
    {
        List<Character> foundCharacters = new List<Character>();
        for (int x = -characterData.range; x < characterData.range; x++)
        {
            for (int y = -characterData.range; y < characterData.range; y++)
            {
                try
                {
                    if (State.GridContents[gridPos.x + x, gridPos.y + y].Entity)
                    {
                        foundCharacters.Add(State.GridContents[gridPos.x + x, gridPos.y + y].Entity.GetComponent(typeof(Character)) as Character);
                    }
                }
                catch (IndexOutOfRangeException) { }
            }
        }
        return foundCharacters;
    }

    abstract public float attack();
}
