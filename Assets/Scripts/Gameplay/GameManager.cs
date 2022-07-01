using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridManager gridManager;
    public static Vector3 gridDimensions;
    public GameObject towerPrefab;
    public int numOfOreToSpawn;
    public GameObject orePrefab;
    // Start is called before the first frame update
    void Awake()
    {
        State.gameManager = this;
        State.initializeLevel();
    }

    private async void Start()
    {
        await Task.Delay(1000);
        gridDimensions = new Vector3(gridManager.GridBreadth, gridManager.GridHeight, gridManager.GridWidth);
        spawnTowers();
        spawnOreDeposits();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void spawnTowers()
    {
        GameObject instance = spawnOnGrid(towerPrefab, new Vector2Int(State.GridContents.GetLength(0) / 2, 0));
        instance.GetComponent<Tower>().ownerPlayer = 0;
        GameObject instance2 = spawnOnGrid(towerPrefab, new Vector2Int(State.GridContents.GetLength(0) / 2, State.GridContents.GetLength(1) - 1));
        instance2.GetComponent<Tower>().ownerPlayer = 1;
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

        Entity e = obj.GetComponent<Entity>();

        if (e == null)
        {
            e = obj.GetComponentInChildren<Entity>();
        }

       e.gridPos = pos;
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
            spawnOnGrid(orePrefab, new Vector2Int(Random.Range(0, Mathf.CeilToInt(State.GridContents.GetLength(0) / 2)), Random.Range(0, Mathf.CeilToInt(State.GridContents.GetLength(1)))));
        }

        for (int x = 0; x < numOfOreToSpawn / 2; x++)
        {
            spawnOnGrid(orePrefab, new Vector2Int(Random.Range(Mathf.CeilToInt(State.GridContents.GetLength(0) / 2), 0), Random.Range(0, Mathf.CeilToInt(State.GridContents.GetLength(1)))));
        }

    }
}
