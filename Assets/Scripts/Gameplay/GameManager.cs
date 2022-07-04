using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public GridManager gridManager;
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

    public void initGameManager(object sender, EventArgs e)
    {
        gridDimensions = new Vector3(gridManager.GridBreadth, gridManager.GridHeight, gridManager.GridWidth);
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
            dragDropManager.gameObject.SetActive(false);
            activePlayer = defendingPlayer;
            dragDropManager.updateChoices(false);
            incrementPhase();
        }
        else
        {
            changeActivePlayers(1);
        }
    }

    void changeActivePlayers(int turn)
    {
        if (turn == 0)
            activePlayer = defendingPlayer;
        else 
            activePlayer = attackingPlayer;

        dragDropManager.updateChoices(activePlayer.isAttacking);
    }

    public void incrementPhase()
    {
        currentPhase++;
        if (currentPhase == 3)
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
                defendingPlayer.goldLeft += gameData.defenceGoldIncrease[round - 1];
                attackingPlayer.goldLeft += gameData.attackGoldIncrease[round - 1];
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
    
    public static GameObject spawnOnGrid(GameObject obj, Vector2Int pos)
    {
        if (State.validMovePosition(pos))
        {
            GameObject instance = Instantiate(obj, Vector3.zero, Quaternion.identity);
            placeOnGrid(instance, pos);
            return instance;
        }
        return null;
    }

    public static void placeOnGrid(GameObject obj, Vector2Int pos)
    {
        obj.transform.position = State.gridToWorldPos(pos);
        Transform mesh = findTopLayerMesh(findTopLayerMesh(obj.transform));
        float y = mesh.GetComponent<Renderer>().localBounds.extents.y * mesh.localScale.y + (gridDimensions.y / 2); // needs to be recursive search for the first renderer
        obj.transform.position += new Vector3(0, y, 0);
        State.GridContents[pos.x, pos.y].Entity = obj;

        obj.GetComponentInChildren<Entity>().gridPos = pos;
    }

    public static Transform findTopLayerMesh(Transform obj)
    {
        if (!obj.GetComponent<Renderer>() && obj.childCount > 0)
        {
            return findTopLayerMesh(obj.GetChild(0));
        }
        return obj;
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

    public static T findClosest<T>(Character sender, bool ignoreOwn) where T : Entity
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
