using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
/*    public GameObject playerPrefab;
    public GameObject player;
    public GridManager gridManager;
    public Tweener tweener;
    public bool isMoveThreadRunning;
    public CharacterData character;
    public Vector2Int gridPos;
    bool canRegen = true;
    public Vector2Int startPos;
    public bool debugMove;

    // Start is called before the first frame update
    async void Start()
    {
        CharacterData character = new SoldierData();
        healthRegen();
        await Task.Delay(1000);
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
            character.currentEnergy++;
        }
    }

    async void continuosMove()
    {
        isMoveThreadRunning = true;
        State.GridContents[gridPos.x, gridPos.y].Entity = null;
        movePlayer(0, 1);
        await tweener.waitForComplete();
        State.GridContents[gridPos.x, gridPos.y].Entity = player;
        await Task.Delay(500);
        isMoveThreadRunning = false;
    }

    public void initializePlayer()
    {
        Vector3 spawnPos = State.GridContents[startPos.x, startPos.y].Object.transform.position;
        player = Instantiate(playerPrefab, spawnPos, Quaternion.identity, this.transform);
        gridPos = new Vector2Int(startPos.x, startPos.y);
        State.GridContents[startPos.x, startPos.y].Entity = gameObject;
    }

    public void movePlayer(int XDirection, int YDirecton)
    {
        if (character.currentEnergy > 0)
        {
            if (checkPosOnGrid(new Vector2Int(gridPos.x + XDirection, gridPos.y + YDirecton)))
            {
                gridPos = new Vector2Int(gridPos.x + XDirection, gridPos.y + YDirecton);
                tweener.AddTween(player.transform, player.transform.position, State.GridContents[gridPos.x, gridPos.y].Object.transform.position, character.playerSpeed);
                character.currentEnergy -= XDirection + YDirecton;
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
        for (int x = -character.range; x < character.range; x++)
        {
            for (int y = -character.range; y < character.range; y++)
            {
                try
                {
                    if (State.GridContents[gridPos.x + x, gridPos.y + y].Entity)
                    {
                        foundCharacters.Add(State.GridContents[gridPos.x + x, gridPos.y + y].Entity.GetComponent<PlayerManager>().character);
                    }
                }
                catch (IndexOutOfRangeException) { }
            }
        }
        return foundCharacters;
    }*/
}
