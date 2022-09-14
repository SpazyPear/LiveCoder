using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static PhotonView photonView;
    public GridManager gridManager;
    public CodeExecutor codeExecutor;
    public GameData gameData;
    public UIManager uiManager;
    public static Vector3 gridDimensions;
    public int numOfOreToSpawn;
    public GameObject orePrefab;
    public List<PlayerManager> players;
    [HideInInspector]
    public static PlayerManager activePlayer;
    public DragDropManager dragDropManager;
    int currentPhase;
    int playersReadied;
    int round = 0;
    public int counter = 0;
    public static Dictionary<int, Entity> unitInstances = new Dictionary<int, Entity>(); 
    public static UnityEvent<int> OnPhaseChange = new UnityEvent<int>();
    public static UnityEvent OnAttackUnitsCleared = new UnityEvent();


    // Start is called before the first frame update
    void Awake()
    {
        State.gameManager = this;
        photonView = GetComponent<PhotonView>();
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
            activePlayer = players[0];
        else 
            activePlayer = players[1];

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
                players.ForEach((PlayerManager player) => player.creditsLeft.value += gameData.baseGoldIncrease[round - 1]);
                break;
            case 1:
                dragDropManager.gameObject.SetActive(false);
                activePlayer = players[1];
                uiManager.togglePlayerUI();
                codeExecutor.RunCode();
                break;
        }
    }

    void unitsCleared()
    {
        OnPhaseChange.Invoke(2);
    }
    
    public static GameObject spawnOnGrid(GameObject obj, Vector2Int pos, bool ignorePosClash = false, bool isLeftSide = false)
    {
        if (!State.validMovePosition(pos) && !ignorePosClash)
        {
            return null;
        }
        
        GameObject instance = PhotonNetwork.Instantiate("Prefabs/" + obj.name, Vector3.zero, Quaternion.identity);
        placeOnGrid(instance, pos);
        if (isLeftSide) instance.transform.Rotate(0, 180, 0);


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
        obj.GetComponentInChildren<Entity>().photonView.RPC("replicatedTeleport", RpcTarget.Others, obj.transform.position.x, obj.transform.position.y, obj.transform.position.z, obj.transform.rotation.x, obj.transform.rotation.y, obj.transform.rotation.z, pos.x, pos.y);
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
