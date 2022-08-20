using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public GridManager gridManager;
    public CodeExecutor codeExecutor;
    public GameData gameData;
    public UIManager uiManager;
    public static Vector3 gridDimensions;
    public GameObject towerPrefab;
    public GameObject coinStorePrefab;
    public int numOfOreToSpawn;
    public GameObject orePrefab;
    public PlayerManager attackingPlayer;
    public PlayerManager defendingPlayer;
    [HideInInspector]
    public static PlayerManager activePlayer;
    public DragDropManager dragDropManager;
    int currentPhase;
    int playersReadied;
    int round = 0;
    public int counter = 0;
    public static UnityEvent<int> OnPhaseChange = new UnityEvent<int>();
    public static UnityEvent OnAttackUnitsCleared = new UnityEvent();


    // Start is called before the first frame update
    void Awake()
    {
        State.gameManager = this; 
    }

    void Start()
    {
        State.onLevelLoad += initGameManager;
        State.initializeLevel();
    }


    public void SetIDs(int phase)
    {
        if (phase == 1)
        {
            int counter = 0;
            foreach (Entity e in GameObject.FindObjectsOfType<Entity>())
            {
                e.ID = counter++;
            }
        }
    }

    public void initGameManager(object sender, EventArgs e)
    {
        gridDimensions = new Vector3(gridManager.GridBreadth, gridManager.GridHeight, gridManager.GridWidth);
        OnPhaseChange.AddListener(SetIDs);
        OnPhaseChange.AddListener(phaseEnter);
        OnAttackUnitsCleared.AddListener(unitsCleared);
        spawnStartingObjects();
        spawnOreDeposits();
        OnPhaseChange.Invoke(0);
    }

    public void OnReadyUp()
    {
        playersReadied++;
        if (playersReadied >= 2)
        {
            playersReadied = 0;
            

            incrementPhase();
        }
        else
        {
            changeActivePlayers(1);
            uiManager.togglePlayerUI();
        }
    }

    void changeActivePlayers(int turn)
    {
        if (turn == 0)
            activePlayer = defendingPlayer;
        else 
            activePlayer = attackingPlayer;

    }

    public void incrementPhase()
    {
        currentPhase++;
        if (currentPhase == 2)
            currentPhase = 0;

        OnPhaseChange.Invoke(currentPhase);
    }

    public void phaseEnter(int phase)
    {
        switch (phase)
        {
            case 0:
                round++;
                changeActivePlayers(0);
                uiManager.togglePlayerUI();
                defendingPlayer.creditsLeft.value += gameData.defenceGoldIncrease[round - 1];
                attackingPlayer.creditsLeft.value += gameData.attackGoldIncrease[round - 1];
                break;
            case 1:
                dragDropManager.gameObject.SetActive(false);
                activePlayer = defendingPlayer;
                uiManager.togglePlayerUI();
                codeExecutor.RunCode();
                break;
        }
    }

    void unitsCleared()
    {
        OnPhaseChange.Invoke(2);
    }

    void spawnStartingObjects()
    {
        GameObject instance = spawnOnGrid(towerPrefab, new Vector2Int(State.GridContents.GetLength(0) / 2, 0));
        instance.GetComponentInChildren<Tower>().ownerPlayer = defendingPlayer;
        spawnOnGrid(coinStorePrefab, new Vector2Int((State.GridContents.GetLength(0) / 2) + 1, 0));
    }
    
    public static GameObject spawnOnGrid(GameObject obj, Vector2Int pos, bool ignorePosClash = false)
    {
        if (!State.validMovePosition(pos) && !ignorePosClash)
        {
            return null;
        }
        GameObject instance = Instantiate(obj, Vector3.zero, Quaternion.identity);
        placeOnGrid(instance, pos);
        return instance;
    }

    public static void placeOnGrid(GameObject obj, Vector2Int pos)
    {
        obj.transform.position = State.gridToWorldPos(pos);
        Transform mesh = obj.GetComponentInChildren<Renderer>().transform;
        float y = (gridDimensions.y / 2);
        obj.transform.position += new Vector3(0, 1, 0);
        State.GridContents[pos.x, pos.y].Entity = obj;

        obj.GetComponentInChildren<Entity>().gridPos = pos;
    }

    public static PlayerManager findPlayer(int playerID)
    {
        foreach (PlayerManager player in GameObject.FindObjectsOfType<PlayerManager>())
        {
            if (player.playerID == playerID)
                return player;
        }
        return null;
    }

    public static T findClosest<T>(T sender, bool ignoreOwn) where T : Entity
    {
        float min = Mathf.Infinity;
        T closest = null;
        foreach (T entity in GameObject.FindObjectsOfType<T>())
        {
            if (Vector2Int.Distance(sender.gridPos, entity.gridPos) < min)
            {
                if (ignoreOwn && entity.ownerPlayer != sender.ownerPlayer)
                    continue;

                min = Vector2Int.Distance(sender.gridPos, entity.gridPos);
                closest = entity;
            }
        }
        return closest;
    }

    public static List<T> checkForInRangeEntities<T>(Vector2Int pos, int range, Entity sender, bool ignoreOwnTeam) where T : Entity
    {
        List<T> foundEntities = new List<T>();
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                try
                {
                    if (State.GridContents[pos.x + x, pos.y + y].Entity && State.GridContents[pos.x + x, pos.y + y].Entity.GetComponentInChildren<Entity>() != sender)
                    {
                        if (ignoreOwnTeam && State.GridContents[pos.x + x, pos.y + y].Entity.GetComponentInChildren<Entity>().ownerPlayer == sender.ownerPlayer) continue;

                        foundEntities.Add(State.GridContents[pos.x + x, pos.y + y].Entity.GetComponentInChildren(typeof(T)) as T);
                    }
                }
                catch (IndexOutOfRangeException) { }
            }
        }
        return foundEntities;
    }

    public static Character findClosestEnemy(Character sender)
    {
        float min = Mathf.Infinity;
        Character closest = null;
        foreach (Character character in GameObject.FindObjectsOfType<Character>())
        {
            if (Vector2Int.Distance(sender.gridPos, character.gridPos) < min && character.ownerPlayer != sender.ownerPlayer)
            {
                min = Vector2Int.Distance(sender.gridPos, character.gridPos);
                closest = character;
            }
        }
        return closest;
    }

    public void spawnOreDeposits()
    {
        for (int x = 0; x < numOfOreToSpawn / 2; x++)
        {
            spawnOnGrid(orePrefab, new Vector2Int(UnityEngine.Random.Range(0, Mathf.CeilToInt(State.GridContents.GetLength(0) / 2)), UnityEngine.Random.Range(0, Mathf.CeilToInt(State.GridContents.GetLength(1)))));
        }

        for (int x = 0; x < numOfOreToSpawn / 2; x++)
        {
            spawnOnGrid(orePrefab, new Vector2Int(UnityEngine.Random.Range(Mathf.CeilToInt(State.GridContents.GetLength(0) / 2), 0), UnityEngine.Random.Range(0, Mathf.CeilToInt(State.GridContents.GetLength(1)))));
        }
    }
}
